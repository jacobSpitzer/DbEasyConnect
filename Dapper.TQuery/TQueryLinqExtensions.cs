using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Collections.Generic;

namespace Dapper.TQuery
{
    /// <summary>
    /// Use <see href="https://www.nuget.org/packages/Dapper/">Dapper</see> with most of <see href="https://docs.microsoft.com/en-us/dotnet/api/system.linq">Linq</see> methods as an
    /// <see cref="IQueryable" /> object without downloading the records. <see href="https://stackoverflow.com/q/27563096/6509536">Dapper.NET and IQueryable
    /// issue</see>.
    /// </summary>
    public static class TQueryLinqExtensions
    {
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Filters a TQuery recordset based on a predicate.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{Table}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each element for a condition.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryable{Table}"/> that contains records from the input sequence that
        /// satisfy the condition specified by predicate.
        /// </returns>
        public static TQueryable<Table> Where<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }

        /// <summary>
        /// Correlates the records of two TQuery recordsets based on matching keys.
        /// </summary>
        /// <param name="outer">
        /// The first <see cref="TQueryable{T}"/> TQuery recordset to join.
        /// </param>
        /// <param name="inner">The second <see cref="TQueryable{T}"/> TQuery recordset to join to the first TQuery recordset.</param>
        /// <param name="outerKeySelector">
        /// A function to extract the join key from each record of the first recordset.
        /// </param>
        /// <param name="innerKeySelector">
        /// A function to extract the join key from each record of the second recordset.
        /// </param>
        /// <param name="resultSelector">
        /// A function to create a result record from two joined table records.
        /// </param>
        /// <param name="joinType">
        /// A join type selected by the Dapper.TQuery.JoinType enum. Available options: InnerJoin, LeftJoin, RightJoin, FullJoin. If no option is selected, The default will be InnerJoin.
        /// </param>
        /// <typeparam name="TOuter">
        /// The table type of the records of the first table.
        /// </typeparam>
        /// <typeparam name="TInner">
        /// The table type of the records of the second/joined table.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the keys returned by the key selector functions.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result records.
        /// </typeparam>
        /// <returns>
        /// An <see cref="TQueryable{T}"/> that has records of type TResult obtained by performing
        /// an inner join on two TQuery recordsets.
        /// </returns>
        public static TQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this TQueryable<TOuter> outer, TQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector, JoinType joinType = JoinType.InnerJoin)
        {
            if (outer.EmptyQuery == null) { outer.EmptyQuery = Enumerable.Empty<TOuter>().AsQueryable(); }
            IQueryable<TResult> empty = outer.EmptyQuery.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
            outer.SqlString = new ExpressionToSQL(empty, null, joinType.ToString().ToUpper().Replace("JOIN", " JOIN"));
            TQueryable<TResult> _JoinQuery = new TQueryable<TResult>(outer.SqlConnection, outer.SqlDialect) { EmptyQuery = empty, SqlString = outer.SqlString };
            return _JoinQuery;
        }

        /// <summary>
        /// Returns a specified number of contiguous records from the start of a TQuery recordset.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="count">The number of records to return.</param>
        /// <returns>An <see cref="TQueryable{T}"/> that contains the specified number of records from
        /// the start of TQuery recordset.</returns>
        public static TQueryable<Table> Take<Table>(this TQueryable<Table> tQuery, int count)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(count);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);

            return tQuery;
        }

        /// <summary>
        /// Bypasses a specified number of records in a TQuery recordset and then returns the remaining records.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="count">The number of records to skip before returning the remaining records.</param>
        /// <returns>An <see cref="TQueryable{T}"/> that contains records that occur after the specified 
        /// index in the TQuery recordset.</returns>
        public static TQueryable<Table> Skip<Table>(this TQueryable<Table> tQuery, int count)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Skip(count);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }

        /// <summary>
        /// Returns a specified number of contiguous records from the start of a TQuery recordset.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="count">The number of records to return.</param>
        /// <returns>An <see cref="TQueryable{T}"/> that contains the specified number of records from
        /// the start of TQuery recordset.</returns>
        public static TQueryable<Table> Top<Table>(this TQueryable<Table> tQuery, int count)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(count);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }

        /// <summary>
        /// Returns a specified number of contiguous records from the end of a TQuery recordset.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="count">The number of records to return.</param>
        /// <returns>An <see cref="TQueryable{T}"/> that contains the specified number of records from
        /// the end of TQuery recordset.</returns>
        public static TQueryable<Table> Bottom<Table>(this TQueryable<Table> tQuery, int count)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Reverse().Take(count);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }

        /// <summary>
        /// Sorts the records of a TQuery recordset in ascending order according to a key.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> that contains the queryable table to apply the order predicate to.
        /// </param>
        /// <param name="keySelector">A function to extract a field from an TQuery recordset</param>
        /// <returns>An <see cref="TQueryableOrder{T}"/> whose records are sorted according to a key.</returns>
        public static TQueryableOrder<Table> OrderBy<Table, TKey>(this TQueryable<Table> tQuery, Expression<Func<Table, TKey>> keySelector)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.OrderBy(keySelector);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            var orderedQuery = new TQueryableOrder<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return orderedQuery;
        }

        /// <summary>
        /// Performs a subsequent ordering of a TQuery recordset in ascending order according to a key.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableOrder{T}"/> that contains the ordered queryable table to apply the subsequent order predicate to.
        /// </param>
        /// <param name="keySelector">A function to extract a field from an TQuery recordset</param>
        /// <returns>An <see cref="TQueryableOrder{T}"/> whose records are sorted according to a key.</returns>
        public static TQueryableOrder<Table> ThenBy<Table, TKey>(this TQueryableOrder<Table> tQuery, Expression<Func<Table, TKey>> keySelector)
        {
            // TODO add comment what is this if statement for
            if (typeof(IOrderedQueryable<Table>).IsAssignableFrom(tQuery.EmptyQuery.Expression.Type))
            {
                IOrderedQueryable<Table> orderedQuery = (IOrderedQueryable<Table>)tQuery.EmptyQuery;
                tQuery.EmptyQuery = orderedQuery.ThenBy(keySelector);
            }
            else
            {
                tQuery.EmptyQuery = tQuery.EmptyQuery.OrderBy(keySelector);
            }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<Table> _orderedQuery = new TQueryableOrder<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }

        /// <summary>
        /// Sorts the records of a TQuery recordset in descending order according to a key.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> that contains the queryable table to apply the order predicate to.
        /// </param>
        /// <param name="keySelector">A function to extract a field from an TQuery recordset</param>
        /// <returns>An <see cref="TQueryableOrder{T}"/> whose records are sorted according to a key.</returns>
        public static TQueryableOrder<Table> OrderByDescending<Table, TKey>(this TQueryable<Table> tQuery, Func<Table, TKey> keySelector)
        {
            tQuery.EmptyQuery = (IQueryable<Table>)tQuery.EmptyQuery.OrderByDescending(keySelector);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<Table> _orderedQuery = new TQueryableOrder<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }

        /// <summary>
        /// Performs a subsequent ordering of a TQuery recordset in descending order according to a key.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableOrder{T}"/> that contains the ordered queryable table to apply the subsequent order predicate to.
        /// </param>
        /// <param name="keySelector">A function to extract a field from an TQuery recordset</param>
        /// <returns>An <see cref="TQueryableOrder{T}"/> whose records are sorted according to a key.</returns>
        public static TQueryableOrder<Table> ThenByDescending<Table, TKey>(this TQueryableOrder<Table> tQuery, Expression<Func<Table, TKey>> keySelector)
        {
            if (typeof(IOrderedQueryable<Table>).IsAssignableFrom(tQuery.EmptyQuery.Expression.Type))
            {
                IOrderedQueryable<Table> orderedQuery = (IOrderedQueryable<Table>)tQuery.EmptyQuery;
                tQuery.EmptyQuery = orderedQuery.ThenByDescending(keySelector);
            }
            else
            {
                tQuery.EmptyQuery = tQuery.EmptyQuery.OrderByDescending(keySelector);
            }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<Table> _orderedQuery = new TQueryableOrder<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }

        /// <summary>
        /// Projects the TQuery recordset into a new form of a result table by selecting specific columns or calculations to retrieve from the table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TResult">The new form type of the fields and calculations returned by selector.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> that contains an queryable table to retrieve the new result type.
        /// </param>
        /// <param name="selector">
        /// An selection of the columns or calculations to be retrieved from the TQuery recordset.
        /// Example: tQuery.(x=> new { FullName = x.FirstName + x.LastName, Id = x.Id, AfterTax = x.Total * 1.15 });
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableSelect{T}"/> whose records are projected into a new form of a record result.
        /// </returns>
        public static TQueryableSelect<Table> Select<Table, TResult>(this TQueryable<Table> tQuery, Expression<Func<Table, TResult>> selector)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Select(selector));
            TQueryableSelect<Table> _selectedQuery = new TQueryableSelect<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _selectedQuery;
        }

        /// <summary>
        /// Projects the TQuery recordset into a new form of a result table by selecting specific columns or calculations to retrieve from the table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TResult">The new form type of the fields and calculations returned by selector.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableOrder{T}"/> that contains an ordered queryable table to retrieve the new result type.
        /// </param>
        /// <param name="selector">
        /// An selection of the columns or calculations to be retrieved from the TQuery recordset.
        /// Example: tQuery.(x=> new { FullName = x.FirstName + x.LastName, Id = x.Id, AfterTax = x.Total * 1.15 });
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableSelect{T}"/> whose records are projected into a new form of a record result.
        /// </returns>
        public static TQueryableSelect<Table> Select<Table, TResult>(this TQueryableOrder<Table> tQuery, Expression<Func<Table, TResult>> selector)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Select(selector));
            TQueryableSelect<Table> _selectedQuery = new TQueryableSelect<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _selectedQuery;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Determines whether all records of a TQuery recordset satisfy a condition.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// true if every record of the TQuery recordset passes the test in the specified
        /// predicate, or if the recordset is empty; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// TQuery or predicate is null.
        /// </exception>
        public static bool All<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.All(predicate));
            return tQuery.SqlConnection.Query<bool>(tQuery.SqlString).FirstOrDefault();
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Determines whether any record of a TQuery recordset satisfy a condition.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// true if the TQuery recordset is not empty and at least one of its records passes
        /// the test in the specified predicate; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// TQuery or predicate is null.
        /// </exception>
        public static bool Any<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Any(predicate));
            return tQuery.SqlConnection.Query<bool>(tQuery.SqlString).FirstOrDefault();
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Determines whether a TQuery recordset contains any records.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="tQuery">
        /// The <see cref="TQueryable{T}"/> to check for emptiness.
        /// </param>
        /// <returns>
        /// true if the TQuery recordset contains any records; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// TQuery is null.
        /// </exception>
        public static bool Any<Table>(this TQueryable<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.ExistsAny(0));
            return tQuery.SqlConnection.Query<bool>(tQuery.SqlString).FirstOrDefault();
        }

        /// <summary>
        /// Determines whether all records of a TQuery recordset satisfy a condition.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableBool{T}"/> instance, which the SQL command will return true if every record of the TQuery recordset passes the test in the specified
        /// predicate, or if the recordset is empty; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// TQuery or predicate is null.
        /// </exception>
        public static TQueryableBool<Table> All<Table>(this TQueryableExtended<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.All(predicate));
            var boolQuery = new TQueryableBool<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return boolQuery;
        }

        /// <summary>
        /// Determines whether any record of a TQuery recordset satisfy a condition.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableBool{T}"/> instance, which the SQL command will return true if the TQuery recordset is not empty and at least one of its records passes
        /// the test in the specified predicate; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// TQuery or predicate is null.
        /// </exception>
        public static TQueryableBool<Table> Any<Table>(this TQueryableExtended<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Any(predicate));
            var boolQuery = new TQueryableBool<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return boolQuery;
        }

        /// <summary>
        /// Determines whether a TQuery recordset contains any records.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="tQuery">
        /// The <see cref="TQueryableExtended{T}"/> to check for emptiness.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableBool{T}"/> instance, which the SQL command will return
        /// true if the TQuery recordset contains any records; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// TQuery is null.
        /// </exception>
        public static TQueryableBool<Table> Any<Table>(this TQueryableExtended<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.ExistsAny(0));
            var boolQuery = new TQueryableBool<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return boolQuery;
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the TQuery recordset.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/>.
        /// </param>
        /// <returns>
        /// The first record in the TQuery recordset.
        /// </returns>
        public static Table First<Table>(this TQueryable<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirst<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the TQuery recordset.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/>.
        /// </param>
        /// <returns>
        /// The first record in the TQuery recordset.
        /// </returns>
        public static Table First<Table>(this TQueryableExtended<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirst<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the TQuery recordset that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// The first record in the TQuery recordset that passes the test in the specified predicate function.
        /// </returns>
        public static Table First<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirst<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the TQuery recordset that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// The first record in the TQuery recordset that passes the test in the specified predicate function.
        /// </returns>
        public static Table First<Table>(this TQueryableExtended<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirst<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the TQuery recordset, or NULL if no record is found. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/>.
        /// </param>
        /// <returns>
        /// NULL if TQuery recordset is empty or if no record is found; otherwise, the first record in TQuery recordset.
        /// </returns>
        public static Table FirstOrDefault<Table>(this TQueryable<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirstOrDefault<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the TQuery recordset, or NULL if no record is found. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/>.
        /// </param>
        /// <returns>
        /// NULL if TQuery recordset is empty or if no record is found; otherwise, the first record in TQuery recordset.
        /// </returns>
        public static Table FirstOrDefault<Table>(this TQueryableExtended<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirstOrDefault<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the TQuery recordset that satisfies a specified condition, or NULL if no such record is found. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// NULL if TQuery recordset is empty or if no record passes the test specified
        /// by predicate; otherwise, the first record in TQuery recordset that passes the test specified
        /// by predicate.
        /// </returns>
        public static Table FirstOrDefault<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirstOrDefault<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the TQuery recordset that satisfies a specified condition, or NULL if no such record is found. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// NULL if TQuery recordset is empty or if no record passes the test specified
        /// by predicate; otherwise, the first record in TQuery recordset that passes the test specified
        /// by predicate.
        /// </returns>
        public static Table FirstOrDefault<Table>(this TQueryableExtended<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirstOrDefault<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the TQuery recordset, and throws an exception if there is not exactly one record. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to apply the predicate to.
        /// </param>
        /// <returns>
        /// The single record in the TQuery recordset.
        /// </returns>
        public static Table Single<Table>(this TQueryable<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingle<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the TQuery recordset, and throws an exception if there is not exactly one record. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <returns>
        /// The single record in the TQuery recordset.
        /// </returns>
        public static Table Single<Table>(this TQueryableExtended<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingle<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the TQuery recordset that satisfies a specified condition, and throws an exception if there is not exactly one record that satisfies the predicate function. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// The single record in the TQuery recordset that passes the test in the specified predicate function.
        /// </returns>
        public static Table Single<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingle<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the TQuery recordset that satisfies a specified condition, and throws an exception if there is not exactly one record that satisfies the predicate function. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// The single record in the TQuery recordset that passes the test in the specified predicate function.
        /// </returns>
        public static Table Single<Table>(this TQueryableExtended<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingle<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the TQuery recordset, or NULL if no such record is found, and throws an exception if there is not exactly one record. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to apply the predicate to.
        /// </param>
        /// <returns>
        /// NULL if TQuery recordset is empty or if no record found; otherwise, the single record in TQuery recordset.
        /// </returns>
        public static Table SingleOrDefault<Table>(this TQueryable<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingleOrDefault<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the TQuery recordset, or NULL if no such record is found, and throws an exception if there is not exactly one record. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <returns>
        /// NULL if TQuery recordset is empty or if no record found; otherwise, the single record in TQuery recordset.
        /// </returns>
        public static Table SingleOrDefault<Table>(this TQueryableExtended<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingleOrDefault<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the TQuery recordset that satisfies a specified condition, or NULL if no such record is found, and throws an exception if there is not exactly one record that satisfies the predicate function. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// NULL if TQuery recordset is empty or if no record passes the test specified
        /// by predicate; otherwise, the single record in TQuery recordset that passes the test specified
        /// by predicate.
        /// </returns>
        public static Table SingleOrDefault<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingleOrDefault<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the TQuery recordset that satisfies a specified condition, or NULL if no such record is found, and throws an exception if there is not exactly one record that satisfies the predicate function. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// NULL if TQuery recordset is empty or if no record passes the test specified
        /// by predicate; otherwise, the single record in TQuery recordset that passes the test specified
        /// by predicate.
        /// </returns>
        public static Table SingleOrDefault<Table>(this TQueryableExtended<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingleOrDefault<Table>(tQuery.SqlString);
        }
    }

}
