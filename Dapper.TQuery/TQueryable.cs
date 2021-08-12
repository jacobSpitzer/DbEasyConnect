using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.TQuery.Library
{
    internal class TQuery<T> : TQueryable<T>, TQueryableSelect<T>, TQueryableOrder<T>, TQueryableFilter<T>, TQueryableGroup<T>, TQueryableJoin<T>, TQueryableBool<T>, TQueryableSingle<T>
    {
        public TQuery(string ConnectionString) { this.SqlConnection = new SqlConnection(ConnectionString); }
        public TQuery(SqlConnection sqlConnection) { this.SqlConnection = sqlConnection; }        
        public SqlConnection SqlConnection { get; set; }
        public IQueryable<T> EmptyQuery { get; set; }
        public string SqlString { get; set; }
        public IEnumerable<T> Get()
        {
            if (String.IsNullOrEmpty(SqlString))
            {
                EmptyQuery = Enumerable.Empty<T>().AsQueryable();
                SqlString = new ExpressionToSQL(EmptyQuery);
            }  
            return SqlConnection.Query<T>(SqlString);
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

    public class TQueryCreate
    {
        public TQueryCreate(string ConnectionString) { this.SqlConnection = new SqlConnection(ConnectionString); }
        public TQueryCreate(SqlConnection sqlConnection) { this.SqlConnection = sqlConnection; }
        public SqlConnection SqlConnection { get; set; }
        public string SqlString { get; set; }
        public int Execute()
        {
            //SqlCommand sql = new SqlCommand(SqlString, SqlConnection);
            return SqlConnection.Execute(SqlString);
        }
        public Task<int> ExecuteAsync()
        {
            return SqlConnection.ExecuteAsync(SqlString);
        }
    }
    public class TQueryExecute<T> : TQueryableUpdate<T>, TQueryableDelete<T>, TQueryableInsert<T>
    {
        public SqlConnection SqlConnection { get; set; }
        public TQueryExecute(string ConnectionString) { this.SqlConnection = new SqlConnection(ConnectionString); }
        public TQueryExecute(SqlConnection sqlConnection) { this.SqlConnection = sqlConnection; }
        public IQueryable<T> EmptyQuery { get; set; }
        public string SqlString { get; set; }
        public int Execute()
        {
            return SqlConnection.Execute(SqlString);
        }
        public Task<int> ExecuteAsync()
        {
            return SqlConnection.ExecuteAsync(SqlString);
        }
    }
    public interface TQueryable<T> 
    { 
        IQueryable<T> EmptyQuery { get; set; } 
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        IEnumerable<T> Get();
        Task<IEnumerable<T>> GetAsync();
    }
    public interface TQueryableSelect<T>
    {
        IQueryable<T> EmptyQuery { get; set; }
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        IEnumerable<T> Get();
        Task<IEnumerable<T>> GetAsync();
    }
    public interface TQueryableOrder<T>
    {
        IQueryable<T> EmptyQuery { get; set; }
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        IEnumerable<T> Get();
        Task<IEnumerable<T>> GetAsync();
    }
    public interface TQueryableFilter<T>
    {
        IQueryable<T> EmptyQuery { get; set; }
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        IEnumerable<T> Get();
        Task<IEnumerable<T>> GetAsync();
    }
    public interface TQueryableGroup<T>
    {
        IQueryable<T> EmptyQuery { get; set; }
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        IEnumerable<T> Get();
        Task<IEnumerable<T>> GetAsync();
    }
    public interface TQueryableJoin<T>
    {
        IQueryable<T> EmptyQuery { get; set; }
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        IEnumerable<T> Get();
        Task<IEnumerable<T>> GetAsync();
    }
    public interface TQueryableSingle<T>
    {
        IQueryable<T> EmptyQuery { get; set; }
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        IEnumerable<T> Get();
        Task<IEnumerable<T>> GetAsync();
    }
    public interface TQueryableBool<T>
    {
        IQueryable<T> EmptyQuery { get; set; }
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        IEnumerable<T> Get();
        Task<IEnumerable<T>> GetAsync();
    }
    public interface TQueryableUpdate<T>
    {
        IQueryable<T> EmptyQuery { get; set; }
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        int Execute();
        Task<int> ExecuteAsync();
    }
    public interface TQueryableDelete<T>
    {
        IQueryable<T> EmptyQuery { get; set; }
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        int Execute();
        Task<int> ExecuteAsync();
    }
    public interface TQueryableInsert<T>
    {
        IQueryable<T> EmptyQuery { get; set; }
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        int Execute();
        Task<int> ExecuteAsync();
    }

    public interface TQueryableCreate
    {
        string SqlString { get; set; }
        SqlConnection SqlConnection { get; set; }
        int Execute();
        Task<int> ExecuteAsync();
    }
}
