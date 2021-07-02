using System;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using TQueryableProj;

namespace TQueryable.Library
{

    public class TQuery<T> : TQueryable<T>, TQueryableOrdered<T>, TQueryableGrouped<T>, TQueryableFiltered<T>, TQueryableJoined<T>, TQueryableBoolean<T>, TQueryableUpdated<T>, TQueryableSelected<T>, TQueryableSingle<T>
    {
        private readonly IConfiguration _configuration;

        public static IConfigurationRoot Configuration { get; private set; }
        public IQueryable<T> Empty { get; set; }
        public string SqlString { get; set; }

        public int ExecuteCommand()
        {
            using (var connection = new SqlConnection("Data Source=hgws27.win.hostgator.com; Database=quickpps_tquery; User ID=quickpps_satmer;Password=If3t9q2*; MultipleActiveResultSets=True"))
            {
                return connection.Execute(SqlString);
            }
        }

        public List<T> ExecuteQuery()
        {
            return new List<T>();
        }

        public bool IsQuery()
        {
            return true;
        }
        int TQueryable<T>.ExecuteCommand()
        {
            string conString = ConfigurationExtensions.GetConnectionString(Startup.Configuration, "DefaultConnection");
            using (var connection = new SqlConnection(conString))
            {
                return connection.Execute(SqlString);
            }
        }
    }

    
    public static class TQueryExtensions
    {
        public static TQueryableFiltered<T> Where<T>(this TQueryable<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableFiltered<T> filteredQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString};
            return filteredQuery;
        }
        //TODO need to add overload method, to order a grouped query. but need first to fix code. because the fields available for order is not grouped.
        public static TQueryableOrdered<T> OrderBy<T,TKey>(this TQueryable<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.OrderBy(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableOrdered<T> orderedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return orderedQuery;
        }
        public static TQueryableOrdered<T> OrderBy<T, TKey>(this TQueryableFiltered<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.OrderBy(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableOrdered<T> orderedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return orderedQuery;
        }
        public static TQueryableOrdered<T> ThenBy<T, TKey>(this TQueryableOrdered<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            if (typeof(IOrderedQueryable<T>).IsAssignableFrom(tQuery.Empty.Expression.Type))
            {
                IOrderedQueryable<T> orderedQuery = (IOrderedQueryable<T>)tQuery.Empty;
                tQuery.Empty = orderedQuery.ThenBy(predicate);
            } else
            {
                tQuery.Empty = tQuery.Empty.OrderBy(predicate);
            }
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableOrdered<T> _orderedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }
        public static TQueryableOrdered<T> OrderByDescending<T, TKey>(this TQueryable<T> tQuery, Func<T, TKey> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = (IQueryable<T>)tQuery.Empty.OrderByDescending(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableOrdered<T> _orderedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }
        public static TQueryableOrdered<T> OrderByDescending<T, TKey>(this TQueryableFiltered<T> tQuery, Func<T, TKey> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = (IQueryable<T>)tQuery.Empty.OrderByDescending(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableOrdered<T> _orderedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }
        public static TQueryableOrdered<T> ThenByDescending<T, TKey>(this TQueryableOrdered<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            if (typeof(IOrderedQueryable<T>).IsAssignableFrom(tQuery.Empty.Expression.Type))
            {
                IOrderedQueryable<T> orderedQuery = (IOrderedQueryable<T>)tQuery.Empty;
                tQuery.Empty = orderedQuery.ThenByDescending(predicate);
            }
            else
            {
                tQuery.Empty = tQuery.Empty.OrderByDescending(predicate);
            }
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableOrdered<T> _orderedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }
        public static TQueryableGrouped<T> GroupBy<T, TKey>(this TQueryable<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.GroupBy(predicate).SelectMany(x=>x);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableGrouped<T> _groupedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _groupedQuery;
        }
        public static TQueryableGrouped<T> GroupBy<T, TKey>(this TQueryableFiltered<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.GroupBy(predicate).SelectMany(x => x);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableGrouped<T> _groupedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _groupedQuery;
        }
        public static TQueryableGrouped<T> GroupBy<T, TKey>(this TQueryableOrdered<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.GroupBy(predicate).SelectMany(x => x);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableGrouped<T> _groupedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _groupedQuery;
        }
        public static TQueryable<T> Take<T>(this TQueryable<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);

            return tQuery;
        }
        public static TQueryable<T> Skip<T>(this TQueryable<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Skip(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);

            return tQuery;
        }
        public static TQueryable<T> Top<T>(this TQueryable<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            return tQuery;
        }
        public static TQueryable<T> Bottom<T>(this TQueryable<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            var exp = ExpressionToSQL.Bottom(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty,exp);
            return tQuery;
        }
        public static TQueryableFiltered<T> Take<T>(this TQueryableFiltered<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);

            return tQuery;
        }
        public static TQueryableFiltered<T> Skip<T>(this TQueryableFiltered<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Skip(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);

            return tQuery;
        }
        public static TQueryableFiltered<T> Top<T>(this TQueryableFiltered<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            return tQuery;
        }
        public static TQueryableFiltered<T> Bottom<T>(this TQueryableFiltered<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            var exp = ExpressionToSQL.Bottom(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty, exp);
            return tQuery;
        }
        public static TQueryableOrdered<T> Take<T>(this TQueryableOrdered<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);

            return tQuery;
        }
        public static TQueryableOrdered<T> Skip<T>(this TQueryableOrdered<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Skip(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);

            return tQuery;
        }
        public static TQueryableOrdered<T> Top<T>(this TQueryableOrdered<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            return tQuery;
        }
        public static TQueryableOrdered<T> Bottom<T>(this TQueryableOrdered<T> tQuery, int predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            var exp = ExpressionToSQL.Bottom(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty, exp);
            return tQuery;
        }
        public static TQueryableUpdated<T> Update<T>(this TQueryable<T> tQuery, Expression<Func<T, T>> expression)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty, ExpressionToSQL.PUpdate(expression));
            TQueryableUpdated<T> _updatedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _updatedQuery;
        }
        public static TQueryableUpdated<T> Update<T>(this TQueryableFiltered<T> tQuery, Expression<Func<T, T>> expression)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty, ExpressionToSQL.PUpdate(expression));
            TQueryableUpdated<T> _updatedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _updatedQuery;
        }
        public static TQueryableSelected<T> Select<T,TResult>(this TQueryable<T> tQuery, Expression<Func<T, TResult>> expression)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty, ExpressionToSQL.PSelect(expression));
            TQueryableSelected<T> _selectedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _selectedQuery;
        }
        public static TQueryableSelected<T> Select<T, TResult>(this TQueryableOrdered<T> tQuery, Expression<Func<T, TResult>> expression)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty, ExpressionToSQL.PSelect(expression));
            TQueryableSelected<T> _selectedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _selectedQuery;
        }
        public static TQueryableSelected<T> Select<T, TResult>(this TQueryableFiltered<T> tQuery, Expression<Func<T, TResult>> expression)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty, ExpressionToSQL.PSelect(expression));
            TQueryableSelected<T> _selectedQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _selectedQuery;
        }
        //TODO fix all joined methods to returnType 'TQueryableJoined<T>'. but it will require more methods,to allow order and filter the joined.
        //some issues with JOIN: 
        //1. Join should be take only regular TQueryable, and all methods should be available on it. but right now when join is defined with another method, the join statement is not correct. (1. join is missing, 2. coulumn selector is not writing from which table)
        //2. when selecting all fields from both tables, the statement is returns only one star, should be a star for each table. 
        //3. handle duplicate column names.
        public static TQueryableJoined<TResult> Join<TOuter, TInner, TKey, TResult>(this TQueryable<TOuter> outer, TQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            if (outer.Empty == null) { outer.Empty = Enumerable.Empty<TOuter>().AsQueryable(); }
            IQueryable<TResult> empty = outer.Empty.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
            outer.SqlString = new ExpressionToSQL(empty, outer.Empty.Expression, "JOIN");
            TQueryableJoined<TResult> _joinedQuery = new TQuery<TResult>() { Empty = empty, SqlString = outer.SqlString };
            return _joinedQuery;
        }
        public static TQueryableJoined<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this TQueryable<TOuter> outer, TQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            if (outer.Empty == null) { outer.Empty = Enumerable.Empty<TOuter>().AsQueryable(); }
            IQueryable<TResult> empty = outer.Empty.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
            outer.SqlString = new ExpressionToSQL(empty,null, "JOIN LEFT");
            TQueryableJoined<TResult> _joinedQuery = new TQuery<TResult>() { Empty = empty, SqlString = outer.SqlString };
            return _joinedQuery;
        }
        public static TQueryableJoined<TResult> RightJoin<TOuter, TInner, TKey, TResult>(this TQueryable<TOuter> outer, TQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            if (outer.Empty == null) { outer.Empty = Enumerable.Empty<TOuter>().AsQueryable(); }
            IQueryable<TResult> empty = outer.Empty.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
            outer.SqlString = new ExpressionToSQL(empty,null, "JOIN RIGHT");
            TQueryableJoined<TResult> _joinedQuery = new TQuery<TResult>() { Empty = empty, SqlString = outer.SqlString };
            return _joinedQuery;
        }
        public static TQueryableJoined<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this TQueryable<TOuter> outer, TQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            if (outer.Empty == null) { outer.Empty = Enumerable.Empty<TOuter>().AsQueryable(); }
            IQueryable<TResult> empty = outer.Empty.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
            outer.SqlString = new ExpressionToSQL(empty, outer.Empty.GroupBy(outerKeySelector).Expression, "JOIN");
            TQueryableJoined<TResult> _joinedQuery = new TQuery<TResult>() { Empty = empty, SqlString = outer.SqlString };
            return _joinedQuery;
        }
        public static TQueryable<T> All<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> Any<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> Contains<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> Aggregate<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> Average<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> Count<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> Max<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> Sum<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> ElementAt<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> ElementAtOrDefault<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryableSingle<T> First<T>(this TQueryable<T> tQuery, Expression<Func<T, bool>> predicate)

        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Take(1).Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableSingle<T> _singleQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> FirstOrDefault<T>(this TQueryable<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Take(1).Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableSingle<T> _singleQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> Last<T>(this TQueryable<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty,exp);
            TQueryableSingle<T> _singleQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> LastOrDefault<T>(this TQueryable<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty,exp);
            TQueryableSingle<T> _singleQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> First<T>(this TQueryableOrdered<T> tQuery, Expression<Func<T, bool>> predicate)

        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Take(1).Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableSingle<T> _singleQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> FirstOrDefault<T>(this TQueryableOrdered<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Take(1).Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty);
            TQueryableSingle<T> _singleQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> Last<T>(this TQueryableOrdered<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty, exp);
            TQueryableSingle<T> _singleQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> LastOrDefault<T>(this TQueryableOrdered<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.Empty = tQuery.Empty.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.Empty, exp);
            TQueryableSingle<T> _singleQuery = new TQuery<T>() { Empty = tQuery.Empty, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryable<T> Signle<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> SignleOrDefault<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> Distinct<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }
        public static TQueryable<T> Union<T>(this TQueryable<T> tQuery, Func<T> predicate)
        {

            return tQuery;
        }

        public static TQueryable<T> Delete<T>(this TQueryable<T> tQuery)
        {
            //var sql = new ExpressionToSQL(predicate);

            return new TQuery<T>();// { SqlString = sql };
        }

        public static TQueryable<T> Create<T>(this TQueryable<T> tQuery)
        {
            if (tQuery.Empty == null) { tQuery.Empty = Enumerable.Empty<T>().AsQueryable(); }
            var table = tQuery.Empty.GetType().GenericTypeArguments[0];
            var props = table.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add(field.Name + " " + field.PropertyType.ToSqlDbTypeInternal()); }
            tQuery.SqlString = "CREATE TABLE " + table.Name+"("+Environment.NewLine + fields.Join(Environment.NewLine + ",") + ");";
            return tQuery;
        }

        public static TQueryable<T> Insert<T>(this TQueryable<T> tQuery, List<T> records)
        {
            return new TQuery<T>();
        }
        public static TQueryable<T> Drop<T>(this TQueryable<T> tQuery)
        {
            return new TQuery<T>();
        }

    }
}
