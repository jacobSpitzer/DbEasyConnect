using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.SqlClient;

namespace DbEasyConnect.Tools
{
    internal class ExpressionToSQL : ExpressionVisitor
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _groupBy = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _orderBy = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<SqlParameter> _parameters = new List<SqlParameter>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _select = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _parts = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _join = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _selectMany = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _update = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _where = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? _skip;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? _take;
        private int? _last;
        public string CurrentMethodCall;
        public int _member = 0;
        public string[] _members = new string[] { };
        public string _joinType;
        public bool firstJoin = true;
        public bool _all = false, _any = false;
        public List<string> join_tables = new List<string>();
        public ExpressionToSQL(IQueryable queryable, Expression exp = null, string _join = null)
        {
            if (queryable is null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }
            if (!String.IsNullOrEmpty(_join)) _joinType = _join;
            Expression expression = queryable.Expression;
            Visit(expression);
            if (exp != null) { Visit(exp); }
            Type type = (GetEntityType(expression) as IQueryable).ElementType;
            if (TableName == null)
                TableName = $"[{type.GetTableName()}]";
        }

        public string From => $"FROM {TableName}";
        public string GroupBy => _groupBy.Count == 0 ? null : "GROUP BY " + _groupBy.Distinct().ToList().Join(", ");
        public bool IsDelete { get; private set; } = false;
        public bool IsDistinct { get; private set; }
        public string OrderBy => BuildOrderByStatement().Join(" ");
        public SqlParameter[] Parameters => _parameters.ToArray();
        public string SelectStr => BuildSelectStatement().Join(" ");
        public string Join => _join.Count == 0 ? null : _join.Join(Environment.NewLine);
        public int? Skip => _skip;
        public string TableName { get; private set; }
        public int? Take => _take;
        public int? Last => _last;
        public string UpdateStr => "SET " + _update.Distinct().ToList().Join(", ");
        public string whereType => _all ? "WHERE NOT " : "WHERE ";
        public string Where =>
            _where.Count == 0 ? null : whereType + _where.Distinct().ToList().Join(" AND ");
        public static implicit operator string(ExpressionToSQL simpleExpression) => simpleExpression.ToString();
        public override string ToString() =>
            BuildDeclaration()
                .Union(BuildSqlStatement())
                .Join(Environment.NewLine);

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            this.expressions.Add(binaryExpression);
            if (CurrentMethodCall != nameof(ExpressionToSQL.Update) && CurrentMethodCall != nameof(ExpressionToSQL.Select))
            {
                _parts.Add("(");
                Visit(binaryExpression.Left);
                switch (binaryExpression.NodeType)
                {
                    case ExpressionType.And:
                        _parts.Add("AND");
                        break;

                    case ExpressionType.AndAlso:
                        _parts.Add("AND");
                        break;

                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        _parts.Add("OR");
                        break;

                    case ExpressionType.Equal:
                        if (IsNullConstant(binaryExpression.Right))
                        {
                            _parts.Add("IS");
                        }
                        else
                        {
                            _parts.Add("=");
                        }
                        break;

                    case ExpressionType.NotEqual:
                        if (IsNullConstant(binaryExpression.Right))
                        {
                            _parts.Add("IS NOT");
                        }
                        else
                        {
                            _parts.Add("<>");
                        }
                        break;

                    case ExpressionType.LessThan:
                        _parts.Add("<");
                        break;

                    case ExpressionType.LessThanOrEqual:
                        _parts.Add("<=");
                        break;

                    case ExpressionType.GreaterThan:
                        _parts.Add(">");
                        break;

                    case ExpressionType.GreaterThanOrEqual:
                        _parts.Add(">=");
                        break;

                    default:
                        throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", binaryExpression.NodeType));
                }

                Visit(binaryExpression.Right);
                _parts.Add(")");
            }
            else if (CurrentMethodCall == nameof(ExpressionToSQL.Select))
            {
                _parts.Add("(");
                Visit(binaryExpression.Left);
                switch (binaryExpression.NodeType)
                {
                    case ExpressionType.Add:
                        _parts.Add("+");
                        break;

                    case ExpressionType.Divide:
                        _parts.Add("/");
                        break;


                    case ExpressionType.Multiply:
                        _parts.Add("*");
                        break;

                    case ExpressionType.Modulo:
                        _parts.Add("%");
                        break;


                    case ExpressionType.Increment:
                        _parts.Add("+ 1");
                        break;

                    case ExpressionType.Decrement:
                        _parts.Add("- 1");
                        break;

                    case ExpressionType.Subtract:
                        _parts.Add("-");
                        break;

                    default:
                        throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", binaryExpression.NodeType));
                }
                Visit(binaryExpression.Right);
                _parts.Add(")");
                if (!_parts.All(x => x == ")"))
                {
                    _parts.Add("AS [" + _members[_member] + "]");
                    _member++;
                    _select.Add(_parts.Join(" "));
                }
                else { _select[_select.Count() - 1] = _select.Last().Replace(" AS", _parts[0] + " AS"); }
                _parts.Clear();
            }
            else if (CurrentMethodCall == nameof(ExpressionToSQL.Update))
            {
                _parts.Add("(");
                Visit(binaryExpression.Left);
                switch (binaryExpression.NodeType)
                {
                    case ExpressionType.Add:
                        _parts.Add("+");
                        break;

                    case ExpressionType.Divide:
                        _parts.Add("/");
                        break;


                    case ExpressionType.Multiply:
                        _parts.Add("*");
                        break;

                    case ExpressionType.Modulo:
                        _parts.Add("%");
                        break;


                    case ExpressionType.Increment:
                        _parts.Add("+ 1");
                        break;

                    case ExpressionType.Decrement:
                        _parts.Add("- 1");
                        break;

                    case ExpressionType.Subtract:
                        _parts.Add("-");
                        break;

                    default:
                        throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", binaryExpression.NodeType));
                }

                Visit(binaryExpression.Right);
                _parts.Add(")");
                //if (!_parts.All(x => x == ")"))
                //{
                //    _update.Add(_parts.Join(" "));
                //}
                //else { _update[_update.Count() - 1] = _update.Last()+ _parts[0]; }
                //_parts.Clear();
            }
            return binaryExpression;
        }

        protected override Expression VisitNew(NewExpression newExpression)
        {
            this.expressions.Add(newExpression);
            if (CurrentMethodCall == nameof(ExpressionToSQL.Select))
            {
                foreach (Expression arg in newExpression.Arguments)
                {
                    Visit(arg);
                    if (_parts.Count > 0)
                    {
                        if (_parts.Last() != _members[_member])
                        {
                            _parts.Add("AS [" + _members[_member] + "]");
                        }
                        _select.Add(_parts.Join(" "));
                        _parts.Clear();
                        _member++;
                    }
                }
            }
            if (CurrentMethodCall == nameof(ExpressionToSQL.Update))
            {
                foreach (Expression arg in newExpression.Arguments)
                {
                    Visit(arg);
                    if (_parts.Count > 0)
                    {
                        if (_parts.Last() != _members[_member])
                        {
                            _parts.Add($"[{_members[_member]}] = ");
                        }
                        _update.Add(_parts.Join(" "));
                        _parts.Clear();
                        _member++;
                    }
                }
            }
            return newExpression;
        }

        protected override Expression VisitConstant(ConstantExpression constantExpression)
        {
            this.expressions.Add(constantExpression);
            if (CurrentMethodCall != nameof(ExpressionToSQL.Update) && CurrentMethodCall != nameof(ExpressionToSQL.Select) && CurrentMethodCall != nameof(ExpressionToSQL.Bottom))
                switch (constantExpression.Value)
                {
                    case null when constantExpression.Value == null:
                        _parts.Add("NULL");
                        break;

                    default:

                        if (constantExpression.Type.CanConvertToSqlDbType() && _parts.Count(x => x.Contains("(")) > _parts.Count(x => x.Contains(")")))
                        {
                            _parts.Add(CreateParameter(constantExpression.Value).ParameterName);
                        }

                        break;
                }
            if (CurrentMethodCall == nameof(ExpressionToSQL.Select) || CurrentMethodCall == nameof(ExpressionToSQL.Update))
                _parts.Add(CreateParameter(constantExpression.Value).ParameterName);
            if (CurrentMethodCall == nameof(ExpressionToSQL.Bottom) && constantExpression.Value != null && (int)constantExpression.Value > 0)
                _last = (int)constantExpression.Value;
            return constantExpression;
        }

        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            this.expressions.Add(memberExpression);
            Expression VisitMemberLocal(Expression expression)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Parameter:
                        switch (CurrentMethodCall)
                        {
                            case nameof(Queryable.Where):
                            case nameof(ExpressionToSQL.All):
                            case nameof(ExpressionToSQL.Any):
                                _parts.Add($"[{memberExpression.Member.Name}]");
                                break;
                            case nameof(ExpressionToSQL.Select):
                            case nameof(ExpressionToSQL.Update):
                                _parts.Add($"[{memberExpression.Member.Name}]");
                                break;
                            default:
                                break;
                        }
                        return memberExpression;

                    case ExpressionType.Constant:
                        _parts.Add(CreateParameter(GetValue(memberExpression)).ParameterName);
                        return memberExpression;
                    case ExpressionType.MemberAccess:
                        MemberExpression exp = memberExpression.Expression as MemberExpression;
                        _parts.Add($"[{exp.Member.Name}].[{memberExpression.Member.Name}]");
                        return memberExpression;
                }

                throw new NotSupportedException(string.Format("The member '{0}' is not supported", memberExpression.Member.Name));
            }

            if (memberExpression.Expression == null)
            {
                return VisitMemberLocal(memberExpression);
            }

            return VisitMemberLocal(memberExpression.Expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            this.expressions.Add(methodCallExpression);
            CurrentMethodCall = methodCallExpression.Method.Name;
            LambdaExpression lambda;
            switch (methodCallExpression.Method.Name)
            {
                case nameof(ExpressionToSQL.All):
                    _all = true;
                    lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[0]);
                    Visit(lambda.Body);
                    _where.Add(_parts.Join(" "));
                    _parts.Clear();

                    foreach (Expression arg in methodCallExpression.Arguments)
                    {
                        MethodCallExpression expression = arg as MethodCallExpression;
                        if (expression == null) { continue; }
                        CurrentMethodCall = expression.Method.Name;
                        if (arg.NodeType == ExpressionType.Call) Visit(arg); break;
                    }

                    return methodCallExpression;
                case nameof(ExpressionToSQL.ExistsAny):
                    _any = true;
                    return methodCallExpression;
                case nameof(ExpressionToSQL.Any):
                    _any = true;
                    lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[0]);
                    Visit(lambda.Body);
                    _where.Add(_parts.Join(" "));
                    _parts.Clear();

                    foreach (Expression arg in methodCallExpression.Arguments)
                    {
                        MethodCallExpression expression = arg as MethodCallExpression;
                        if (expression == null) { continue; }
                        CurrentMethodCall = expression.Method.Name;
                        if (arg.NodeType == ExpressionType.Call) Visit(arg); break;
                    }

                    return methodCallExpression;
                case nameof(Queryable.Where) when methodCallExpression.Method.DeclaringType == typeof(Queryable):

                    lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);
                    Visit(lambda.Body);
                    _where.Add(_parts.Join(" "));
                    _parts.Clear();

                    foreach (Expression arg in methodCallExpression.Arguments)
                    {
                        MethodCallExpression expression = arg as MethodCallExpression;
                        if (expression == null) { continue; }
                        CurrentMethodCall = expression.Method.Name;
                        if (arg.NodeType == ExpressionType.Call) Visit(arg); break;
                    }

                    return methodCallExpression;

                case nameof(Queryable.SelectMany):
                    return ParseExpression(methodCallExpression, _selectMany);

                case nameof(Queryable.GroupBy):
                    return ParseExpression(methodCallExpression, _groupBy);

                case nameof(Queryable.Reverse):
                    return methodCallExpression;

                case nameof(Queryable.Take):
                    if (methodCallExpression.ToString().Contains("Reverse"))
                    {
                        return ParseExpression(methodCallExpression, ref _last);
                    }
                    else
                    {
                        return ParseExpression(methodCallExpression, ref _take);
                    }

                case nameof(ExpressionToSQL.Bottom):
                    Visit(methodCallExpression.Arguments[0]);
                    return methodCallExpression;

                case nameof(Queryable.Skip):
                    return ParseExpression(methodCallExpression, ref _skip);

                case nameof(Queryable.OrderBy):
                case nameof(Queryable.ThenBy):
                    return ParseExpression(methodCallExpression, _orderBy, "ASC");

                case nameof(Queryable.OrderByDescending):
                case nameof(Queryable.ThenByDescending):
                    return ParseExpression(methodCallExpression, _orderBy, "DESC");

                case nameof(Queryable.Distinct):
                    IsDistinct = true;
                    return Visit(methodCallExpression.Arguments[0]);

                case nameof(string.StartsWith):
                    _parts.AddRange(ParseExpression(methodCallExpression, methodCallExpression.Object));
                    _parts.Add("LIKE");
                    _parts.Add(CreateParameter(GetValue(methodCallExpression.Arguments[0]).ToString() + "%").ParameterName);
                    return methodCallExpression.Arguments[0];

                case nameof(string.EndsWith):
                    _parts.AddRange(ParseExpression(methodCallExpression, methodCallExpression.Object));
                    _parts.Add("LIKE");
                    _parts.Add(CreateParameter("%" + GetValue(methodCallExpression.Arguments[0]).ToString()).ParameterName);
                    return methodCallExpression.Arguments[0];

                case nameof(string.Contains):
                    _parts.AddRange(ParseExpression(methodCallExpression, methodCallExpression.Object));
                    _parts.Add("LIKE");
                    _parts.Add(CreateParameter("%" + GetValue(methodCallExpression.Arguments[0]).ToString() + "%").ParameterName);
                    return methodCallExpression.Arguments[0];

                case nameof(ExpressionToSQL.Update):
                    lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[0]);
                    MemberInitExpression memberInit = lambda.Body as MemberInitExpression;
                    _members = lambda.Body.ToString().Split("{")[1].Split("}")[0].Split(", ").ToList<string>().Select(x => x.Split(" = ")[0]).ToArray();
                    for (var m = 0; m < memberInit.Bindings.Count(); m++)
                    {
                        _parts.Add($"[{_members[m]}] =");
                        VisitMemberBinding(memberInit.Bindings[m]);
                        _update.Add(_parts.Join(" "));
                        _parts.Clear();
                    }
                    return methodCallExpression;

                case nameof(ExpressionToSQL.Select):
                    lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[0]);
                    if (lambda.Body.Type.Name.Contains("AnonymousType"))
                    {
                        _members = lambda.Body.Type.GetProperties().Select(x => x.Name).ToArray<string>();
                    }
                    else
                    {
                        _members = lambda.Body.ToString().Split("{")[1].Split("}")[0].Split(", ").ToList<string>().Select(x => x.Split(" = ")[0]).ToArray();
                    }
                    Visit(lambda.Body);
                    return methodCallExpression;

                case nameof(Queryable.Join):
                    //left table keys
                    foreach (Expression arg in methodCallExpression.Arguments)
                    {
                        MethodCallExpression expression = arg as MethodCallExpression;
                        if (expression == null) { continue; }
                        CurrentMethodCall = expression.Method.Name;
                        if (arg.NodeType == ExpressionType.Call) Visit(arg); break;
                    }
                    LambdaExpression leftKeys = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[2]);
                    LambdaExpression joinKeys = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[3]);
                    lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[4]);
                    if (firstJoin)
                    {
                        TableName = $"[{lambda.Parameters[0].Type.GetTableName()}]";
                    }
                    if (leftKeys.Body.NodeType == ExpressionType.MemberAccess)
                    {
                        MemberExpression leftKey = leftKeys.Body as MemberExpression;
                        MemberExpression joinKey = joinKeys.Body as MemberExpression;
                        join_tables.Add(leftKey.Expression.Type.GetTableName());
                        join_tables.Add(joinKey.Expression.Type.GetTableName());
                        _parts.Add($"[{joinKey.Expression.Type.GetTableName()}] ON");
                        _parts.Add($"[{leftKey.Expression.Type.GetTableName()}].[{leftKey.Member.Name}] = [{joinKey.Expression.Type.GetTableName()}].[{joinKey.Member.Name}]");
                    }
                    else
                    {
                        NewExpression leftMembers = leftKeys.Body as NewExpression;
                        NewExpression joinMembers = joinKeys.Body as NewExpression;
                        for (int i = 0; i < leftMembers.Arguments.Count; i++)
                        {
                            if (i > 0) { _parts.Add("AND"); }
                            MemberExpression leftKey = leftMembers.Arguments[i] as MemberExpression;
                            MemberExpression joinKey = joinMembers.Arguments[i] as MemberExpression;
                            _parts.Add($"[{lambda.Parameters[0].Type.GetTableName()}].[{leftKey.Member.Name}] = [{lambda.Parameters[1].Type.GetTableName()}].[{joinKey.Member.Name}]");
                        }
                    }
                    _join.Add($"{_joinType} {_parts.Join(" ")}");
                    _parts.Clear();
                    _select.Clear();
                    List<Expression> expressions = new List<Expression>();
                    switch (lambda.Body.NodeType)
                    {
                        case ExpressionType.Parameter:
                            expressions.Add(lambda.Body);
                            break;
                        case ExpressionType.New:
                            NewExpression newExpression = lambda.Body as NewExpression;
                            expressions = newExpression.Arguments.ToList();
                            for (int i = 0; i < expressions.Count; i++)
                            {
                                var currentMember = expressions[i];
                                if (currentMember.Type.Name.Contains("AnonymousType"))
                                {
                                    List<Type> table_types = new List<Type>();
                                    table_types.AddRange(GetDeepProperties(currentMember.Type));
                                    table_types.ForEach(x => _select.Add($"[{x.GetTableName()}].*"));
                                }
                                else
                                {
                                    if (join_tables.Contains(currentMember.Type.GetTableName()) && currentMember.NodeType == ExpressionType.Parameter)
                                    {
                                        _select.Add($"[{currentMember.Type.GetTableName()}].*");
                                    }
                                    else
                                    {
                                        MemberExpression memberExpression = currentMember as MemberExpression;
                                        _select.Add($"[{memberExpression.Expression.Type.GetTableName()}].[{memberExpression.Member.Name}] AS {newExpression.Members[i].Name}");
                                    }
                                }
                            }
                            break;
                    }
                    firstJoin = false;
                    return methodCallExpression;

                case nameof(ExpressionToSQL.Delete):
                    IsDelete = true;
                    return methodCallExpression;
                default:
                    if (methodCallExpression.Object != null)
                    {
                        _parts.Add(CreateParameter(GetValue(methodCallExpression)).ParameterName);
                        return methodCallExpression;
                    }
                    break;
            }

            throw new NotSupportedException($"The method '{methodCallExpression.Method.Name}' is not supported");
        }

        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            this.expressions.Add(unaryExpression);
            switch (unaryExpression.NodeType)
            {
                case ExpressionType.Not:
                    _parts.Add("NOT");
                    Visit(unaryExpression.Operand);
                    break;

                case ExpressionType.Convert:
                    Visit(unaryExpression.Operand);
                    break;
                case ExpressionType.Quote:
                    Visit(unaryExpression.Operand);
                    break;

                default:
                    throw new NotSupportedException($"The unary operator '{unaryExpression.NodeType}' is not supported");
            }
            return unaryExpression;
        }

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression;
        }

        [SuppressMessage("Style", "IDE0011:Add braces", Justification = "Easier to read")]
        private IEnumerable<string> BuildDeclaration()
        {
            if (Parameters.Length == 0)                        /**/    yield break;
            foreach (SqlParameter parameter in Parameters)     /**/    yield return $"DECLARE {parameter.ParameterName} {parameter.SqlDbType}";

            foreach (SqlParameter parameter in Parameters)     /**/
                if (parameter.SqlDbType.RequiresQuotes())      /**/    yield return $"SET {parameter.ParameterName} = '{parameter.SqlValue?.ToString().Replace("'", "''") ?? "NULL"}'";
                else                                           /**/    yield return $"SET {parameter.ParameterName} = {parameter.SqlValue}";
        }

        [SuppressMessage("Style", "IDE0011:Add braces", Justification = "Easier to read")]
        private IEnumerable<string> BuildOrderByStatement()
        {
            if (Skip.HasValue && _orderBy.Count == 0)                       /**/   yield return "ORDER BY (SELECT NULL)";
            else if (_orderBy.Count == 0)                                   /**/   yield break;
            else if (_groupBy.Count > 0 && _orderBy[0].StartsWith("[Key]")) /**/   yield return "ORDER BY " + _groupBy.Distinct().ToList().Join(", ");
            else                                                            /**/   yield return "ORDER BY " + _orderBy.Distinct().ToList().Join(", ");

            if (Skip.HasValue && Take.HasValue)                             /**/   yield return $"OFFSET {Skip} ROWS FETCH NEXT {Take} ROWS ONLY";
            else if (Skip.HasValue && !Take.HasValue)                       /**/   yield return $"OFFSET {Skip} ROWS";
        }

        [SuppressMessage("Style", "IDE0011:Add braces", Justification = "Easier to read")]
        private IEnumerable<string> BuildSelectStatement()
        {
            yield return "SELECT";

            if (IsDistinct)                                 /**/    yield return "DISTINCT";

            if (Take.HasValue && !Skip.HasValue)            /**/    yield return $"TOP ({Take.Value})";
            if (Last.HasValue)                              /**/    yield return $"BOTTOM ({Last.Value})";

            if (_select.Count == 0 && _groupBy.Count > 0)   /**/    yield return _groupBy.Distinct().ToList().Select(x => $"MAX({x})").Join(", ");
            else if (_select.Count == 0)                    /**/    yield return "*";
            else                                            /**/    yield return _select.Distinct().ToList().Join(", ");
        }

        [SuppressMessage("Style", "IDE0011:Add braces", Justification = "Easier to read")]
        private IEnumerable<string> BuildDeleteStatement()
        {
            yield return "DELETE";
            if (Take.HasValue && !Skip.HasValue)            /**/    yield return $"TOP ({Take.Value})";
            else                                            /**/    yield return _select.Join(", ");
        }

        [SuppressMessage("Style", "IDE0011:Add braces", Justification = "Easier to read")]
        private IEnumerable<string> BuildSqlStatement()
        {
            if (IsDelete)                   /**/   yield return "DELETE";
            else if (_update.Count > 0)     /**/   yield return $"UPDATE {TableName}";
            else if (_all)                  /**/   yield return $"SELECT NOT EXISTS ({Environment.NewLine} {SelectStr}";
            else if (_any)                  /**/   yield return $"SELECT EXISTS ({Environment.NewLine} {SelectStr}";
            else                            /**/   yield return SelectStr;

            if (_update.Count == 0)         /**/   yield return From;
            else if (_update.Count > 0)     /**/   yield return UpdateStr;

            if (Join != null)               /**/   yield return Join;
            if (_all || _any)               /**/   yield return $"{Where} {Environment.NewLine})";
            else if (Where != null)         /**/   yield return Where;

            if (GroupBy != null)            /**/   yield return GroupBy;
            if (OrderBy != null)            /**/   yield return OrderBy;
        }

        private SqlParameter CreateParameter(object value)
        {
            string parameterName = $"@p{_parameters.Count}";

            var parameter = new SqlParameter()
            {
                ParameterName = parameterName,
                Value = value
            };

            _parameters.Add(parameter);

            return parameter;
        }

        private IEnumerable<string> GetNewExpressionString(NewExpression newExpression, string appendString = null)
        {
            for (int i = 0; i < newExpression.Members.Count; i++)
            {
                if (newExpression.Arguments[i].NodeType == ExpressionType.MemberAccess)
                {
                    yield return
                        appendString == null ?
                        $"[{newExpression.Members[i].Name}]" :
                        $"[{newExpression.Members[i].Name}] {appendString}";
                }
                else
                {
                    yield return
                        appendString == null ?
                        $"[{newExpression.Members[i].Name}] = {CreateParameter(GetValue(newExpression.Arguments[i])).ParameterName}" :
                        $"[{newExpression.Members[i].Name}] = {CreateParameter(GetValue(newExpression.Arguments[i])).ParameterName}";
                }
            }
        }

        private object GetValue(Expression expression)
        {
            object GetMemberValue(MemberInfo memberInfo, object container = null)
            {
                switch (memberInfo)
                {
                    case FieldInfo fieldInfo:
                        return fieldInfo.GetValue(container);

                    case PropertyInfo propertyInfo when memberInfo.Name != propertyInfo.PropertyType.Name:
                        return null;

                    case PropertyInfo propertyInfo:
                        return propertyInfo.GetValue(container);

                    default: return null;
                }
            }

            switch (expression)
            {
                case ConstantExpression constantExpression:
                    return constantExpression.Value;

                case MemberExpression memberExpression when memberExpression.Expression is ConstantExpression constantExpression:
                    return GetMemberValue(memberExpression.Member, constantExpression.Value);

                case MemberExpression memberExpression when memberExpression.Expression is null: // static
                    return GetMemberValue(memberExpression.Member);

                case MethodCallExpression methodCallExpression:
                    return Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();

                case null:
                    return null;
                case MemberExpression memberExp when memberExp.Expression is MemberExpression memberExpression:
                    var i = GetMemberValue(memberExpression.Member);
                    //Visit(expression);
                    return null;
            }

            throw new NotSupportedException();
        }

        private object GetEntityType(Expression expression)
        {
            while (true)
            {
                switch (expression)
                {
                    case ConstantExpression constantExpression:
                        return constantExpression.Value;

                    case MethodCallExpression methodCallExpression:
                        expression = methodCallExpression.Arguments[0];
                        continue;

                    default:
                        return null;
                }
            }
        }

        private bool IsNullConstant(Expression expression) => expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == null;

        private IEnumerable<string> ParseExpression(Expression parent, Expression body, string appendString = null)
        {
            switch (body)
            {
                case MemberExpression memberExpression:
                    return appendString == null ?
                        new string[] { $"[{memberExpression.Member.Name}]" } :
                        new string[] { $"[{memberExpression.Member.Name}] {appendString}" };

                case NewExpression newExpression:
                    return GetNewExpressionString(newExpression, appendString);

                case ParameterExpression parameterExpression when parent is LambdaExpression lambdaExpression && lambdaExpression.ReturnType == parameterExpression.Type:
                    return new string[0];
                case ParameterExpression parameterExpression when body.ToString() == "x":
                    return new string[0];

                case ConstantExpression constantExpression:
                    return new string[0];

                    //return constantExpression
                    //    .Type
                    //    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    //    .Select(x => $"[{x.Name}] = {CreateParameter(x.GetValue(constantExpression.Value)).ParameterName}");
            }

            throw new NotSupportedException();
        }

        private Expression ParseExpression(MethodCallExpression expression, List<string> commandList, string appendString = null)
        {
            Visit(expression.Arguments[0]);
            UnaryExpression unary;
            string[] arg = new string[] { };
            switch (expression.Method.Name)
            {
                //case nameof(TQueryExtensions.PUpdate):
                //    unary = (UnaryExpression)expression.Arguments[0];
                //    var lambda = (LambdaExpression)StripQuotes(expression.Arguments[0]);
                //    if (unary.ToString().Contains("AnonymousType"))
                //    { arg = unary.ToString().Split("(")[1]?.Split(")")[0]?.Split(", "); } else{
                //        try { arg = unary.ToString().Split("{")[1]?.Split("}")[0]?.Split(", "); } catch(Exception e) { }
                //    }
                //    for (int i = 0; i< arg.Length; i++)
                //    {
                //        arg[i] = "[" + arg[i].Substring(0, arg[i].IndexOf(" ")) + "]" + arg[i].Substring(arg[i].IndexOf(" ="));
                //        if (arg[i].IndexOf("x.")> 0) {
                //            arg[i] = arg[i].IndexOf("(") > 0 ? arg[i].Insert(arg[i].Split("x.")[0].Length + arg[i].Split("x.")[1].IndexOf(" ") + 2, "]") : arg[i] + "]";
                //            arg[i] = arg[i].Replace("x.", "["); 
                //        }
                //    }
                //    _update.AddRange(arg);
                //    return Visit(expression.Arguments[0]);

                case nameof(Queryable.Join):
                    unary = (UnaryExpression)expression.Arguments[4];
                    break;
                default:
                    unary = (UnaryExpression)expression.Arguments[1];
                    break;
            }
            var lambdaExpression = (LambdaExpression)unary.Operand;

            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            commandList.AddRange(ParseExpression(lambdaExpression, lambdaExpression.Body, appendString));

            return Visit(expression.Arguments[0]);
        }

        private Expression ParseExpression(MethodCallExpression expression, ref int? size)
        {
            var sizeExpression = (ConstantExpression)expression.Arguments[1];

            if (int.TryParse(sizeExpression.Value.ToString(), out int value))
            {
                size = value;
                return Visit(expression.Arguments[0]);
            }

            throw new NotSupportedException();
        }

        IEnumerable<Type> GetDeepProperties(Type type)
        {
            var types = type.GetProperties().Select(x => x.PropertyType).ToList();
            foreach (Type t in types)
            {
                if (join_tables.Contains(t.GetTableName()))
                {
                    yield return t;
                }
                else
                {
                    foreach (Type _t in GetDeepProperties(t))
                        yield return _t;
                }
            }
        }


        //more visitors, just for test

        protected override Expression VisitBlock(BlockExpression node)
        {
            this.expressions.Add(node);
            return base.VisitBlock(node);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            this.expressions.Add(node);
            return base.VisitConditional(node);
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            this.expressions.Add(node);
            return base.VisitDebugInfo(node);
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            this.expressions.Add(node);
            return base.VisitDefault(node);
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            this.expressions.Add(node);
            return base.VisitDynamic(node);
        }

        protected override Expression VisitExtension(Expression node)
        {
            this.expressions.Add(node);
            return base.VisitExtension(node);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            this.expressions.Add(node);
            return base.VisitGoto(node);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            this.expressions.Add(node);
            return base.VisitIndex(node);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            this.expressions.Add(node);
            return base.VisitInvocation(node);
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            this.expressions.Add(node);
            return base.VisitLabel(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            this.expressions.Add(node);
            return base.VisitLambda(node);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            this.expressions.Add(node);
            return base.VisitListInit(node);
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            this.expressions.Add(node);
            return base.VisitLoop(node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            this.expressions.Add(node);
            return base.VisitMemberInit(node);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            this.expressions.Add(node);
            return base.VisitNewArray(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            this.expressions.Add(node);
            return base.VisitParameter(node);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            this.expressions.Add(node);
            return base.VisitRuntimeVariables(node);
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            this.expressions.Add(node);
            return base.VisitSwitch(node);
        }

        protected override Expression VisitTry(TryExpression node)
        {
            this.expressions.Add(node);
            return base.VisitTry(node);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            this.expressions.Add(node);
            return base.VisitTypeBinary(node);
        }

        public IEnumerable<Expression> Explore(Expression node)
        {
            this.expressions.Clear();
            this.Visit(node);
            return expressions.ToArray();
        }
        private readonly List<Expression> expressions = new List<Expression>();

        //internal static Expression PUpdate<T>(Expression<Func<T, T>> selector )
        //{
        //    var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Update), BindingFlags.NonPublic | BindingFlags.Static);
        //    MethodInfo methodInfoGeneric = methodInfo.MakeGenericMethod(typeof(T));
        //    MethodCallExpression methodCallExpression = Expression.Call(methodInfoGeneric,selector);
        //    return Expression.Lambda(methodCallExpression);
        //}

        //internal static Expression PSelect<T, TResult>(Expression<Func<T, TResult>> expression)
        //{
        //    var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Select), BindingFlags.NonPublic | BindingFlags.Static);
        //    MethodInfo methodInfoGeneric = methodInfo.MakeGenericMethod(typeof(T), typeof(TResult));
        //    MethodCallExpression methodCallExpression = Expression.Call(methodInfoGeneric, expression);
        //    return Expression.Lambda(methodCallExpression);
        //}

        internal static Expression Update<T>(Expression<Func<T, T>> selector)
        {
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Update), BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo methodInfoGeneric = methodInfo.MakeGenericMethod(typeof(T));
            MethodCallExpression methodCallExpression = Expression.Call(methodInfoGeneric, selector);
            return Expression.Lambda(methodCallExpression);
        }

        internal static Expression Select<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Select), BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo methodInfoGeneric = methodInfo.MakeGenericMethod(typeof(T), typeof(TResult));
            MethodCallExpression methodCallExpression = Expression.Call(methodInfoGeneric, expression);
            return Expression.Lambda(methodCallExpression);
        }

        internal static Expression Bottom(int predicate)
        {
            var expression = Expression.Constant(predicate);
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Bottom), BindingFlags.NonPublic | BindingFlags.Static);
            MethodCallExpression methodCallExpression = Expression.Call(methodInfo, expression);
            return Expression.Lambda(methodCallExpression);
        }

        internal static Expression RecCount(int predicate)
        {
            var expression = Expression.Constant(predicate);
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.RecCount), BindingFlags.NonPublic | BindingFlags.Static);
            MethodCallExpression methodCallExpression = Expression.Call(methodInfo, expression);
            return Expression.Lambda(methodCallExpression);
        }

        internal static Expression Count<T, TKey>(Expression<Func<T, TKey>> predicate)
        {
            var expression = Expression.Constant(predicate);
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Count), BindingFlags.NonPublic | BindingFlags.Static);
            MethodCallExpression methodCallExpression = Expression.Call(methodInfo, expression);
            return Expression.Lambda(methodCallExpression);
        }

        internal static Expression Sum<T, TKey>(Expression<Func<T, TKey>> predicate)
        {
            var expression = Expression.Constant(predicate);
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Sum), BindingFlags.NonPublic | BindingFlags.Static);
            MethodCallExpression methodCallExpression = Expression.Call(methodInfo, expression);
            return Expression.Lambda(methodCallExpression);
        }

        internal static Expression Max<T, TKey>(Expression<Func<T, TKey>> predicate)
        {
            var expression = Expression.Constant(predicate);
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Max), BindingFlags.NonPublic | BindingFlags.Static);
            MethodCallExpression methodCallExpression = Expression.Call(methodInfo, expression);
            return Expression.Lambda(methodCallExpression);
        }
        internal static Expression Min<T, TKey>(Expression<Func<T, TKey>> predicate)
        {
            var expression = Expression.Constant(predicate);
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Min), BindingFlags.NonPublic | BindingFlags.Static);
            MethodCallExpression methodCallExpression = Expression.Call(methodInfo, expression);
            return Expression.Lambda(methodCallExpression);
        }
        internal static Expression Average<T, TKey>(Expression<Func<T, TKey>> predicate)
        {
            var expression = Expression.Constant(predicate);
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Average), BindingFlags.NonPublic | BindingFlags.Static);
            MethodCallExpression methodCallExpression = Expression.Call(methodInfo, expression);
            return Expression.Lambda(methodCallExpression);
        }

        internal static Expression All<T>(Expression<Func<T, bool>> predicate)
        {
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.All), BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo methodInfoGeneric = methodInfo.MakeGenericMethod(typeof(T));
            MethodCallExpression methodCallExpression = Expression.Call(methodInfoGeneric, predicate);
            return Expression.Lambda(methodCallExpression);
        }

        internal static Expression Any<T>(Expression<Func<T, bool>> predicate)
        {
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Any), BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo methodInfoGeneric = methodInfo.MakeGenericMethod(typeof(T));
            MethodCallExpression methodCallExpression = Expression.Call(methodInfoGeneric, predicate);
            return Expression.Lambda(methodCallExpression);
        }

        internal static Expression ExistsAny(int predicate)
        {
            var expression = Expression.Constant(predicate);
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.ExistsAny), BindingFlags.NonPublic | BindingFlags.Static);
            MethodCallExpression methodCallExpression = Expression.Call(methodInfo, expression);
            return Expression.Lambda(methodCallExpression);
        }
        internal static Expression Delete(int something)
        {
            var expression = Expression.Constant(something);
            var methodInfo = typeof(ExpressionToSQL).GetMethod(nameof(ExpressionToSQL.Delete), BindingFlags.NonPublic | BindingFlags.Static);
            MethodCallExpression methodCallExpression = Expression.Call(methodInfo, expression);
            return Expression.Lambda(methodCallExpression);
        }

    }
}
