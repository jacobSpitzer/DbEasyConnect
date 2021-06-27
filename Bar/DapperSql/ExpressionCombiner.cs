using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bar.DapperSql
{
    public class ExpressionCombiner
    {
        public static Expression<Func<TSource, TDestination>> Combine<TSource, TDestination>(
        params Expression<Func<TSource, TDestination>>[] selectors)
        {
            var param = Expression.Parameter(typeof(TSource), "x");
            return Expression.Lambda<Func<TSource, TDestination>>(
                Expression.MemberInit(
                    Expression.New(typeof(TDestination).GetConstructor(Type.EmptyTypes)),
                    from selector in selectors
                    let replace = new ParameterReplaceVisitor(
                          selector.Parameters[0], param)
                    from binding in ((MemberInitExpression)selector.Body).Bindings
                          .OfType<MemberAssignment>()
                    select Expression.Bind(binding.Member,
                          replace.VisitAndConvert(binding.Expression, "Combine")))
                , param);
        }

        public class ParameterReplaceVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression from, to;
            public ParameterReplaceVisitor(ParameterExpression from, ParameterExpression to)
            {
                this.from = from;
                this.to = to;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == from ? to : base.VisitParameter(node);
            }
        }
    }
}
