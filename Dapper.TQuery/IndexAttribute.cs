using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace DbEasyConnect.Tools
{
    /// <summary>
    /// When this attribute is placed on a property it indicates that the database column to which the
    /// property is mapped has an index.
    /// </summary>
    /// <remarks>
    /// This attribute is used by Entity Framework Migrations to create indexes on mapped database columns.
    /// Multi-column indexes are created by using the same index name in multiple attributes. The information
    /// in these attributes is then merged together to specify the actual database index.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IndexAttribute : Attribute
    {
        private string _name;
        private int _order = -1;
        private bool? _isClustered;
        private bool? _isUnique;

        /// <summary>
        /// Creates a <see cref="IndexAttribute" /> instance for an index that will be named by convention and
        /// has no column order, clustering, or uniqueness specified.
        /// </summary>
        public IndexAttribute()
        {
        }

        /// <summary>
        /// Creates a <see cref="IndexAttribute" /> instance for an index with the given name and
        /// has no column order, clustering, or uniqueness specified.
        /// </summary>
        /// <param name="name">The index name.</param>
        public IndexAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, name, "name"));

            _name = name;
        }

        /// <summary>
        /// Creates a <see cref="IndexAttribute" /> instance for an index with the given name and column order, 
        /// but with no clustering or uniqueness specified.
        /// </summary>
        /// <remarks>
        /// Multi-column indexes are created by using the same index name in multiple attributes. The information
        /// in these attributes is then merged together to specify the actual database index.
        /// </remarks>
        /// <param name="name">The index name.</param>
        /// <param name="order">A number which will be used to determine column ordering for multi-column indexes.</param>
        public IndexAttribute(string name, int order)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, name, "name"));
            if (order < 0)
            {
                throw new ArgumentOutOfRangeException("order");
            }

            _name = name;
            _order = order;
        }

        internal IndexAttribute(string name, bool? isClustered, bool? isUnique)
        {
            _name = name;
            _isClustered = isClustered;
            _isUnique = isUnique;
        }

        internal IndexAttribute(string name, int order, bool? isClustered, bool? isUnique)
        {
            _name = name;
            _order = order;
            _isClustered = isClustered;
            _isUnique = isUnique;
        }

        /// <summary>
        /// The index name.
        /// </summary>
        /// <remarks>
        /// Multi-column indexes are created by using the same index name in multiple attributes. The information
        /// in these attributes is then merged together to specify the actual database index.
        /// </remarks>
        public virtual string Name
        {
            get { return _name; }
            internal set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, value, "name"));
                _name = value;
            }
        }

        /// <summary>
        /// A number which will be used to determine column ordering for multi-column indexes. This will be -1 if no
        /// column order has been specified.
        /// </summary>
        /// <remarks>
        /// Multi-column indexes are created by using the same index name in multiple attributes. The information
        /// in these attributes is then merged together to specify the actual database index.
        /// </remarks>
        public virtual int Order
        {
            get { return _order; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _order = value;
            }
        }

        /// <summary>
        /// Set this property to true to define a clustered index. Set this property to false to define a 
        /// non-clustered index.
        /// </summary>
        /// <remarks>
        /// The value of this property is only relevant if <see cref="IsClusteredConfigured"/> returns true.
        /// If <see cref="IsClusteredConfigured"/> returns false, then the value of this property is meaningless.
        /// </remarks>
        public virtual bool IsClustered
        {
            get { return _isClustered.HasValue && _isClustered.Value; }
            set { _isClustered = value; }
        }

        /// <summary>
        /// Returns true if <see cref="IsClustered"/> has been set to a value.
        /// </summary>
        public virtual bool IsClusteredConfigured
        {
            get { return _isClustered.HasValue; }
        }

        /// <summary>
        /// Set this property to true to define a unique index. Set this property to false to define a 
        /// non-unique index.
        /// </summary>
        /// <remarks>
        /// The value of this property is only relevant if <see cref="IsUniqueConfigured"/> returns true.
        /// If <see cref="IsUniqueConfigured"/> returns false, then the value of this property is meaningless.
        /// </remarks>
        public virtual bool IsUnique
        {
            get { return _isUnique.HasValue && _isUnique.Value; }
            set { _isUnique = value; }
        }

        /// <summary>
        /// Returns true if <see cref="IsUnique"/> has been set to a value.
        /// </summary>
        public virtual bool IsUniqueConfigured
        {
            get { return _isUnique.HasValue; }
        }
    }
}
