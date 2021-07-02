using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TQueryable.Library
{
    public static class ExpressionToSQLExtensions
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
