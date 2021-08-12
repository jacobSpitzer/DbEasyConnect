﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Dapper.TQuery.Library
{   
    public static class TQueryExtensions
    {
        public static TQueryable<T> TQuery<T>(this SqlConnection sqlConnection) 
        {
            TQueryable<T> queryable = new TQuery<T>(sqlConnection);
            return queryable;
        }
        public static TQueryableFilter<T> Where<T>(this TQueryable<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableFilter<T> filteredQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString};
            return filteredQuery;
        }
        public static TQueryableFilter<T> Where<T>(this TQueryableJoin<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            string join = tQuery.SqlString.IndexOf("JOIN LEFT") > 0 ? "JOIN LEFT" : tQuery.SqlString.IndexOf("JOIN RIGHT") > 0 ? "JOIN RIGHT" : "JOIN";
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, null, join);
            TQueryableFilter<T> filteredQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return filteredQuery;
        }
        //TODO need to add overload method, to order a grouped query. but need first to fix code. because the fields available for order is not grouped.
        public static TQueryableOrder<T> OrderBy<T,TKey>(this TQueryable<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.OrderBy(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<T> orderedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return orderedQuery;
        }
        public static TQueryableOrder<T> OrderBy<T, TKey>(this TQueryableFilter<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.OrderBy(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<T> orderedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return orderedQuery;
        }
        public static TQueryableOrder<T> ThenBy<T, TKey>(this TQueryableOrder<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            if (typeof(IOrderedQueryable<T>).IsAssignableFrom(tQuery.EmptyQuery.Expression.Type))
            {
                IOrderedQueryable<T> orderedQuery = (IOrderedQueryable<T>)tQuery.EmptyQuery;
                tQuery.EmptyQuery = orderedQuery.ThenBy(predicate);
            } else
            {
                tQuery.EmptyQuery = tQuery.EmptyQuery.OrderBy(predicate);
            }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<T> _orderedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }
        public static TQueryableOrder<T> OrderByDescending<T, TKey>(this TQueryable<T> tQuery, Func<T, TKey> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = (IQueryable<T>)tQuery.EmptyQuery.OrderByDescending(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<T> _orderedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }
        public static TQueryableOrder<T> OrderByDescending<T, TKey>(this TQueryableFilter<T> tQuery, Func<T, TKey> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = (IQueryable<T>)tQuery.EmptyQuery.OrderByDescending(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<T> _orderedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }
        public static TQueryableOrder<T> ThenByDescending<T, TKey>(this TQueryableOrder<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            if (typeof(IOrderedQueryable<T>).IsAssignableFrom(tQuery.EmptyQuery.Expression.Type))
            {
                IOrderedQueryable<T> orderedQuery = (IOrderedQueryable<T>)tQuery.EmptyQuery;
                tQuery.EmptyQuery = orderedQuery.ThenByDescending(predicate);
            }
            else
            {
                tQuery.EmptyQuery = tQuery.EmptyQuery.OrderByDescending(predicate);
            }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<T> _orderedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }
        public static TQueryableGroup<T> GroupBy<T, TKey>(this TQueryable<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.GroupBy(predicate).SelectMany(x=>x);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableGroup<T> _groupedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _groupedQuery;
        }
        public static TQueryableGroup<T> GroupBy<T, TKey>(this TQueryableFilter<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.GroupBy(predicate).SelectMany(x => x);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableGroup<T> _groupedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _groupedQuery;
        }
        public static TQueryableGroup<T> GroupBy<T, TKey>(this TQueryableOrder<T> tQuery, Expression<Func<T, TKey>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.GroupBy(predicate).SelectMany(x => x);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableGroup<T> _groupedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _groupedQuery;
        }
        public static TQueryable<T> Take<T>(this TQueryable<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);

            return tQuery;
        }
        public static TQueryable<T> Skip<T>(this TQueryable<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Skip(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);

            return tQuery;
        }
        public static TQueryable<T> Top<T>(this TQueryable<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }
        public static TQueryable<T> Bottom<T>(this TQueryable<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            var exp = ExpressionToSQL.Bottom(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery,exp);
            return tQuery;
        }
        public static TQueryableFilter<T> Take<T>(this TQueryableFilter<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);

            return tQuery;
        }
        public static TQueryableFilter<T> Skip<T>(this TQueryableFilter<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Skip(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);

            return tQuery;
        }
        public static TQueryableFilter<T> Top<T>(this TQueryableFilter<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }
        public static TQueryableFilter<T> Bottom<T>(this TQueryableFilter<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            var exp = ExpressionToSQL.Bottom(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            return tQuery;
        }
        public static TQueryableOrder<T> Take<T>(this TQueryableOrder<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);

            return tQuery;
        }
        public static TQueryableOrder<T> Skip<T>(this TQueryableOrder<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Skip(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);

            return tQuery;
        }
        public static TQueryableOrder<T> Top<T>(this TQueryableOrder<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }
        public static TQueryableOrder<T> Bottom<T>(this TQueryableOrder<T> tQuery, int predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            var exp = ExpressionToSQL.Bottom(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            return tQuery;
        }
        public static TQueryableUpdate<T> Update<T>(this TQueryable<T> tQuery, Expression<Func<T, T>> expression)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.PUpdate(expression));
            TQueryableUpdate<T> _updatedQuery = new TQueryExecute<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _updatedQuery;
        }
        public static TQueryableUpdate<T> Update<T>(this TQueryableFilter<T> tQuery, Expression<Func<T, T>> expression)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.PUpdate(expression));
            TQueryableUpdate<T> _updatedQuery = new TQueryExecute<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _updatedQuery;
        }
        public static TQueryableSelect<T> Select<T,TResult>(this TQueryable<T> tQuery, Expression<Func<T, TResult>> expression)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.PSelect(expression));
            TQueryableSelect<T> _selectedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _selectedQuery;
        }
        public static TQueryableSelect<T> Select<T, TResult>(this TQueryableOrder<T> tQuery, Expression<Func<T, TResult>> expression)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.PSelect(expression));
            TQueryableSelect<T> _selectedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _selectedQuery;
        }
        public static TQueryableSelect<T> Select<T, TResult>(this TQueryableFilter<T> tQuery, Expression<Func<T, TResult>> expression)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.PSelect(expression));
            TQueryableSelect<T> _selectedQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _selectedQuery;
        }
        public static TQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this TQueryable<TOuter> outer, TQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            if (outer.EmptyQuery == null) { outer.EmptyQuery = Enumerable.Empty<TOuter>().AsQueryable(); }
            IQueryable<TResult> empty = outer.EmptyQuery.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
            outer.SqlString = new ExpressionToSQL(empty, outer.EmptyQuery.Expression, "JOIN");
            TQueryable<TResult> _JoinQuery = new TQuery<TResult>(outer.SqlConnection) { EmptyQuery = empty, SqlString = outer.SqlString };
            return _JoinQuery;
        }
        public static TQueryableJoin<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this TQueryable<TOuter> outer, TQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            if (outer.EmptyQuery == null) { outer.EmptyQuery = Enumerable.Empty<TOuter>().AsQueryable(); }
            IQueryable<TResult> empty = outer.EmptyQuery.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
            outer.SqlString = new ExpressionToSQL(empty,null, "JOIN LEFT");
            TQueryableJoin<TResult> _JoinQuery = new TQuery<TResult>(outer.SqlConnection) { EmptyQuery = empty, SqlString = outer.SqlString };
            return _JoinQuery;
        }
        public static TQueryableJoin<TResult> RightJoin<TOuter, TInner, TKey, TResult>(this TQueryable<TOuter> outer, TQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            if (outer.EmptyQuery == null) { outer.EmptyQuery = Enumerable.Empty<TOuter>().AsQueryable(); }
            IQueryable<TResult> empty = outer.EmptyQuery.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
            outer.SqlString = new ExpressionToSQL(empty,null, "JOIN RIGHT");
            TQueryableJoin<TResult> _JoinQuery = new TQuery<TResult>(outer.SqlConnection) { EmptyQuery = empty, SqlString = outer.SqlString };
            return _JoinQuery;
        }
        public static TQueryableJoin<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this TQueryable<TOuter> outer, TQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            if (outer.EmptyQuery == null) { outer.EmptyQuery = Enumerable.Empty<TOuter>().AsQueryable(); }
            IQueryable<TResult> empty = outer.EmptyQuery.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
            outer.SqlString = new ExpressionToSQL(empty, outer.EmptyQuery.GroupBy(outerKeySelector).Expression, "JOIN");
            TQueryableJoin<TResult> _JoinQuery = new TQuery<TResult>(outer.SqlConnection) { EmptyQuery = empty, SqlString = outer.SqlString };
            return _JoinQuery;
        }
        //public static TQueryable<T> All<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> Any<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> Contains<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> Aggregate<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> Average<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> Count<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> Max<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> Sum<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> ElementAt<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> ElementAtOrDefault<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        public static TQueryableSingle<T> First<T>(this TQueryable<T> tQuery, Expression<Func<T, bool>> predicate)

        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(1).Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableSingle<T> _singleQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> FirstOrDefault<T>(this TQueryable<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(1).Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableSingle<T> _singleQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> Last<T>(this TQueryable<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery,exp);
            TQueryableSingle<T> _singleQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> LastOrDefault<T>(this TQueryable<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery,exp);
            TQueryableSingle<T> _singleQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> First<T>(this TQueryableOrder<T> tQuery, Expression<Func<T, bool>> predicate)

        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(1).Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableSingle<T> _singleQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> FirstOrDefault<T>(this TQueryableOrder<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(1).Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableSingle<T> _singleQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> Last<T>(this TQueryableOrder<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            TQueryableSingle<T> _singleQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        public static TQueryableSingle<T> LastOrDefault<T>(this TQueryableOrder<T> tQuery, Expression<Func<T, bool>> predicate)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            TQueryableSingle<T> _singleQuery = new TQuery<T>(tQuery.SqlConnection) { EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _singleQuery;
        }
        //public static TQueryable<T> Signle<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> SignleOrDefault<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> Distinct<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<T> Union<T>(this TQueryable<T> tQuery, Func<T> predicate)
        //{

        //    return tQuery;
        //}

        //public static TQueryable<T> Delete<T>(this TQueryable<T> tQuery)
        //{
        //    return tQuery;
        //}

        public static TQueryCreate CreateTable<T>(this TQueryable<T> tQuery)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            var props = table.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add(field.Name + " " + field.PropertyType.ToSqlDbTypeInternal()); }
            return new TQueryCreate(tQuery.SqlConnection) {
                SqlString = $"CREATE TABLE {table.Name} ({Environment.NewLine + fields.Join(Environment.NewLine + ",")});"
            };
        }

        public static TQueryCreate CreateTableIfNotExists<T>(this TQueryable<T> tQuery)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            var props = table.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add(field.Name + " " + field.PropertyType.ToSqlDbTypeInternal()); }
            return new TQueryCreate(tQuery.SqlConnection)
            {
                SqlString = $"IF OBJECT_ID('{table.Name}', 'U') IS NULL {Environment.NewLine}CREATE TABLE {table.Name} ({Environment.NewLine + fields.Join(Environment.NewLine + ",")});"
            };
        }

        public static TQueryCreate DropTable<T>(this TQueryable<T> tQuery)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return new TQueryCreate(tQuery.SqlConnection)
            {
                SqlString = $"DROP TABLE {table.Name}"
            };
        }

        public static TQueryCreate DropTableIfExists<T>(this TQueryable<T> tQuery)
        {
            if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<T>().AsQueryable(); }
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return new TQueryCreate(tQuery.SqlConnection)
            {
                SqlString = $"DROP TABLE IF EXISTS {table.Name}"
            };
        }

        //public static TQueryCreate DropExtraTableColumns<T>(this TQueryable<T> tQuery)
        //{
        //    //check if is missing columns
        //    //
        //}
        //public static TQueryCreate AddMissingTableColumns<T>(this TQueryable<T> tQuery)
        //{
        //    //check if is missing columns
        //    //
        //}
        //public static TQueryCreate ModifyTableColumnsDataType<T>(this TQueryable<T> tQuery)
        //{
        //    //check if is missing columns
        //    //
        //}
        //public static TQueryCreate RenameTableColumns<T>(this TQueryable<T> tQuery)
        //{
        //    //check if is missing columns
        //    //
        //}

        private static string CreateSql(this Type type)
        {
            var table = type.Name;
            var props = type.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add(field.Name + " " + field.PropertyType.ToSqlDbTypeInternal()); }
            return $"CREATE TABLE {table} ({Environment.NewLine + fields.Join(Environment.NewLine + ",")});{Environment.NewLine}";
        }
        private static string DropSql(this Type type)
        {
            var table = type.Name;
            var props = type.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add(field.Name + " " + field.PropertyType.ToSqlDbTypeInternal()); }
            return $"DROP TABLE {table} ;{Environment.NewLine}";
        }

        public static TQueryCreate CreateAllTables(this SqlConnection sqlConnection)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in Assembly
            .GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            TQueryCreate query = new TQueryCreate(sqlConnection);
            foreach (var t in types)
            {
                query.SqlString += t.CreateSql() + Environment.NewLine;
            }
            query.SqlConnection = sqlConnection;
            return query;
        }

        public static TQueryCreate DropAllTables(this SqlConnection sqlConnection)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in Assembly
            .GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            TQueryCreate query = new TQueryCreate(sqlConnection);
            foreach (var t in types)
            {
                query.SqlString += t.DropSql() + Environment.NewLine;
            }
            query.SqlConnection = sqlConnection;
            return query;
        }

        public static Dictionary<string, dynamic> GetAllDbTablesType(this SqlConnection sqlConnection)
        {
            sqlConnection.Open();
            DataTable t = sqlConnection.GetSchema("Tables");
            sqlConnection.Close();
            var tables = new Dictionary<string, object>();
            foreach (DataRow row in t.Rows)
            {
                string table = (string)row[2];
                sqlConnection.Open();
                var dtCols = sqlConnection.GetSchema("Columns", new[] { sqlConnection.Database, null, table });
                sqlConnection.Close();
                var fields = new List<Field>();
                foreach (DataRow field in dtCols.Rows)
                {
                    fields.Add(new Field((string)field[3], ExpressionToSQLExtensions.GetDNetType((string)field[7])));
                }
                object obj = new ClassBuilder(fields);
                tables.Add(table,obj);
            }
            return tables;
        }

        public static bool IsEqualServerDbToCodeDb(this SqlConnection sqlConnection)
        {
            sqlConnection.Open();
            DataTable t = sqlConnection.GetSchema("Tables");
            sqlConnection.Close();
            var types = new List<Type>();
            foreach (Type type in Assembly
            .GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0) 
                {
                    types.Add(type);
                }
            }
            foreach (DataRow row in t.Rows)
            {
                string table = (string)row[2];
                sqlConnection.Open();
                var dtCols = sqlConnection.GetSchema("Columns", new[] { sqlConnection.Database, null, table });
                sqlConnection.Close();
                var props = types.FirstOrDefault(x => x.Name == table).GetProperties().ToList();
                foreach (DataRow field in dtCols.Rows)
                {
                    var classProp = props.FirstOrDefault(x => x.Name == (string)field[3]);
                    var isEqual = classProp !=null && classProp.PropertyType == ExpressionToSQLExtensions.GetDNetType((string)field[7]);
                    if (!isEqual)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static List<CompareDb> GetDiffServerDbToCodeDb(this SqlConnection sqlConnection)
        {
            sqlConnection.Open();
            DataTable t = sqlConnection.GetSchema("Tables");
            sqlConnection.Close();
            var types = new List<Type>();
            foreach (Type type in Assembly
            .GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                {
                    types.Add(type);
                }
            }
            var diff = new List<CompareDb>();
            foreach (DataRow row in t.Rows)
            {
                string table = (string)row[2];
                sqlConnection.Open();
                var dtCols = sqlConnection.GetSchema("Columns", new[] { sqlConnection.Database, null, table });
                sqlConnection.Close();
                var props = types.FirstOrDefault(x => x.Name == table).GetProperties().ToList();
                foreach (DataRow field in dtCols.Rows)
                {
                    var serverType = ExpressionToSQLExtensions.GetDNetType((string)field[7]);
                    var classProp = props.FirstOrDefault(x => x.Name == (string)field[3]);
                    var isEqual = classProp != null && classProp.PropertyType == serverType;
                    if (!isEqual)
                    {
                        var namedesc = (string)field[3] == classProp.Name ? $"Field name '{(string)field[3]}' from server is different from '{classProp.Name}'" : null;
                        var typedesc = serverType == classProp.PropertyType ? $"Field type '{serverType.FullName}' from server is different from '{classProp.PropertyType.FullName}'" : null;
                        diff.Add(new CompareDb(table, (string)field[3], serverType,
                        classProp.Name, classProp.PropertyType,namedesc == null ? typedesc: typedesc == null ? namedesc: $"{namedesc} AND {typedesc}"));
                    }
                }
            }

            return diff;
        }

        public class CompareDb
        {
            public string TableName { get; set; }
            public string ServerField { get; set; }
            public Type ServerType { get; set; }
            public string CodeField { get; set; }
            public Type CodeType { get; set; }
            public string Description { get; set; }
            public CompareDb(string table,string serverFld,Type serverType,string codeFld,Type codeType, string desc)
            {
                TableName = table; ServerField = serverFld; ServerType = serverType; CodeField = codeFld;  CodeType = codeType; Description = desc;
            }
        }

    }
}
