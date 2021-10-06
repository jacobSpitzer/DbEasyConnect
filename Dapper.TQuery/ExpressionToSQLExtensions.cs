using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

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

        internal class SqlTypeConversionHolder
        {
            public string SqlName { get; private set; }
            public Type SqlType { get; private set; }
            public Type DotNetType { get; private set; }
            public SqlTypeConversionHolder(string sqlName,Type sqlType,Type dotnetType)
            {
                SqlName = sqlName; sqlType = SqlType; DotNetType = dotnetType;
            }
        }

        /// <summary>
        /// The map of types. THis maps all the corresponding types between sql server types, .net sql types, and .net types
        /// </summary>
        internal static List<SqlTypeConversionHolder> TypeList = new List<SqlTypeConversionHolder>()
        {
            new SqlTypeConversionHolder("bigint", typeof(SqlInt64),typeof(Int64)),
            new SqlTypeConversionHolder("binary", typeof(SqlBytes),typeof(Byte[])),
            new SqlTypeConversionHolder("bit", typeof(SqlBoolean),typeof(Boolean)),
            new SqlTypeConversionHolder("char", typeof(SqlChars),typeof(char)), //this one may need work
            new SqlTypeConversionHolder("cursor", null,null),
            new SqlTypeConversionHolder("date", typeof(SqlDateTime),typeof(DateTime)),
            new SqlTypeConversionHolder("datetime", typeof(SqlDateTime),typeof(DateTime)),
            new SqlTypeConversionHolder("datetime2", null,typeof(DateTime)),
            new SqlTypeConversionHolder("DATETIMEOFFSET", null,typeof(DateTimeOffset)),
            new SqlTypeConversionHolder("decimal", typeof(SqlDecimal),typeof(Decimal)),
            new SqlTypeConversionHolder("float", typeof(SqlDouble),typeof(Double)),
            //new SqlTypeConversionHolder("geography", typeof(SqlGeography),typeof(null));
            //new SqlTypeConversionHolder("geometry", typeof(SqlGeometry),typeof(null));
            //new SqlTypeConversionHolder("hierarchyid", typeof(SqlHierarchyId),typeof(null));
            new SqlTypeConversionHolder("image", null,null),
            new SqlTypeConversionHolder("int", typeof(SqlInt32),typeof(Int32)),
            new SqlTypeConversionHolder("money", typeof(SqlMoney),typeof(Decimal)),
            new SqlTypeConversionHolder("nchar", typeof(SqlChars),typeof(String)),
            new SqlTypeConversionHolder("ntext", null,null),
            new SqlTypeConversionHolder("numeric", typeof(SqlDecimal),typeof(Decimal)),
            new SqlTypeConversionHolder("nvarchar", typeof(SqlChars),typeof(String)),
            new SqlTypeConversionHolder("nvarchar(1)", typeof(SqlChars),typeof(Char)),
            new SqlTypeConversionHolder("nchar(1)", typeof(SqlChars),typeof(Char)),
            new SqlTypeConversionHolder("real", typeof(SqlSingle),typeof(Single)),
            new SqlTypeConversionHolder("rowversion", null,typeof(Byte[])),
            new SqlTypeConversionHolder("smallint", typeof(SqlInt16),typeof(Int16)),
            new SqlTypeConversionHolder("smallmoney", typeof(SqlMoney),typeof(Decimal)),
            new SqlTypeConversionHolder("sql_variant", null,typeof(Object)),
            new SqlTypeConversionHolder("table", null,null),
            new SqlTypeConversionHolder("text", typeof(SqlString),typeof(string)), //this one may need work
            new SqlTypeConversionHolder("time", null,typeof(TimeSpan)),
            new SqlTypeConversionHolder("timestamp", null,null),
            new SqlTypeConversionHolder("tinyint", typeof(SqlByte),typeof(Byte)),
            new SqlTypeConversionHolder("uniqueidentifier", typeof(SqlGuid),typeof(Guid)),
            new SqlTypeConversionHolder("varbinary", typeof(SqlBytes),typeof(Byte[])),
            new SqlTypeConversionHolder("varbinary(1)", typeof(SqlBytes),typeof(byte)),
            new SqlTypeConversionHolder("binary(1)", typeof(SqlBytes),typeof(byte)),
            new SqlTypeConversionHolder("varchar", typeof(SqlString),typeof(string)), //this one may need work
            new SqlTypeConversionHolder("xml", typeof(SqlXml),typeof(string))
        };

        internal static Type GetDNetType(string sqlString)
        {
            return TypeList.FirstOrDefault(x => x.SqlName == sqlString).DotNetType;
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
    }
}
