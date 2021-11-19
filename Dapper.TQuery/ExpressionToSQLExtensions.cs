using Microsoft.SqlServer.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using static Dapper.TQuery.TQueryExceptions;

namespace Dapper.TQuery
{
    internal static class ExpressionToSQLExtensions
    {

        public static bool CanConvertToSqlDbType(this Type type) => type.ToSqlDbTypeInternal().HasValue;

        internal static string Join(this IEnumerable<string> values, string separator) => string.Join(separator, values);

        internal static bool RequiresQuotes(this SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.Time:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Text:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.Timestamp:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                case SqlDbType.Variant:
                case SqlDbType.NVarChar:
                    return true;

                default:
                    return false;
            }
        }

        private static T GetValueOfField<T>(this object obj, string name)
        {
            FieldInfo field = obj
                .GetType()
                .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);

            return (T)field.GetValue(obj);
        }

        [SuppressMessage("Style", "IDE0011:Add braces", Justification = "Easier to read than with Allman braces")]
        public static SqlDbType? ToSqlDbTypeInternal(this Type type)
        {
            if (Nullable.GetUnderlyingType(type) is Type nullableType)
                return nullableType.ToSqlDbTypeInternal();

            if (type.IsEnum)
                return Enum.GetUnderlyingType(type).ToSqlDbTypeInternal();

            if (type == typeof(long))            /**/                return SqlDbType.BigInt;
            if (type == typeof(byte[]))          /**/                return SqlDbType.VarBinary;
            if (type == typeof(bool))            /**/                return SqlDbType.Bit;
            if (type == typeof(string))          /**/                return SqlDbType.NVarChar;
            if (type == typeof(DateTime))        /**/                return SqlDbType.DateTime2;
            if (type == typeof(decimal))         /**/                return SqlDbType.Decimal;
            if (type == typeof(double))          /**/                return SqlDbType.Float;
            if (type == typeof(int))             /**/                return SqlDbType.Int;
            if (type == typeof(float))           /**/                return SqlDbType.Real;
            if (type == typeof(Guid))            /**/                return SqlDbType.UniqueIdentifier;
            if (type == typeof(short))           /**/                return SqlDbType.SmallInt;
            if (type == typeof(object))          /**/                return SqlDbType.Variant;
            if (type == typeof(DateTimeOffset))  /**/                return SqlDbType.DateTimeOffset;
            if (type == typeof(TimeSpan))        /**/                return SqlDbType.Time;
            if (type == typeof(byte))            /**/                return SqlDbType.TinyInt;

            return null;
        }

        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsIntType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }

        internal enum SqlTypeCategory
        {
            String,
            Numeric,
            DateTime,
            Other
        }

        internal class SqlTypeConversionHolder
        {
            internal string SqlName { get; private set; }
            internal Type SqlType { get; private set; }
            internal Type DotNetType { get; private set; }
            internal SqlTypeCategory Category { get; set; }
            internal SqlTypeConversionHolder(string sqlName,Type sqlType,Type dotnetType, SqlTypeCategory category = SqlTypeCategory.Other)
            {
                SqlName = sqlName; sqlType = SqlType; DotNetType = dotnetType; Category = category;
            }
        }

        /// <summary>
        /// The map of types. THis maps all the corresponding types between sql server types, .net sql types, and .net types
        /// </summary>
        internal static List<SqlTypeConversionHolder> SqlServerToDotnetList = new List<SqlTypeConversionHolder>()
        {
            new SqlTypeConversionHolder("bigint", typeof(SqlInt64),typeof(Int64), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("binary", typeof(SqlBytes),typeof(Byte[]), SqlTypeCategory.String),
            new SqlTypeConversionHolder("bit", typeof(SqlBoolean),typeof(Boolean), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("char", typeof(SqlChars),typeof(char), SqlTypeCategory.String), //this one may need work
            new SqlTypeConversionHolder("cursor", null,null),
            new SqlTypeConversionHolder("date", typeof(SqlDateTime),typeof(DateTime), SqlTypeCategory.DateTime),
            new SqlTypeConversionHolder("datetime2", null,typeof(DateTime), SqlTypeCategory.DateTime),
            new SqlTypeConversionHolder("datetime", typeof(SqlDateTime),typeof(DateTime), SqlTypeCategory.DateTime),
            new SqlTypeConversionHolder("DATETIMEOFFSET", null,typeof(DateTimeOffset), SqlTypeCategory.DateTime),
            new SqlTypeConversionHolder("decimal", typeof(SqlDecimal),typeof(Decimal), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("float", typeof(SqlDouble),typeof(Double), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("geography", typeof(SqlGeography),null),
            new SqlTypeConversionHolder("geometry", typeof(SqlGeometry),null),
            new SqlTypeConversionHolder("hierarchyid", typeof(SqlHierarchyId),null),
            new SqlTypeConversionHolder("image", null,null),
            new SqlTypeConversionHolder("int", typeof(SqlInt32),typeof(Int32), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("money", typeof(SqlMoney),typeof(Decimal), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("nchar", typeof(SqlChars),typeof(String), SqlTypeCategory.String),
            new SqlTypeConversionHolder("ntext", null,null),
            new SqlTypeConversionHolder("numeric", typeof(SqlDecimal),typeof(Decimal), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("nvarchar", typeof(SqlChars),typeof(String), SqlTypeCategory.String),
            new SqlTypeConversionHolder("nvarchar(max)", typeof(SqlChars),typeof(String), SqlTypeCategory.String),
            new SqlTypeConversionHolder("nvarchar(1)", typeof(SqlChars),typeof(Char), SqlTypeCategory.String),
            new SqlTypeConversionHolder("nchar(1)", typeof(SqlChars),typeof(Char), SqlTypeCategory.String),
            new SqlTypeConversionHolder("real", typeof(SqlSingle),typeof(Single), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("rowversion", null,typeof(Byte[]), SqlTypeCategory.String),
            new SqlTypeConversionHolder("smallint", typeof(SqlInt16),typeof(Int16), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("smallmoney", typeof(SqlMoney),typeof(Decimal), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("sql_variant", null,typeof(Object)),
            new SqlTypeConversionHolder("table", null,null),
            new SqlTypeConversionHolder("text", null,null), //this one may need work
            new SqlTypeConversionHolder("time", null,typeof(TimeSpan), SqlTypeCategory.DateTime),
            new SqlTypeConversionHolder("timestamp", null,null, SqlTypeCategory.DateTime),
            new SqlTypeConversionHolder("tinyint", typeof(SqlByte),typeof(Byte), SqlTypeCategory.Numeric),
            new SqlTypeConversionHolder("uniqueidentifier", typeof(SqlGuid),typeof(Guid)),
            new SqlTypeConversionHolder("varbinary", typeof(SqlBytes),typeof(Byte[]), SqlTypeCategory.String),
            new SqlTypeConversionHolder("varbinary(max)", typeof(SqlBytes),typeof(Byte[]), SqlTypeCategory.String),
            new SqlTypeConversionHolder("varbinary(1)", typeof(SqlBytes),typeof(byte), SqlTypeCategory.String),
            new SqlTypeConversionHolder("binary(1)", typeof(SqlBytes),typeof(byte), SqlTypeCategory.String),
            new SqlTypeConversionHolder("varchar", typeof(SqlString),typeof(string), SqlTypeCategory.String), //this one may need work
            new SqlTypeConversionHolder("varchar(max)", typeof(SqlString),typeof(string), SqlTypeCategory.String), //this one may need work
            new SqlTypeConversionHolder("xml", typeof(SqlXml),typeof(string), SqlTypeCategory.String)
        };


        /// <summary>
        /// The map of types. THis maps all the corresponding types between sql server types, .net sql types, and .net types
        /// </summary>
        internal static List<SqlTypeConversionHolder> dotnetToSqlServerList = new List<SqlTypeConversionHolder>()
        {
            //Binary datatypes - default is 2 Mb.
            new SqlTypeConversionHolder("binary(2000)", typeof(SqlBytes),typeof(Byte[])), // same as typeof(byte[])
            
            //numeric
            new SqlTypeConversionHolder("real", typeof(SqlSingle),typeof(Single)), // same as typeof(float)
            new SqlTypeConversionHolder("float", typeof(SqlDouble),typeof(Double)), // same as typeof(double)
            new SqlTypeConversionHolder("bigint", typeof(SqlInt64),typeof(Int64)), // same as typeof(long)
            new SqlTypeConversionHolder("bigint", typeof(SqlInt32),typeof(UInt32)),
            new SqlTypeConversionHolder("int", typeof(SqlInt32),typeof(Int32)), // same as typeof(int)
            new SqlTypeConversionHolder("int", typeof(SqlInt32),typeof(UInt16)),
            new SqlTypeConversionHolder("smallint", typeof(SqlInt16),typeof(Int16)),
            new SqlTypeConversionHolder("smallint", typeof(SqlInt16),typeof(SByte)),
            new SqlTypeConversionHolder("tinyint", typeof(SqlByte),typeof(Byte)), // same as typeof(byte)
            new SqlTypeConversionHolder("bit", typeof(SqlBoolean),typeof(Boolean)), // same as typeof(bool)
            new SqlTypeConversionHolder("decimal(20,0)", typeof(SqlDecimal),typeof(UInt64)),
            new SqlTypeConversionHolder("decimal(18,2)", typeof(SqlDecimal),typeof(Decimal)), //default is decimal(18,2) based on EF
            //new SqlTypeConversionHolder("money", typeof(SqlMoney),typeof(Decimal)),
            //new SqlTypeConversionHolder("numeric", typeof(SqlDecimal),typeof(Decimal)),
            //new SqlTypeConversionHolder("smallmoney", typeof(SqlMoney),typeof(Decimal)),

            //strings & char
            new SqlTypeConversionHolder("char", typeof(SqlChars),typeof(char)), //this one may need work
            new SqlTypeConversionHolder("nvarchar(255)", typeof(SqlChars),typeof(String)),
            
            //date & time
            new SqlTypeConversionHolder("datetime2", null,typeof(DateTime)),
            new SqlTypeConversionHolder("DATETIMEOFFSET", null,typeof(DateTimeOffset)),
            new SqlTypeConversionHolder("time", null,typeof(TimeSpan)),

            //other
            // ???? new SqlTypeConversionHolder("sql_variant", null,typeof(Object)),
            new SqlTypeConversionHolder("uniqueidentifier", typeof(SqlGuid),typeof(Guid)),

            //list & array
            //new SqlTypeConversionHolder("List, Array, Collection are not supported types.",null,typeof(IEnumerable)),
        };

        internal class SqlTypeWithPrecision
        {
            internal string SqlName { get; private set; }
            internal int SqlDefault { get; set; }
            internal int CustomDefault { get; set; }
            internal int Max { get; set; }
            internal bool MaxStr { get; set; }

            internal SqlTypeWithPrecision(string sqlName, int sqlDefault, int cDefault, int max, bool maxStr = false)
            {
                SqlName = sqlName; SqlDefault = sqlDefault; Max = max; MaxStr = maxStr; CustomDefault = cDefault;
            }
        }

        /// <summary>
        /// The map of types. THis maps all the corresponding types between sql server types, .net sql types, and .net types
        /// </summary>
        internal static List<SqlTypeWithPrecision> SqlServerWithPrecisionList = new List<SqlTypeWithPrecision>()
        {
            new SqlTypeWithPrecision("binary",1,2000,8000),
            new SqlTypeWithPrecision("char",1,1,8000),
            new SqlTypeWithPrecision("datetime2",7,7,7),
            new SqlTypeWithPrecision("datetimeoffset",7,7,7),
            new SqlTypeWithPrecision("decimal",18,18,38),
            new SqlTypeWithPrecision("float",53,53,53),
            new SqlTypeWithPrecision("nchar",1,1,4000),
            new SqlTypeWithPrecision("numeric",18,18,38),
            new SqlTypeWithPrecision("nvarchar",1,255,4000,true),
            new SqlTypeWithPrecision("time",7,7,7),
            new SqlTypeWithPrecision("varbinary",1,2000,8000,true),
            new SqlTypeWithPrecision("varchar",1,255,8000,true)
        };

        internal static Type GetDNetType(string sqlString)
        {
            sqlString = sqlString.ToLower();
            if (sqlString.EndsWith(")") && !sqlString.Contains("(max)") && !sqlString.Contains("(1)"))
                sqlString = sqlString.Split("(")[0];
            return SqlServerToDotnetList.FirstOrDefault(x => x.SqlName == sqlString)?.DotNetType;
        }

        internal static string GetSqlServerType(this Type dotnetType)
        {
            
            return dotnetToSqlServerList.FirstOrDefault(x => x.DotNetType == dotnetType)?.SqlName;
        }

        internal static bool CanConvertToSqlServerType(Type dotnetType)
        {
            return dotnetType.GetSqlServerType().Any();
        }

        internal static string MatchBestSqlDataType(this PropertyInfo field)
        {
            Type fieldType = field.GetType();
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                fieldType = Nullable.GetUnderlyingType(field.GetType());
            //check if valid/supported type
            if (!fieldType.CanConvertToSqlDbType()) throw new NotSupportedFieldTypeException(field);
            //if is declared datatype, validate, get length if supported & declared
            ColumnAttribute columnAttr = field.GetCustomAttribute<ColumnAttribute>();
            int datatypeLen = 0;
            bool isMax = false;
            string bestMatch;
            //1: get datatype attribute, if any
            if (columnAttr != null && !String.IsNullOrEmpty(columnAttr.TypeName))
            {
                if (GetDNetType(columnAttr.TypeName) == null) throw new NotSupportedFieldTypeException(field);
                if (GetDNetType(columnAttr.TypeName) != GetDNetType(fieldType.GetSqlServerType()))
                    throw new InvalidCastException($"Declared type '{columnAttr.TypeName}' in [Column(TypeName = \"\")] attribute is invalid "
                            + $"on field '{field.Name}' on class '{field.DeclaringType.Name}', which is a '{fieldType}' type");
                var isPrecisionField = SqlServerWithPrecisionList.Any(sqlType => sqlType.SqlName == GetSqlServerType(GetDNetType(columnAttr.TypeName)));
                if (isPrecisionField && columnAttr.TypeName.Contains("(") && !columnAttr.TypeName.ToLower().Contains("(max)"))
                    datatypeLen = int.Parse(columnAttr.TypeName.Split("(")[1].Split(")")[0]);
                else if (isPrecisionField && columnAttr.TypeName.ToLower().Contains("(max)")) { isMax = true; }
                bestMatch = columnAttr.TypeName.Split('(')[0];
            }
            else bestMatch = GetSqlServerType(fieldType).Split('(')[0];
            if (SqlServerWithPrecisionList.Any(sqlType => sqlType.SqlName == bestMatch))
            {
                //2: get maxLength, and/or stringLength
                var typeWithPrecision = SqlServerWithPrecisionList.FirstOrDefault(type => type.SqlName == bestMatch);
                var maxLen = field.IsDefined(typeof(MaxLengthAttribute)) ? field.GetCustomAttribute<MaxLengthAttribute>()?.Length : 0;
                var strLen = field.IsDefined(typeof(StringLengthAttribute)) ? field.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength : 0;
                //3: get the minimum from all length attributes, skip zero.
                var lenVars = new int[] { datatypeLen, (int)maxLen, (int)strLen };
                var finalLen = lenVars.Where(x => x > 0).Any() ? lenVars.Where(x => x > 0).Min() : typeWithPrecision.CustomDefault;
                if (isMax && typeWithPrecision.MaxStr) { return columnAttr.TypeName; }
                else if (isMax) { return $"{bestMatch}({typeWithPrecision.Max})"; }
                else if (finalLen <= typeWithPrecision.Max) { return $"{bestMatch}({finalLen})"; }
                else if (finalLen > typeWithPrecision.Max) { return $"{bestMatch}({typeWithPrecision.Max})"; }
            }
            return bestMatch;
        }

        public static Type GetClrType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(long?);

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);

                case SqlDbType.Bit:
                    return typeof(bool?);

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return typeof(string);

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return typeof(DateTime?);

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal?);

                case SqlDbType.Float:
                    return typeof(double?);

                case SqlDbType.Int:
                    return typeof(int?);

                case SqlDbType.Real:
                    return typeof(float?);

                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid?);

                case SqlDbType.SmallInt:
                    return typeof(short?);

                case SqlDbType.TinyInt:
                    return typeof(byte?);

                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return typeof(object);

                case SqlDbType.Structured:
                    return typeof(DataTable);

                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset?);

                default:
                    throw new ArgumentOutOfRangeException("sqlType");
            }
        }
        internal static string GetTableName(this Type type)
        {
            TQueryable query = new TQueryable(type, TQueryDefaults.SqlDialect);
            return query.TableName;
        }
    }
}
