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
        private static SqlDbType? ToSqlDbTypeInternal(this Type type)
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
    }
}
