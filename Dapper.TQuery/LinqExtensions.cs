using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Collections.Generic;
using DbEasyConnect.Tools;
using Dapper;

namespace DbEasyConnect.Linq
{
    /// <summary>
    /// Use <see href="https://www.nuget.org/packages/Dapper/">Dapper</see> with most of <see href="https://docs.microsoft.com/en-us/dotnet/api/system.linq">Linq</see> methods as an
    /// <see cref="IQueryable" /> object without downloading the records. <see href="https://stackoverflow.com/q/27563096/6509536">Dapper.NET and IQueryable
    /// issue</see>.
    /// </summary>
    public static class LinqExtensions
    {
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Filters a IDbEc recordset based on a predicate.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{Table}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each element for a condition.
        /// </param>
        /// <returns>
        /// An <see cref="DbEc{Table}"/> that contains records from the input sequence that
        /// satisfy the condition specified by predicate.
        /// </returns>
        public static DbEc<Table> Where<Table>(this DbEc<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Where(predicate);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery;
        }

        /// <summary>
        /// Correlates the records of two IDbEc recordsets based on matching keys.
        /// </summary>
        /// <param name="outer">
        /// The first <see cref="DbEc{T}"/> IDbEc recordset to join.
        /// </param>
        /// <param name="inner">The second <see cref="DbEc{T}"/> IDbEc recordset to join to the first IDbEc recordset.</param>
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
        /// A join type selected by the DbEasyConnect.JoinType enum. Available options: InnerJoin, LeftJoin, RightJoin, FullJoin. If no option is selected, The default will be InnerJoin.
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
        /// An <see cref="DbEc{T}"/> that has records of type TResult obtained by performing
        /// an inner join on two IDbEc recordsets.
        /// </returns>
        public static DbEc<TResult> Join<TOuter, TInner, TKey, TResult>(this DbEc<TOuter> outer, DbEc<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector, JoinType joinType = JoinType.InnerJoin)
        {
            if (outer.EmptyQuery == null) { outer.EmptyQuery = Enumerable.Empty<TOuter>().AsQueryable(); }
            IQueryable<TResult> empty = outer.EmptyQuery.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
            outer.SqlString = new ExpressionToSQL(empty, null, joinType.ToString().ToUpper().Replace("JOIN", " JOIN"));
            DbEc<TResult> _JoinQuery = new DbEc<TResult>(outer.SqlConnection, outer.SqlDialect) { EmptyQuery = empty, SqlString = outer.SqlString };
            return _JoinQuery;
        }

        /// <summary>
        /// Returns a specified number of contiguous records from the start of a IDbEc recordset.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="count">The number of records to return.</param>
        /// <returns>An <see cref="DbEc{T}"/> that contains the specified number of records from
        /// the start of IDbEc recordset.</returns>
        public static DbEc<Table> Take<Table>(this DbEc<Table> dbEcQuery, int count)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Take(count);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);

            return dbEcQuery;
        }

        /// <summary>
        /// Bypasses a specified number of records in a IDbEc recordset and then returns the remaining records.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="count">The number of records to skip before returning the remaining records.</param>
        /// <returns>An <see cref="DbEc{T}"/> that contains records that occur after the specified 
        /// index in the DbEasyConnect recordset.</returns>
        public static DbEc<Table> Skip<Table>(this DbEc<Table> dbEcQuery, int count)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Skip(count);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery;
        }

        /// <summary>
        /// Returns a specified number of contiguous records from the start of a IDbEc recordset.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="count">The number of records to return.</param>
        /// <returns>An <see cref="DbEc{T}"/> that contains the specified number of records from
        /// the start of IDbEc recordset.</returns>
        public static DbEc<Table> Top<Table>(this DbEc<Table> dbEcQuery, int count)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Take(count);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery;
        }

        /// <summary>
        /// Returns a specified number of contiguous records from the end of a IDbEc recordset.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="count">The number of records to return.</param>
        /// <returns>An <see cref="DbEc{T}"/> that contains the specified number of records from
        /// the end of IDbEc recordset.</returns>
        public static DbEc<Table> Bottom<Table>(this DbEc<Table> dbEcQuery, int count)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Reverse().Take(count);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery;
        }

        /// <summary>
        /// Sorts the records of a IDbEc recordset in ascending order according to a key.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> that contains the queryable table to apply the order predicate to.
        /// </param>
        /// <param name="keySelector">A function to extract a field from an IDbEc recordset</param>
        /// <returns>An <see cref="DbEcOrder{T}"/> whose records are sorted according to a key.</returns>
        public static DbEcOrder<Table> OrderBy<Table, TKey>(this DbEc<Table> dbEcQuery, Expression<Func<Table, TKey>> keySelector)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.OrderBy(keySelector);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            var orderedQuery = new DbEcOrder<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return orderedQuery;
        }

        /// <summary>
        /// Performs a subsequent ordering of a IDbEc recordset in ascending order according to a key.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcOrder{T}"/> that contains the ordered queryable table to apply the subsequent order predicate to.
        /// </param>
        /// <param name="keySelector">A function to extract a field from an IDbEc recordset</param>
        /// <returns>An <see cref="DbEcOrder{T}"/> whose records are sorted according to a key.</returns>
        public static DbEcOrder<Table> ThenBy<Table, TKey>(this DbEcOrder<Table> dbEcQuery, Expression<Func<Table, TKey>> keySelector)
        {
            // TODO add comment what is this if statement for
            if (typeof(IOrderedQueryable<Table>).IsAssignableFrom(dbEcQuery.EmptyQuery.Expression.Type))
            {
                IOrderedQueryable<Table> orderedQuery = (IOrderedQueryable<Table>)dbEcQuery.EmptyQuery;
                dbEcQuery.EmptyQuery = orderedQuery.ThenBy(keySelector);
            }
            else
            {
                dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.OrderBy(keySelector);
            }
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            DbEcOrder<Table> _orderedQuery = new DbEcOrder<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return _orderedQuery;
        }

        /// <summary>
        /// Sorts the records of a IDbEc recordset in descending order according to a key.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> that contains the queryable table to apply the order predicate to.
        /// </param>
        /// <param name="keySelector">A function to extract a field from an IDbEc recordset</param>
        /// <returns>An <see cref="DbEcOrder{T}"/> whose records are sorted according to a key.</returns>
        public static DbEcOrder<Table> OrderByDescending<Table, TKey>(this DbEc<Table> dbEcQuery, Func<Table, TKey> keySelector)
        {
            dbEcQuery.EmptyQuery = (IQueryable<Table>)dbEcQuery.EmptyQuery.OrderByDescending(keySelector);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            DbEcOrder<Table> _orderedQuery = new DbEcOrder<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return _orderedQuery;
        }

        /// <summary>
        /// Performs a subsequent ordering of a IDbEc recordset in descending order according to a key.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcOrder{T}"/> that contains the ordered queryable table to apply the subsequent order predicate to.
        /// </param>
        /// <param name="keySelector">A function to extract a field from an IDbEc recordset</param>
        /// <returns>An <see cref="DbEcOrder{T}"/> whose records are sorted according to a key.</returns>
        public static DbEcOrder<Table> ThenByDescending<Table, TKey>(this DbEcOrder<Table> dbEcQuery, Expression<Func<Table, TKey>> keySelector)
        {
            if (typeof(IOrderedQueryable<Table>).IsAssignableFrom(dbEcQuery.EmptyQuery.Expression.Type))
            {
                IOrderedQueryable<Table> orderedQuery = (IOrderedQueryable<Table>)dbEcQuery.EmptyQuery;
                dbEcQuery.EmptyQuery = orderedQuery.ThenByDescending(keySelector);
            }
            else
            {
                dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.OrderByDescending(keySelector);
            }
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            DbEcOrder<Table> _orderedQuery = new DbEcOrder<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return _orderedQuery;
        }

        /// <summary>
        /// Projects the DbEasyConnect recordset into a new form of a result table by selecting specific columns or calculations to retrieve from the table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TResult">The new form type of the fields and calculations returned by selector.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> that contains an queryable table to retrieve the new result type.
        /// </param>
        /// <param name="selector">
        /// An selection of the columns or calculations to be retrieved from the DbEasyConnect recordset.
        /// Example: dbEcQuery.(x=> new { FullName = x.FirstName + x.LastName, Id = x.Id, AfterTax = x.Total * 1.15 });
        /// </param>
        /// <returns>
        /// An <see cref="DbEcSelect{T}"/> whose records are projected into a new form of a record result.
        /// </returns>
        public static DbEcSelect<Table> Select<Table, TResult>(this DbEc<Table> dbEcQuery, Expression<Func<Table, TResult>> selector)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.Select(selector));
            DbEcSelect<Table> _selectedQuery = new DbEcSelect<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return _selectedQuery;
        }

        /// <summary>
        /// Projects the DbEasyConnect recordset into a new form of a result table by selecting specific columns or calculations to retrieve from the table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TResult">The new form type of the fields and calculations returned by selector.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcOrder{T}"/> that contains an ordered queryable table to retrieve the new result type.
        /// </param>
        /// <param name="selector">
        /// An selection of the columns or calculations to be retrieved from the DbEasyConnect recordset.
        /// Example: dbEcQuery.(x=> new { FullName = x.FirstName + x.LastName, Id = x.Id, AfterTax = x.Total * 1.15 });
        /// </param>
        /// <returns>
        /// An <see cref="DbEcSelect{T}"/> whose records are projected into a new form of a record result.
        /// </returns>
        public static DbEcSelect<Table> Select<Table, TResult>(this DbEcOrder<Table> dbEcQuery, Expression<Func<Table, TResult>> selector)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.Select(selector));
            DbEcSelect<Table> _selectedQuery = new DbEcSelect<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return _selectedQuery;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Determines whether all records of a IDbEc recordset satisfy a condition.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// true if every record of the DbEasyConnect recordset passes the test in the specified
        /// predicate, or if the recordset is empty; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// IDbEc or predicate is null.
        /// </exception>
        public static bool All<Table>(this DbEc<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.All(predicate));
            return dbEcQuery.SqlConnection.Query<bool>(dbEcQuery.SqlString).FirstOrDefault();
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Determines whether any record of a IDbEc recordset satisfy a condition.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// true if the DbEasyConnect recordset is not empty and at least one of its records passes
        /// the test in the specified predicate; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// IDbEc or predicate is null.
        /// </exception>
        public static bool Any<Table>(this DbEc<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.Any(predicate));
            return dbEcQuery.SqlConnection.Query<bool>(dbEcQuery.SqlString).FirstOrDefault();
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Determines whether a IDbEc recordset contains any records.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="dbEcQuery">
        /// The <see cref="DbEc{T}"/> to check for emptiness.
        /// </param>
        /// <returns>
        /// true if the DbEasyConnect recordset contains any records; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// IDbEc is null.
        /// </exception>
        public static bool Any<Table>(this DbEc<Table> dbEcQuery)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.ExistsAny(0));
            return dbEcQuery.SqlConnection.Query<bool>(dbEcQuery.SqlString).FirstOrDefault();
        }

        /// <summary>
        /// Determines whether all records of a IDbEc recordset satisfy a condition.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// An <see cref="DbEcBool{T}"/> instance, which the SQL command will return true if every record of the DbEasyConnect recordset passes the test in the specified
        /// predicate, or if the recordset is empty; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// IDbEc or predicate is null.
        /// </exception>
        public static DbEcBool<Table> All<Table>(this DbEcExtended<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.All(predicate));
            var boolQuery = new DbEcBool<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return boolQuery;
        }

        /// <summary>
        /// Determines whether any record of a IDbEc recordset satisfy a condition.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// An <see cref="DbEcBool{T}"/> instance, which the SQL command will return true if the DbEasyConnect recordset is not empty and at least one of its records passes
        /// the test in the specified predicate; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// IDbEc or predicate is null.
        /// </exception>
        public static DbEcBool<Table> Any<Table>(this DbEcExtended<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.Any(predicate));
            var boolQuery = new DbEcBool<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return boolQuery;
        }

        /// <summary>
        /// Determines whether a IDbEc recordset contains any records.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="dbEcQuery">
        /// The <see cref="DbEcExtended{T}"/> to check for emptiness.
        /// </param>
        /// <returns>
        /// An <see cref="DbEcBool{T}"/> instance, which the SQL command will return
        /// true if the DbEasyConnect recordset contains any records; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// IDbEc is null.
        /// </exception>
        public static DbEcBool<Table> Any<Table>(this DbEcExtended<Table> dbEcQuery)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.ExistsAny(0));
            var boolQuery = new DbEcBool<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return boolQuery;
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the DbEasyConnect recordset.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/>.
        /// </param>
        /// <returns>
        /// The first record in the DbEasyConnect recordset.
        /// </returns>
        public static Table First<Table>(this DbEc<Table> dbEcQuery)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QueryFirst<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the DbEasyConnect recordset.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcExtended{T}"/>.
        /// </param>
        /// <returns>
        /// The first record in the DbEasyConnect recordset.
        /// </returns>
        public static Table First<Table>(this DbEcExtended<Table> dbEcQuery)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QueryFirst<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the DbEasyConnect recordset that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// The first record in the DbEasyConnect recordset that passes the test in the specified predicate function.
        /// </returns>
        public static Table First<Table>(this DbEc<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Where(predicate);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QueryFirst<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the DbEasyConnect recordset that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// The first record in the DbEasyConnect recordset that passes the test in the specified predicate function.
        /// </returns>
        public static Table First<Table>(this DbEcExtended<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Where(predicate);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QueryFirst<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the DbEasyConnect recordset, or NULL if no record is found. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/>.
        /// </param>
        /// <returns>
        /// NULL if IDbEc recordset is empty or if no record is found; otherwise, the first record in IDbEc recordset.
        /// </returns>
        public static Table FirstOrDefault<Table>(this DbEc<Table> dbEcQuery)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QueryFirstOrDefault<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the DbEasyConnect recordset, or NULL if no record is found. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcExtended{T}"/>.
        /// </param>
        /// <returns>
        /// NULL if IDbEc recordset is empty or if no record is found; otherwise, the first record in IDbEc recordset.
        /// </returns>
        public static Table FirstOrDefault<Table>(this DbEcExtended<Table> dbEcQuery)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QueryFirstOrDefault<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the DbEasyConnect recordset that satisfies a specified condition, or NULL if no such record is found. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// NULL if IDbEc recordset is empty or if no record passes the test specified
        /// by predicate; otherwise, the first record in IDbEc recordset that passes the test specified
        /// by predicate.
        /// </returns>
        public static Table FirstOrDefault<Table>(this DbEc<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Where(predicate);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QueryFirstOrDefault<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Executes a single-row query, returning the first record of the DbEasyConnect recordset that satisfies a specified condition, or NULL if no such record is found. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// NULL if IDbEc recordset is empty or if no record passes the test specified
        /// by predicate; otherwise, the first record in IDbEc recordset that passes the test specified
        /// by predicate.
        /// </returns>
        public static Table FirstOrDefault<Table>(this DbEcExtended<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Where(predicate);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QueryFirstOrDefault<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the DbEasyConnect recordset, and throws an exception if there is not exactly one record. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> to apply the predicate to.
        /// </param>
        /// <returns>
        /// The single record in the DbEasyConnect recordset.
        /// </returns>
        public static Table Single<Table>(this DbEc<Table> dbEcQuery)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QuerySingle<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the DbEasyConnect recordset, and throws an exception if there is not exactly one record. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <returns>
        /// The single record in the DbEasyConnect recordset.
        /// </returns>
        public static Table Single<Table>(this DbEcExtended<Table> dbEcQuery)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QuerySingle<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the DbEasyConnect recordset that satisfies a specified condition, and throws an exception if there is not exactly one record that satisfies the predicate function. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// The single record in the DbEasyConnect recordset that passes the test in the specified predicate function.
        /// </returns>
        public static Table Single<Table>(this DbEc<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Where(predicate);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QuerySingle<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the DbEasyConnect recordset that satisfies a specified condition, and throws an exception if there is not exactly one record that satisfies the predicate function. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// The single record in the DbEasyConnect recordset that passes the test in the specified predicate function.
        /// </returns>
        public static Table Single<Table>(this DbEcExtended<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Where(predicate);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QuerySingle<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the DbEasyConnect recordset, or NULL if no such record is found, and throws an exception if there is not exactly one record. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> to apply the predicate to.
        /// </param>
        /// <returns>
        /// NULL if IDbEc recordset is empty or if no record found; otherwise, the single record in IDbEc recordset.
        /// </returns>
        public static Table SingleOrDefault<Table>(this DbEc<Table> dbEcQuery)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QuerySingleOrDefault<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the DbEasyConnect recordset, or NULL if no such record is found, and throws an exception if there is not exactly one record. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <returns>
        /// NULL if IDbEc recordset is empty or if no record found; otherwise, the single record in IDbEc recordset.
        /// </returns>
        public static Table SingleOrDefault<Table>(this DbEcExtended<Table> dbEcQuery)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QuerySingleOrDefault<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the DbEasyConnect recordset that satisfies a specified condition, or NULL if no such record is found, and throws an exception if there is not exactly one record that satisfies the predicate function. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// NULL if IDbEc recordset is empty or if no record passes the test specified
        /// by predicate; otherwise, the single record in IDbEc recordset that passes the test specified
        /// by predicate.
        /// </returns>
        public static Table SingleOrDefault<Table>(this DbEc<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Where(predicate);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QuerySingleOrDefault<Table>(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Returning the only record of the DbEasyConnect recordset that satisfies a specified condition, or NULL if no such record is found, and throws an exception if there is not exactly one record that satisfies the predicate function. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEcExtended{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each record for a condition.
        /// </param>
        /// <returns>
        /// NULL if IDbEc recordset is empty or if no record passes the test specified
        /// by predicate; otherwise, the single record in IDbEc recordset that passes the test specified
        /// by predicate.
        /// </returns>
        public static Table SingleOrDefault<Table>(this DbEcExtended<Table> dbEcQuery, Expression<Func<Table, bool>> predicate)
        {
            dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.Where(predicate);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
            return dbEcQuery.SqlConnection.QuerySingleOrDefault<Table>(dbEcQuery.SqlString);
        }
    }

}
