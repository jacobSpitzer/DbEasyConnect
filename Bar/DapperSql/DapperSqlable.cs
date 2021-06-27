using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bar.DapperSql
{
    public class Sqlable<T> : ISqlable<T>
    {
        public IQueryable<T> Empty { get; set; }
        public string SqlString { get; set; }

        public int ExecuteCommand()
        {
            return 0;
        }

        public List<T> ExecuteQuery()
        {
            return new List<T>();
        }

        public bool IsQuery()
        {
            return true;
        }

        int ISqlable<T>.ExecuteCommand()
        {
            return 0;
        }
    }

    
    public static class SqlableExtensions
    {
        #region private methods to allow call extensios
        //private static readonly MethodInfo _deleteMethod;
        //private static readonly MethodInfo _deleteMethodAsync;
        //private static readonly MethodInfo _toSqlStringMethod;
        //private static readonly MethodInfo _updateMethod;
        //private static readonly MethodInfo _updateMethodAsync;
        //private static readonly MethodInfo _whereMethod;

        //static SqlableExtensions()
        //{
        //    Type extensionType = typeof(SqlableExtensions);
        //    _whereMethod = extensionType.GetMethod(nameof(SqlableExtensions.Where), BindingFlags.Static | BindingFlags.Public);
        //    //_deleteMethod = extensionType.GetMethod(nameof(Extensions.Delete), BindingFlags.Static | BindingFlags.Public);
        //    _updateMethod = extensionType.GetMethod(nameof(SqlableExtensions.Update), BindingFlags.Static | BindingFlags.Public);

        //    //_deleteMethodAsync = extensionType.GetMethod(nameof(Extensions.DeleteAsync), BindingFlags.Static | BindingFlags.Public);
        //    //_updateMethodAsync = extensionType.GetMethod(nameof(Extensions.Update), BindingFlags.Static | BindingFlags.Public);

        //    //_toSqlStringMethod = extensionType.GetMethod(nameof(Extensions.ToSqlString), BindingFlags.Static | BindingFlags.Public);
        //}
        //private static IQueryable<TResult> AppendCall<TSource, TResult>(this IQueryable<TSource> queryable, MethodInfo methodInfo, Expression<Func<TSource, TResult>> selector)
        //{
        //    MethodInfo methodInfoGeneric = methodInfo.MakeGenericMethod(typeof(TSource), typeof(TResult));
        //    MethodCallExpression methodCallExpression = Expression.Call(methodInfoGeneric, queryable.Expression, selector);

        //    return new EntityQueryable<TResult>(queryable.Provider as IAsyncQueryProvider, methodCallExpression);
        //}

        //private static IQueryable<T> AppendCall<T>(this IQueryable<T> queryable, MethodInfo methodInfo)
        //{
        //    MethodInfo methodInfoGeneric = methodInfo.MakeGenericMethod(typeof(T));
        //    MethodCallExpression methodCallExpression = Expression.Call(methodInfoGeneric, queryable.Expression);

        //    return new EntityQueryable<T>(queryable.Provider as IAsyncQueryProvider, methodCallExpression);
        //}
        #endregion
        public static ISqlable<T> Where<T>(this ISqlable<T> sqlable, Expression<Func<T, bool>> predicate)
        {
            if (sqlable.Empty == null) { sqlable.Empty = Enumerable.Empty<T>().AsQueryable(); }
            sqlable.Empty = sqlable.Empty.Where(predicate);
            sqlable.SqlString = new ExpressionToSQL(sqlable.Empty);
            return sqlable;
        }
        public static ISqlable<T> OrderBy<T,TKey>(this ISqlable<T> sqlable, Expression<Func<T, TKey>> predicate)
        {
            if (sqlable.Empty == null) { sqlable.Empty = Enumerable.Empty<T>().AsQueryable(); }
            sqlable.Empty = sqlable.Empty.OrderBy(predicate);
            sqlable.SqlString = new ExpressionToSQL(sqlable.Empty);
            return sqlable;
        }
        
        public static ISqlable<T> ThenBy<T, TKey>(this ISqlable<T> sqlable, Expression<Func<T, TKey>> predicate)
        {
            if (sqlable.Empty == null) { sqlable.Empty = Enumerable.Empty<T>().AsQueryable(); }
            if (typeof(IOrderedQueryable<T>).IsAssignableFrom(sqlable.Empty.Expression.Type))
            {
                IOrderedQueryable<T> orderedQuery = (IOrderedQueryable<T>)sqlable.Empty;
                sqlable.Empty = orderedQuery.ThenBy(predicate);
            } else
            {
                sqlable.Empty = sqlable.Empty.OrderBy(predicate);
            }
            sqlable.SqlString = new ExpressionToSQL(sqlable.Empty);
            return sqlable;
        }

        public static ISqlable<T> OrderByDescending<T, TKey>(this ISqlable<T> sqlable, Func<T, TKey> predicate)
        {
            if (sqlable.Empty == null) { sqlable.Empty = Enumerable.Empty<T>().AsQueryable(); }
            sqlable.Empty = (IQueryable<T>)sqlable.Empty.OrderByDescending(predicate);
            sqlable.SqlString = new ExpressionToSQL(sqlable.Empty);
            return sqlable;
        }
        public static ISqlable<T> ThenByDescending<T, TKey>(this ISqlable<T> sqlable, Expression<Func<T, TKey>> predicate)
        {
            if (sqlable.Empty == null) { sqlable.Empty = Enumerable.Empty<T>().AsQueryable(); }
            if (typeof(IOrderedQueryable<T>).IsAssignableFrom(sqlable.Empty.Expression.Type))
            {
                IOrderedQueryable<T> orderedQuery = (IOrderedQueryable<T>)sqlable.Empty;
                sqlable.Empty = orderedQuery.ThenByDescending(predicate);
            }
            else
            {
                sqlable.Empty = sqlable.Empty.OrderByDescending(predicate);
            }
            sqlable.SqlString = new ExpressionToSQL(sqlable.Empty);
            return sqlable;
        }
        public static ISqlable<T> GroupBy<T, TKey>(this ISqlable<T> sqlable, Expression<Func<T, TKey>> predicate)
        {
            if (sqlable.Empty == null) { sqlable.Empty = Enumerable.Empty<T>().AsQueryable(); }
            sqlable.Empty = sqlable.Empty.GroupBy(predicate).SelectMany(x=>x);
            sqlable.SqlString = new ExpressionToSQL(sqlable.Empty);
            return sqlable;
        }
        public static ISqlable<T> Take<T>(this ISqlable<T> sqlable, int predicate)
        {
            if (sqlable.Empty == null) { sqlable.Empty = Enumerable.Empty<T>().AsQueryable(); }
            sqlable.Empty = sqlable.Empty.Take(predicate);
            sqlable.SqlString = new ExpressionToSQL(sqlable.Empty);

            return sqlable;
        }
        public static ISqlable<T> Skip<T>(this ISqlable<T> sqlable, int predicate)
        {
            if (sqlable.Empty == null) { sqlable.Empty = Enumerable.Empty<T>().AsQueryable(); }
            sqlable.Empty = sqlable.Empty.Skip(predicate);
            sqlable.SqlString = new ExpressionToSQL(sqlable.Empty);

            return sqlable;
        }
        public static ISqlable<T> Update<T, TResult>(this ISqlable<T> sqlable, Expression<Func<T, TResult>> selector)
        {
            if (sqlable.Empty == null) { sqlable.Empty = Enumerable.Empty<T>().AsQueryable(); }
            //sqlable.Empty = sqlable.Empty.PUpdate(selector);
            sqlable.SqlString = new ExpressionToSQL(sqlable.Empty.PUpdate(selector));

            return sqlable;
        }

        public static Expression PUpdate<T, TResult>(this IQueryable<T> sqlable, Expression<Func<T, TResult>> selector)
        {
            var methodInfo = typeof(SqlableExtensions).GetMethod(nameof(SqlableExtensions.PUpdate), BindingFlags.Static | BindingFlags.Public);
            MethodInfo methodInfoGeneric = methodInfo.MakeGenericMethod(typeof(T), typeof(TResult));
            MethodCallExpression methodCallExpression = Expression.Call(methodInfoGeneric, sqlable.Expression, selector);
            return Expression.Lambda(methodCallExpression);
        }

        public static ISqlable<T> SelectMany<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Join<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> GroupJoin<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> All<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Any<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Contains<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Aggregate<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Average<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Count<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Max<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Sum<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> ElementAt<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> ElementAtOrDefault<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> First<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> FirstOrDefault<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Last<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> LastOrDefault<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Signle<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> SignleOrDefault<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Distinct<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }
        public static ISqlable<T> Union<T>(this ISqlable<T> sqlable, Func<T> predicate)
        {

            return sqlable;
        }

        public static ISqlable<T> Delete<T>(this ISqlable<T> sqlable)
        {
            //var sql = new ExpressionToSQL(predicate);

            return new Sqlable<T>();// { SqlString = sql };
        }

        public static ISqlable<T> Create<T>(this ISqlable<T> sqlable)
        {
            return new Sqlable<T>();
        }

        public static ISqlable<T> Insert<T>(this ISqlable<T> sqlable, List<T> records)
        {
            return new Sqlable<T>();
        }
        public static ISqlable<T> Drop<T>(this ISqlable<T> sqlable)
        {
            return new Sqlable<T>();
        }

    }
}
