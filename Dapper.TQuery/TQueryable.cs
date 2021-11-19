using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dapper.TQuery
{
    public static class TQueryDefaults
    {
        /// <summary>
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// If no dialect was given, the default dialect <see cref="SqlDialect.SqlServer"/> will be used.
        /// </summary>
        public static SqlDialect SqlDialect { get; set; } = SqlDialect.SqlServer;
        public static string Schema { get; set; } = "dbo";
        public static bool AllowSubTypeAuto { get; set; } = true;
        public static bool SetNullablePropsInSqlToNull { get; set; } = false;
        public static bool IdFieldIsKeyByDefault { get; set; } = true;
        public static bool PrimaryKeyIsAutoIncrementByDefault { get; set; } = true;

    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class TQuery<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        internal SqlConnection SqlConnection { get; set; }
        internal IQueryable<T> EmptyQuery { get; set; }
        internal string SqlString { get; set; }
        internal SqlDialect SqlDialect { get; set; }
        internal string TableName { get; set; }
        internal Type TableType { get; set; }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class TQueryGet<T> : TQuery<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Executes a query, returning an IEnumerable object with the data typed as T.
        /// </summary>
        /// <returns>
        /// A sequence of data of the supplied table type; an instance
        /// is created per row, and a direct column-name===member-name mapping is assumed
        /// (case insensitive).
        /// </returns>
        public IEnumerable<T> AsEnumerable()
        {
            if (String.IsNullOrEmpty(SqlString))
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
                SqlString = new ExpressionToSQL(EmptyQuery);
            }
            return SqlConnection.Query<T>(SqlString);
        }
        /// <summary>
        /// Executes a query, returning an List object with the data typed as T.
        /// </summary>
        /// <returns>
        /// A sequence of data of the supplied table type; an instance
        /// is created per row, and a direct column-name===member-name mapping is assumed
        /// (case insensitive).
        /// </returns>
        public IList<T> ToList()
        {
            if (String.IsNullOrEmpty(SqlString))
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
                SqlString = new ExpressionToSQL(EmptyQuery);
            }
            return SqlConnection.Query<T>(SqlString)?.ToList();
        }
        /// <summary>
        /// Execute a query asynchronously using Task, returning an List object with the data typed as T.
        /// </summary>
        /// <returns>
        /// A sequence of data of the supplied table type; an instance
        /// is created per row, and a direct column-name===member-name mapping is assumed
        /// (case insensitive).
        /// </returns>
        public Task<IEnumerable<T>> ToListAsync()
        {
            if (String.IsNullOrEmpty(SqlString))
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
                SqlString = new ExpressionToSQL(EmptyQuery);
            }
            return SqlConnection.QueryAsync<T>(SqlString);
        }
        /// <summary>
        /// Returns the TQuery generated Sql command.<br/>
        /// To modify the Sql command manually, use the TQueryExtended method,
        /// and then use the <see cref="TQueryStartExtensions.ModifySqlString{Table}(TQueryableExtended{Table}, string)"/> method.
        /// </summary>
        /// <returns> TQuery generated Sql command as a String.</returns>
        public string ToSqlString()
        {
            return SqlString;
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class TQueryExecute<T> : TQuery<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Execute TQuery SQL command.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public int Execute()
        {
            return SqlConnection.Execute(SqlString);
        }
        /// <summary>
        /// Execute TQuery SQL command asynchronously using Task.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public Task<int> ExecuteAsync()
        {
            return SqlConnection.ExecuteAsync(SqlString);
        }
        /// <summary>
        /// Returns the TQuery generated Sql command.<br/>
        /// To modify the Sql command manually, use the TQueryExtended method,
        /// and then use the ModifySqlString or ReplaceInSqlString method.
        /// </summary>
        /// <returns> TQuery generated Sql command as a String.</returns>
        public string ToSqlString()
        {
            return SqlString;
        }
    }
    /// <summary>
    /// An <see cref="TQueryable{T}"/> instanse which will be used to query and/or modify the Database with Dapper.TQuery method extensions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TQueryable<T> : TQueryGet<T>
    {
        internal TQueryable(string ConnectionString, SqlDialect sqlDialect)
        {
            this.SqlConnection = new SqlConnection(ConnectionString);
            this.SqlDialect = sqlDialect;
            if (EmptyQuery == null)
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
            }
            this.TableType = typeof(T);
            this.TableName = typeof(T).Name + "s";
            if (TableType.GetCustomAttribute<TableAttribute>().Schema?.Length > 0 || !string.IsNullOrEmpty(TQueryDefaults.Schema))
            {
                string schema = TableType.GetCustomAttribute<TableAttribute>().Schema?.Length > 0 ?
                    TableType.GetCustomAttribute<TableAttribute>().Schema : TQueryDefaults.Schema;
                this.TableName = $"{schema}.{this.TableName}";
            }
            else { this.TableName = TableType.GetCustomAttribute<TableAttribute>().Name; }
        }
        internal TQueryable(SqlConnection SqlConnection, SqlDialect sqlDialect)
        {
            this.SqlConnection = SqlConnection;
            this.SqlDialect = sqlDialect;
            if (EmptyQuery == null)
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
            }
            this.TableType = typeof(T);
            this.TableName = typeof(T).Name + "s";
            if (TableType.GetCustomAttribute<TableAttribute>().Schema?.Length > 0 || !string.IsNullOrEmpty(TQueryDefaults.Schema))
            {
                string schema = TableType.GetCustomAttribute<TableAttribute>().Schema?.Length > 0 ?
                    TableType.GetCustomAttribute<TableAttribute>().Schema : TQueryDefaults.Schema;
                this.TableName = $"{schema}.{this.TableName}";
            }
            else { this.TableName = TableType.GetCustomAttribute<TableAttribute>().Name; }
        }
    }
    internal class TQueryable
    {
        internal SqlDialect SqlDialect { get; set; }
        internal string TableName { get; set; }
        internal Type TableType { get; set; }
        internal TQueryable(Type type, SqlDialect sqlDialect)
        {
            this.SqlDialect = sqlDialect;
            this.TableType = type;
            this.TableName = type.Name + "s";
            if (TableType.GetCustomAttribute<TableAttribute>().Schema?.Length > 0 || !string.IsNullOrEmpty(TQueryDefaults.Schema))
            {
                string schema = TableType.GetCustomAttribute<TableAttribute>().Schema?.Length > 0 ?
                    TableType.GetCustomAttribute<TableAttribute>().Schema : TQueryDefaults.Schema;
                this.TableName = $"{schema}.{this.TableName}";
            }
            else { this.TableName = TableType.GetCustomAttribute<TableAttribute>().Name; }
        }
    }

    /// <summary>
    /// An <see cref="TQueryableExtended{T}"/> instanse which will be used to query and/or modify the Database with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TQueryableExtended<T> : TQueryGet<T>
    {
        internal TQueryableExtended(string ConnectionString, SqlDialect sqlDialect)
        {
            this.SqlConnection = new SqlConnection(ConnectionString);
            if (EmptyQuery == null)
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
            }
            this.TableType = typeof(T);
            this.TableName = typeof(T).Name + "s";
            if (TableType.GetCustomAttribute<TableAttribute>().Schema?.Length > 0 || !string.IsNullOrEmpty(TQueryDefaults.Schema))
            {
                string schema = TableType.GetCustomAttribute<TableAttribute>().Schema?.Length > 0 ?
                    TableType.GetCustomAttribute<TableAttribute>().Schema : TQueryDefaults.Schema;
                this.TableName = $"{schema}.{this.TableName}";
            }
            else { this.TableName = TableType.GetCustomAttribute<TableAttribute>().Name; }
        }
        internal TQueryableExtended(SqlConnection SqlConnection, SqlDialect sqlDialect)
        {
            this.SqlConnection = SqlConnection;
            if (EmptyQuery == null)
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
            }
            this.TableType = typeof(T);
            this.TableName = typeof(T).Name + "s";
            if (TableType.GetCustomAttribute<TableAttribute>().Schema?.Length > 0 || !string.IsNullOrEmpty(TQueryDefaults.Schema))
            {
                string schema = TableType.GetCustomAttribute<TableAttribute>().Schema?.Length > 0 ?
                    TableType.GetCustomAttribute<TableAttribute>().Schema : TQueryDefaults.Schema;
                this.TableName = $"{schema}.{this.TableName}";
            }
            else { this.TableName = TableType.GetCustomAttribute<TableAttribute>().Name; }
        }
    }
    /// <summary>
    /// An <see cref="TQueryableSelect{T}"/> instanse which will be used for the 'Select' method extensions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TQueryableSelect<T> : TQueryGet<T> { }
    /// <summary>
    /// An <see cref="TQueryableOrder{T}"/> instanse which will be used for the 'Order' method extensions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TQueryableOrder<T> : TQueryGet<T> { }
    /// <summary>
    /// An <see cref="TQueryableUpdate{T}"/> instanse which will be used for the 'Update' method extensions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TQueryableUpdate<T> : TQueryExecute<T> { }
    /// <summary>
    /// An <see cref="TQueryableDelete{T}"/> instanse which will be used for the 'Delete' method extensions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TQueryableDelete<T> : TQueryExecute<T> { }
    /// <summary>
    /// An <see cref="TQueryableCreate{T}"/> instanse which will be used for all 'Create/Modify/Delete Table' method extensions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TQueryableCreate<T> : TQueryExecute<T> { }
    /// <summary>
    /// An <see cref="TQueryableBool{T}"/> instanse which will be used for the 'Boolean' method extensions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TQueryableBool<T> : TQuery<T>
    {
        /// <summary>
        /// Execute TQuery SQL boolean command and returns true or false.
        /// </summary>
        /// <returns> True if the command result is true, otherwise returns false.</returns>
        public bool Execute()
        {
            return SqlConnection.ExecuteScalar<bool>(SqlString);
        }
        /// <summary>
        /// Execute TQuery SQL boolean command asynchronously using Task, and returns true or false.
        /// </summary>
        /// <returns> True if the command result is true, otherwise returns false.</returns>
        public Task<bool> ExecuteAsync()
        {
            return SqlConnection.ExecuteScalarAsync<bool>(SqlString);
        }
    }
    /// <summary>
    /// An <see cref="TQueryDatabase"/> instanse which will be used to handle with all tables at once on the Database with Dapper.TQuery method extensions.
    /// <br/>Like create/drop all tables, get all table defenitions and compare with the code tables, and more.
    /// </summary>
    public class TQueryDatabase
    {
        internal SqlConnection SqlConnection { get; set; }
        internal string SqlString { get; set; }
        internal SqlDialect SqlDialect { get; set; }
        internal TQueryDatabase(string ConnectionString, SqlDialect sqlDialect)
        {
            this.SqlConnection = new SqlConnection(ConnectionString);
            this.SqlDialect = sqlDialect;
        }
        internal TQueryDatabase(SqlConnection SqlConnection, SqlDialect sqlDialect)
        {
            this.SqlConnection = SqlConnection;
            this.SqlDialect = sqlDialect;
        }
        /// <summary>
        /// Execute TQuery SQL command.
        /// </summary>
        public void Execute()
        {
            SqlConnection.Execute(SqlString);
        }
        /// <summary>
        /// Execute TQuery SQL command asynchronously using Task.
        /// </summary>
        public Task<int> ExecuteAsync()
        {
            return SqlConnection.ExecuteAsync(SqlString);
        }
    }
    /// <summary>
    /// An <see cref="TQueryDatabase"/> instanse which will be used to handle with all tables at once on the Database with Dapper.TQuery method extensions, with some advanced options of the TQuery library, to read/modify the generated SQL command, and more.
    /// </summary>
    public class TQueryDatabaseExtended
    {
        internal SqlConnection SqlConnection { get; set; }
        internal string SqlString { get; set; }
        internal SqlDialect SqlDialect { get; set; }
        internal TQueryDatabaseExtended(string ConnectionString, SqlDialect sqlDialect)
        {
            this.SqlConnection = new SqlConnection(ConnectionString);
            this.SqlDialect = sqlDialect;
        }
        internal TQueryDatabaseExtended(SqlConnection SqlConnection, SqlDialect sqlDialect)
        {
            this.SqlConnection = SqlConnection;
            this.SqlDialect = sqlDialect;
        }
        /// <summary>
        /// Execute TQuery SQL command.
        /// </summary>
        public void Execute()
        {
            SqlConnection.Execute(SqlString);
        }
        /// <summary>
        /// Execute TQuery SQL command asynchronously using Task.
        /// </summary>
        public Task<int> ExecuteAsync()
        {
            return SqlConnection.ExecuteAsync(SqlString);
        }
    }

    /// <summary>
    /// SQL dialect enumeration
    /// </summary>
    public enum SqlDialect
    {
        /// <summary>
        /// MS SQL Server
        /// </summary>
        SqlServer,

        /// <summary>
        /// MySql
        /// </summary>
        MySql,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,

        /// <summary>
        /// SQLite
        /// </summary>
        SQLite,

        /// <summary>
        /// PostgreSql
        /// </summary>
        PostgreSql
    }

    /// <summary>
    /// SQL Join Type enumeration
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// (INNER) JOIN: Returns records that have matching values in both tables
        /// </summary>
        InnerJoin,

        /// <summary>
        /// LEFT (OUTER) JOIN: Returns all records from the left table, and the matched records from the right table
        /// </summary>
        LeftJoin,

        /// <summary>
        /// RIGHT (OUTER) JOIN: Returns all records from the right table, and the matched records from the left table
        /// </summary>
        RightJoin,

        /// <summary>
        /// FULL (OUTER) JOIN: Returns all records when there is a match in either left or right table
        /// </summary>
        FullJoin
    }
}
