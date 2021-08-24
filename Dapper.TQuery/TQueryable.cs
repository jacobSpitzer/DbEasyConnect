using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.TQuery.Library
{
    internal class AutoIncrementAttribute : Attribute
    {
    }
    public abstract class TQuery<T>
    {
        internal SqlConnection SqlConnection { get; set; }
        internal IQueryable<T> EmptyQuery { get; set; }
        internal string SqlString { get; set; }
    }
    public abstract class TQueryGet<T> : TQuery<T>
    {
        public List<T> Get()
        {
            if (String.IsNullOrEmpty(SqlString))
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
                SqlString = new ExpressionToSQL(EmptyQuery);
            }
            return SqlConnection.Query<T>(SqlString).ToList();
        }
        public Task<IEnumerable<T>> GetAsync()
        {
            if (String.IsNullOrEmpty(SqlString))
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
                SqlString = new ExpressionToSQL(EmptyQuery);
            }
            return SqlConnection.QueryAsync<T>(SqlString);
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
    }
    
    public class TQueryable<T> : TQueryGet<T>
    {
        internal TQueryable(string ConnectionString)
        {
            this.SqlConnection = new SqlConnection(ConnectionString);
            if (EmptyQuery == null)
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
            }
        }
        internal TQueryable(SqlConnection sqlConnection)
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
    public class TQueryableSingle<T> : TQueryGet<T> { }
    public class TQueryableBool<T> : TQueryGet<T> { }
    
    public class TQueryableUpdate<T> : TQueryExecute<T> { }
    public class TQueryableDelete<T> : TQueryExecute<T> { }
    public class TQueryableInsert<T> : TQueryExecute<T> { }
    public class TQueryableCreate<T> : TQueryExecute<T> { }
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
}
