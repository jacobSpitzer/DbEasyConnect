using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;



namespace Dapper.TQuery
{
    internal class TableField
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool Required { get; set; } = false;
        public int Order { get; set; }
        public bool PrimaryKey { get; set; } = false;
        public bool AutoIncrement { get; set; } = false;
        public ForeignKey ForeignKey { get; set; }
    }

    internal class ForeignKey
    {
        public ForeignKey(Type internalTable, Type externalTable, string internalKey, string externalKey)
        {
            InternalKey = internalKey;
            ExternalKey = externalKey;
            InternalTable = internalTable;
            ExternalTable = externalTable;
        }
        public Type InternalTable { get; set; }
        public string InternalKey { get; set; }
        public Type ExternalTable { get; set; }
        public string ExternalKey { get; set; }
    }

    public class FieldAttributes
    {
        /// <summary>
        /// Auto Increment Attribute.<br/>
        /// Auto-increment allows a unique number to be generated automatically when a new record is inserted into a table. 
        /// Often this is the primary key field that we would like to be created automatically every time a new record is inserted.
        /// </summary>
        public sealed class AutoIncrementAttribute : Attribute
        {
        }        
        /// <summary>
                 /// Not Auto Increment Attribute.<br/>
                 /// Auto-increment allows a unique number to be generated automatically when a new record is inserted into a table. 
                 /// Often this is the primary key field that we would like to be created automatically every time a new record is inserted.
                 /// </summary>
        public sealed class NotAutoIncrementAttribute : Attribute
        {
        }
        /// <summary>
        /// Identity Attribute.<br/>
        /// Inentity allows a unique number to be generated automatically when a new record is inserted into a table. 
        /// Often this is the primary key field that we would like to be created automatically every time a new record is inserted.
        /// </summary>
        public sealed class IdentityAttribute : Attribute
        {
        }

        /// <summary>
        /// Denotes a property used as a foreign key in a relationship.
        /// The annotation may be placed on the foreign key property and specify the associated navigation property name, 
        /// or placed on a navigation property and specify the associated foreign key name.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
        public class FKeyAttribute : Attribute
        {
            private readonly string _external_table_name;
            private readonly string _external_key;
            private readonly string _internal_key;

            /// <summary>
            /// Initializes a new instance of the <see cref="FKeyAttribute"/> class.
            /// </summary>
            /// <param name="table_name">
            /// If placed on a foreign key property, the name of the associated navigation property.
            /// If placed on a navigation property, the name of the associated foreign key(s).
            /// If a navigation property has multiple foreign keys, a comma separated list should be supplied.
            /// </param>
            public FKeyAttribute(string table_name)
            {
                if (string.IsNullOrWhiteSpace(table_name))
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, table_name, "table_name"));
                }

                _external_table_name = table_name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ForeignKeyAttribute"/> class.
            /// </summary>
            /// <param name="table_name">
            /// <paramref name="external_key"/>
            /// If placed on a foreign key property, the name of the associated navigation property.
            /// If placed on a navigation property, the name of the associated foreign key(s).
            /// If a navigation property has multiple foreign keys, a comma separated list should be supplied.
            /// </param>
            public FKeyAttribute(string table_name, string external_key)
            {
                if (string.IsNullOrWhiteSpace(table_name))
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, table_name, "table_name"));
                }
                if (string.IsNullOrWhiteSpace(external_key))
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, external_key, "external_key"));
                }

                _external_table_name = table_name;
                _external_key = external_key;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ForeignKeyAttribute"/> class.
            /// </summary>
            /// <param name="table_name">
            /// <paramref name="external_key"/>
            /// <paramref name="internal_key"/>
            /// If placed on a foreign key property, the name of the associated navigation property.
            /// If placed on a navigation property, the name of the associated foreign key(s).
            /// If a navigation property has multiple foreign keys, a comma separated list should be supplied.
            /// </param>
            public FKeyAttribute(string table_name, string external_key, string internal_key)
            {
                if (string.IsNullOrWhiteSpace(table_name))
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, table_name, "table_name"));
                }
                if (string.IsNullOrWhiteSpace(external_key))
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, external_key, "external_key"));
                }
                if (string.IsNullOrWhiteSpace(internal_key))
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, internal_key, "internal_key"));
                }

                _external_table_name = table_name;
                _external_key = external_key;
                _internal_key = internal_key;
            }

            /// <summary>
            /// If placed on a foreign key property, the name of the associated navigation property.
            /// If placed on a navigation property, the name of the associated foreign key(s).
            /// </summary>
            public string TableName
            {
                get { return _external_table_name; }
            }
            public string ExternalKey
            {
                get { return _external_key; }
            }
            public string InternalKey
            {
                get { return _internal_key; }
            }
        }

        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public sealed class SubTypeAttribute : Attribute
        {
            public bool IncludeParentName { get; set; } = false;
        }

        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public sealed class OrderAttribute : Attribute
        {
            private readonly int order_;
            public OrderAttribute([CallerLineNumber] int order = 0)
            {
                order_ = order;
            }

            public int Order { get { return order_; } }
        }
    }

    public class TQueryExceptions
    {


        [Serializable]
        internal class NotSupportedFieldTypeException : Exception
        {
            private System.Reflection.PropertyInfo Field;
            public override string Message => $"{Field.GetType()} is not supported as an TQuery table field.\nTo exclude this property from the table, add [NotMapped] attribute above {Field}.";
            public NotSupportedFieldTypeException(System.Reflection.PropertyInfo field)
            {
                this.Field = field;
            }

        }

        [Serializable]
        internal class MissingTableAttributeException : Exception
        {
            private Type type;
            public override string Message => $"Missing '[Table(\"name\")]' attribute.\nAdd [Table] attribute to type class: {type}";
            public MissingTableAttributeException(Type type)
            {
                this.type = type;
            }
        }
    }
}
