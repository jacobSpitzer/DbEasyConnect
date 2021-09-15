using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.TQuery.Development
{
    internal class AutoIncrementAttribute : Attribute
    {
    }

    public abstract class TQuery<T>
    {
        internal SqlConnection SqlConnection { get; set; }
        internal IQueryable<T> EmptyQuery { get; set; }
        internal string SqlString { get; set; }
        internal SqlDialect SqlDialect { get; set; }
    }
    public abstract class TQueryGet<T>: TQuery<T>
    {
        public List<T>? ToList()
        {
            if (String.IsNullOrEmpty(SqlString))
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
                SqlString = new ExpressionToSQL(EmptyQuery);
            }
            return SqlConnection.Query<T>(SqlString)?.ToList();
        }
        public Task<IEnumerable<T>> ToListAsync()
        {
            if (String.IsNullOrEmpty(SqlString))
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
                SqlString = new ExpressionToSQL(EmptyQuery);
            }
            return SqlConnection.QueryAsync<T>(SqlString);
        }
        public string ToSqlString()
        {
            return SqlString;
        }
    }
    public abstract class TQueryExecute<T> : TQuery<T>
    {
        public int Execute()
        {
            return SqlConnection.Execute(SqlString);
        }
        public Task<int> ExecuteAsync()
        {
            return SqlConnection.ExecuteAsync(SqlString);
        }
        public string ToSqlString()
        {
            return SqlString;
        }
    }
    
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
        }
        internal TQueryable(SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            this.SqlConnection = sqlConnection;
            this.SqlDialect = sqlDialect;
            if (EmptyQuery == null)
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
            }
        }
    }
    public class TQueryableSql<T> : TQueryGet<T>
    {
        internal TQueryableSql(string ConnectionString, SqlDialect sqlDialect)
        {
            this.SqlConnection = new SqlConnection(ConnectionString);
            if (EmptyQuery == null)
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
            }
        }
        internal TQueryableSql(SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            this.SqlConnection = sqlConnection;
            if (EmptyQuery == null)
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
            }
        }
    }
    
    public class TQueryableSelect<T> : TQueryGet<T> { }
    public class TQueryableOrder<T> : TQueryGet<T> { } 
    public class TQueryableFilter<T> : TQueryGet<T> { }
    public class TQueryableGroup<T> : TQueryGet<T> { }
    public class TQueryableJoin<T> : TQueryGet<T> { }
    public class TQueryableJoin<T, TResult> : TQueryGet<T> { }
    public class TQueryableSingle<T> : TQueryGet<T> { }
    public class TQueryableUpdate<T> : TQueryExecute<T> { }
    public class TQueryableDelete<T> : TQueryExecute<T> { }
    public class TQueryableInsert<T> : TQueryExecute<T> { }
    public class TQueryableCreate<T> : TQueryExecute<T> { }
    public class TQueryableBool<T> : TQuery<T>
    {
        public bool Execute()
        {
            return SqlConnection.ExecuteScalar<bool>(SqlString);
        }
        public Task<bool> ExecuteAsync()
        {
            return SqlConnection.ExecuteScalarAsync<bool>(SqlString);
        }
    }
    public class TQueryableDatabase
    {
        internal SqlConnection SqlConnection { get; set; }
        internal string SqlString { get; set; }

        public int Execute()
        {
            return SqlConnection.Execute(SqlString);
        }
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
