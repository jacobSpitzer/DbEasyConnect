using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dapper.TQuery.Development
{
    public static class TQueryExtensions
    {
        //TODO add settings arg to con methods, like CreateAllTables 'SqlDialect sqlDialect = SqlDialect.MsSql, TQueryResultType queryResultType = TQueryResultType.Linq'
        /// <summary>
        /// Initializes a new instance of the Dapper.TQuery.TQueryable`1 class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The System.Data.SqlClient.SqlConnection`1 to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant Dapper.TQuery.SqlDialect`1 for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// If no dialect was given, the default dialect 'SqlDialect.SqlServer' will be used.
        /// </param>
        /// <returns>
        /// An Dapper.TQuery.TQueryable`1 instanse which will be used to query and/or modify the table with TQuery method extensions.
        /// </returns>
        /// <exception cref=""></exception>
        ///         
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        public static TQueryable<Table> TQuery<Table>(this SqlConnection sqlConnection, SqlDialect sqlDialect = SqlDialect.SqlServer)
        {
            TQueryable<Table> query = new TQueryable<Table>(sqlConnection,sqlDialect);
            return query;
        }

        /// <summary>
        /// Initializes a new instance of the Dapper.TQuery.TQueryableSql`1 class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The System.Data.SqlClient.SqlConnection`1 to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant Dapper.TQuery.SqlDialect`1 for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// If no dialect was given, the default dialect 'SqlDialect.SqlServer' will be used.
        /// </param>
        /// <returns>
        /// An Dapper.TQuery.TQueryableSql`1 instanse which will be used to query and/or modify the table with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        /// <exception cref=""></exception>
        ///         
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        public static TQueryableSql<Table> TQuerySql<Table>(this SqlConnection sqlConnection, SqlDialect sqlDialect = SqlDialect.SqlServer)
        {            
            TQueryableSql<Table> query = new TQueryableSql<Table>(sqlConnection,sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Filters a TQuery recordset based on a predicate.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An Dapper.TQuery.TQueryable`1 that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each element for a condition.
        /// </param>
        /// <returns>
        /// An Dapper.TQuery.TQueryable`1 that contains records from the input sequence that
        /// satisfy the condition specified by predicate.
        /// </returns>
        public static TQueryable<Table> Where<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }

        /// <summary>
        /// Correlates the records of two TQuery recordsets based on matching keys. A specified
        /// System.Collections.Generic.IEqualityComparer`1 is used to compare keys.
        /// </summary>
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
        /// <param name="outer">
        /// The first TQuery recordset to join.
        /// </param>
        /// <param name="inner">The TQuery recordset to join to the first TQuery recordset.</param>
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
        /// <returns>
        /// An Dapper.TQuery.TQueryable`1 that has records of type TResult obtained by performing
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
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryable<Table> Take<Table>(this TQueryable<Table> tQuery, int predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);

            return tQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryable<Table> Skip<Table>(this TQueryable<Table> tQuery, int predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Skip(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryable<Table> Top<Table>(this TQueryable<Table> tQuery, int predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryable<Table> Bottom<Table>(this TQueryable<Table> tQuery, int predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Reverse().Take(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryableOrder<Table> OrderBy<Table, TKey>(this TQueryable<Table> tQuery, Expression<Func<Table, TKey>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.OrderBy(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            var orderedQuery = new TQueryableOrder<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return orderedQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryableOrder<Table> ThenBy<Table, TKey>(this TQueryableOrder<Table> tQuery, Expression<Func<Table, TKey>> predicate)
        {
            if (typeof(IOrderedQueryable<Table>).IsAssignableFrom(tQuery.EmptyQuery.Expression.Type))
            {
                IOrderedQueryable<Table> orderedQuery = (IOrderedQueryable<Table>)tQuery.EmptyQuery;
                tQuery.EmptyQuery = orderedQuery.ThenBy(predicate);
            } else
            {
                tQuery.EmptyQuery = tQuery.EmptyQuery.OrderBy(predicate);
            }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<Table> _orderedQuery = new TQueryableOrder<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryableOrder<Table> OrderByDescending<Table, TKey>(this TQueryable<Table> tQuery, Func<Table, TKey> predicate)
        {
            tQuery.EmptyQuery = (IQueryable<Table>)tQuery.EmptyQuery.OrderByDescending(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<Table> _orderedQuery = new TQueryableOrder<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryableOrder<Table> ThenByDescending<Table, TKey>(this TQueryableOrder<Table> tQuery, Expression<Func<Table, TKey>> predicate)
        {
            if (typeof(IOrderedQueryable<Table>).IsAssignableFrom(tQuery.EmptyQuery.Expression.Type))
            {
                IOrderedQueryable<Table> orderedQuery = (IOrderedQueryable<Table>)tQuery.EmptyQuery;
                tQuery.EmptyQuery = orderedQuery.ThenByDescending(predicate);
            }
            else
            {
                tQuery.EmptyQuery = tQuery.EmptyQuery.OrderByDescending(predicate);
            }
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            TQueryableOrder<Table> _orderedQuery = new TQueryableOrder<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _orderedQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tQuery"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static TQueryableSelect<Table> Select<Table, TResult>(this TQueryable<Table> tQuery, Expression<Func<Table, TResult>> expression)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Select(expression));
            TQueryableSelect<Table> _selectedQuery = new TQueryableSelect<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _selectedQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tQuery"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static TQueryableSelect<Table> Select<Table, TResult>(this TQueryableOrder<Table> tQuery, Expression<Func<Table, TResult>> expression)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Select(expression));
            TQueryableSelect<Table> _selectedQuery = new TQueryableSelect<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _selectedQuery;
        }

        ///
        /// Summary:
        ///     Determines whether all records of a TQuery recordset satisfy a condition.
        ///
        /// Parameters:
        ///   tQuery:
        ///     An Dapper.TQuery.TQueryable`1 that contains the queryable table to apply
        ///     the predicate to.
        ///
        ///   predicate:
        ///     A function to test each record for a condition.
        ///
        /// Type parameters:
        ///   Table:
        ///     The type of the records of table class. need to be a class with the [Table("")] attribute.
        ///
        /// Returns:
        ///     true if every record of the TQuery recordset passes the test in the specified
        ///     predicate, or if the recordset is empty; otherwise, false.
        ///
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     TQuery or predicate is null.
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        public static bool All<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.All(predicate));
            return tQuery.SqlConnection.Query<bool>(tQuery.SqlString).FirstOrDefault();
        }

        /// <summary>
        /// Determines whether any record of a TQuery recordset satisfy a condition.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="tQuery">
        /// An Dapper.TQuery.TQueryable`1 whose recordss to apply the predicate to.
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
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        public static bool Any<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Any(predicate));
            return tQuery.SqlConnection.Query<bool>(tQuery.SqlString).FirstOrDefault();
        }

        /// <summary>
        /// Determines whether a TQuery recordset contains any records.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table("")] attribute.
        /// </typeparam>
        /// <param name="tQuery">
        /// The Dapper.TQuery.TQueryable`1 to check for emptiness.
        /// </param>
        /// <returns>
        /// true if the TQuery recordset contains any records; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// TQuery is null.
        /// </exception>
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        public static bool Any<Table>(this TQueryable<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.ExistsAny(0));
            return tQuery.SqlConnection.Query<bool>(tQuery.SqlString).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryableBool<Table> All<Table>(this TQueryableSql<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.All(predicate));
            var boolQuery = new TQueryableBool<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return boolQuery;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryableBool<Table> Any<Table>(this TQueryableSql<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Any(predicate));
            var boolQuery = new TQueryableBool<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return boolQuery;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static TQueryableBool<Table> Any<Table>(this TQueryableSql<Table> tQuery)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.ExistsAny(0));
            var boolQuery = new TQueryableBool<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return boolQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table First<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirst<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table FirstOrDefault<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirstOrDefault<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table Last<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            return tQuery.SqlConnection.QuerySingle<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table LastOrDefault<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            return tQuery.SqlConnection.QuerySingleOrDefault<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table Single<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingle<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table SingleOrDefault<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingleOrDefault<Table>(tQuery.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table First<Table>(this TQueryableSql<Table> tQuery, Expression<Func<Table, bool>> predicate)

        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirst<Table>(tQuery.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table FirstOrDefault<Table>(this TQueryableSql<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QueryFirstOrDefault<Table>(tQuery.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table Last<Table>(this TQueryableSql<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            return tQuery.SqlConnection.QuerySingle<Table>(tQuery.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table LastOrDefault<Table>(this TQueryableSql<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            var exp = ExpressionToSQL.Bottom(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            return tQuery.SqlConnection.QuerySingleOrDefault<Table>(tQuery.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table Single<Table>(this TQueryableSql<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingle<Table>(tQuery.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Table SingleOrDefault<Table>(this TQueryableSql<Table> tQuery, Expression<Func<Table, bool>> predicate)
        {
            tQuery.EmptyQuery = tQuery.EmptyQuery.Where(predicate);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
            return tQuery.SqlConnection.QuerySingleOrDefault<Table>(tQuery.SqlString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static int Update<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, Table>> expression)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Update(expression));
            return tQuery.SqlConnection.Execute(tQuery.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static TQueryableUpdate<Table> Update<Table>(this TQueryableSql<Table> tQuery, Expression<Func<Table, Table>> expression)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Update(expression));
            TQueryableUpdate<Table> _updatedQuery = new TQueryableUpdate<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _updatedQuery;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static int Delete<Table>(this TQueryable<Table> tQuery)
        {
            var exp = ExpressionToSQL.Delete(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            return tQuery.SqlConnection.Execute(tQuery.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static TQueryableDelete<Table> Delete<Table>(this TQueryableSql<Table> tQuery)
        {
            var exp = ExpressionToSQL.Delete(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            var deleteQuery = new TQueryableDelete<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return deleteQuery;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="entity"></param>
        public static void Insert<Table>(this TQueryable<Table> tQuery, Table entity)
        {
            List<Table> entities = new List<Table>();
            entities.Add(entity);
            tQuery.InsertList(entities);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        public static void Update<Table>(this TQueryable<Table> tQuery, Table entity, string keyColumnName = "Id")
        {
            List<Table> entities = new List<Table>();
            entities.Add(entity);
            tQuery.UpdateList(entities,keyColumnName);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        public static void Delete<Table>(this TQueryable<Table> tQuery, Table entity, string keyColumnName = "Id")
        {
            List<Table> entities = new List<Table>();
            entities.Add(entity);
            tQuery.DeleteList(entities, keyColumnName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="entities"></param>
        public static void InsertList<Table>(this TQueryable<Table> tQuery, List<Table> entities)
        {
            using (var copy = new SqlBulkCopy(tQuery.SqlConnection))
            {
                copy.DestinationTableName = typeof(Table).Name;
                tQuery.SqlConnection.Open();
                copy.WriteToServer(ToDataTable(entities));
                tQuery.SqlConnection.Close();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="entities"></param>
        /// <param name="keyColumnName"></param>
        public static void UpdateList<Table>(this TQueryable<Table> tQuery, List<Table> entities, string keyColumnName = "Id")
        {
            if (typeof(Table).GetProperty(keyColumnName) == null)
            {
                throw new NullReferenceException($"Missing column name '{keyColumnName}' in table '{typeof(Table).Name}'.");
            }
            var tableName = typeof(Table).Name;
            tQuery.SqlConnection.Open();
            tQuery.SqlConnection.TQuery<Table>().CreateTempTable().Execute();
            using (var copy = new SqlBulkCopy(tQuery.SqlConnection))
            {
                copy.DestinationTableName = $"#{tableName}Temp";
                copy.WriteToServer(ToDataTable(entities));
                var sql = tQuery.UpdateTableFromTempSql(keyColumnName).SqlString;
                var cmd = new SqlCommand(sql, tQuery.SqlConnection);
                cmd.ExecuteNonQuery();
            }
            tQuery.SqlConnection.Close();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <param name="entities"></param>
        /// <param name="keyColumnName"></param>
        public static void DeleteList<Table>(this TQueryable<Table> tQuery, List<Table> entities, string keyColumnName = "Id")
        {
            if (typeof(Table).GetProperty(keyColumnName) == null)
            {
                throw new NullReferenceException($"Missing column name '{keyColumnName}' in table '{typeof(Table).Name}'.");
            }
            var tableName = typeof(Table).Name;
            tQuery.SqlConnection.Open();
            tQuery.SqlConnection.TQuery<Table>().CreateTempTable().Execute();
            using (var copy = new SqlBulkCopy(tQuery.SqlConnection))
            {
                copy.DestinationTableName = $"#{tableName}Temp";
                copy.WriteToServer(ToDataTable(entities));
                var sql = tQuery.DeleteRecordsFromTempSql(keyColumnName).SqlString;
                var cmd = new SqlCommand(sql, tQuery.SqlConnection);
                cmd.ExecuteNonQuery();
            }
            tQuery.SqlConnection.Close();
        }
        internal static DataTable ToDataTable<Table>(this IList<Table> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(Table));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (Table item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static int CreateTable<Table>(this TQueryable<Table> tQuery)
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return tQuery.SqlConnection.Execute($"CREATE TABLE {table.Name}{CreateSql(table)};");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static TQueryableCreate<Table> CreateTable<Table>(this TQueryableSql<Table> tQuery)
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"CREATE TABLE {table.Name}{CreateSql(table)};"
            };
        }

        internal static TQueryableCreate<Table> UpdateTableFromTempSql<Table>(this TQueryable<Table> tQuery, string keyColumnName = "Id")
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            var props = table.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add($"[T].[{field.Name}] = [Temp].[{field.Name}]"); }
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"UPDATE T SET {fields.Join(", ")} FROM [{table.Name}] T INNER JOIN [#{table.Name}Temp] Temp ON [T].[{keyColumnName}] = [Temp].[{keyColumnName}]; DROP TABLE [#{table.Name}Temp];"
            };
        }
        internal static TQueryableCreate<Table> DeleteRecordsFromTempSql<Table>(this TQueryable<Table> tQuery, string keyColumnName = "Id")
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"DELETE T FROM [{table.Name}] T INNER JOIN [#{table.Name}Temp] Temp ON [T].[{keyColumnName}] = [Temp].[{keyColumnName}]; DROP TABLE [#{table.Name}Temp];"
            };
        }
        internal static TQueryableCreate<Table> CreateTempTable<Table>(this TQueryable<Table> tQuery)
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            var props = table.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add(field.Name + " " + field.PropertyType.ToSqlDbTypeInternal()); }
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"CREATE TABLE #{table.Name}Temp ({Environment.NewLine + fields.Join(Environment.NewLine + ",")});"
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static int CreateTableIfNotExists<Table>(this TQueryable<Table> tQuery)
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return tQuery.SqlConnection.Execute($"IF OBJECT_ID('{table.Name}', 'U') IS NULL {Environment.NewLine}{CreateSql(table)};");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static TQueryableCreate<Table> CreateTableIfNotExists<Table>(this TQueryableSql<Table> tQuery)
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"IF OBJECT_ID('{table.Name}', 'U') IS NULL {Environment.NewLine}{CreateSql(table)};"
            };
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static int DropTable<Table>(this TQueryable<Table> tQuery)
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return tQuery.SqlConnection.Execute($"DROP TABLE {table.Name}");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static TQueryableCreate<Table> DropTable<Table>(this TQueryableSql<Table> tQuery)
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"DROP TABLE {table.Name}"
            };
        }

        internal static TQueryableCreate<Table> DropTable<Table>(this SqlConnection sqlConnection, string table)
        {
            return new TQueryableCreate<Table>()
            {
                SqlConnection = sqlConnection,
                SqlString = $"DROP TABLE {table}"
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static int DropTableIfExists<Table>(this TQueryable<Table> tQuery)
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return tQuery.SqlConnection.Execute($"DROP TABLE IF EXISTS {table.Name}");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery"></param>
        /// <returns></returns>
        public static TQueryableCreate<Table> DropTableIfExists<Table>(this TQueryableSql<Table> tQuery)
        {
            var table = tQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"DROP TABLE IF EXISTS {table.Name}"
            };
        }

        private static string CreateSql(this Type type)
        {
            var table = type.Name;
            var props = type.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props)
            {
                string attr = "";
                if (field.GetCustomAttributes(typeof(AutoIncrementAttribute), true).Length > 0)
                {
                    //for MSSQL-Server use:
                    attr = "IDENTITY(1,1) ";
                    //for MySQL use:
                    //attr = "AUTO_INCREMENT ";
                    //}
                }
                if (field.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0)
                {
                    attr += "PRIMARY KEY ";
                }
                else if (field.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0)
                {
                    attr = "NOT NULL " + attr;
                }
                fields.Add($"{field.Name} {field.PropertyType.ToSqlDbTypeInternal()} {attr}");
            }
            return $"CREATE TABLE {table} ({Environment.NewLine + fields.Join(Environment.NewLine + ",")});{Environment.NewLine}";
        }
        private static string CreateIfNotExistsSql(this Type type)
        {
            return $"IF OBJECT_ID('{type.Name}', 'U') IS NULL {Environment.NewLine}{CreateSql(type)};";
        }

        private static string DropSql(this Type type)
        {
            var table = type.Name;
            var props = type.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add(field.Name + " " + field.PropertyType.ToSqlDbTypeInternal()); }
            return $"DROP TABLE {table} ;{Environment.NewLine}";
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static int CreateAllTables(this SqlConnection sqlConnection)
        {
            List<Type> types = new List<Type>();
            var hgy = Assembly
            .GetEntryAssembly().GetName().Name;
            Console.WriteLine(hgy);
            foreach (Type type in Assembly
            .GetEntryAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            TQueryableDatabase query = new TQueryableDatabase() {  SqlConnection = sqlConnection};
            foreach (var t in types)
            {
                query.SqlString += t.CreateSql() + Environment.NewLine;
            }
            return sqlConnection.Execute(query.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static int CreateAllTablesIfNotExists(this SqlConnection sqlConnection)
        {
            List<Type> types = new List<Type>();
            var hgy = Assembly
            .GetEntryAssembly().GetName().Name;
            Console.WriteLine(hgy);
            foreach (Type type in Assembly
            .GetEntryAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            TQueryableDatabase query = new TQueryableDatabase() { SqlConnection = sqlConnection };
            foreach (var t in types)
            {
                query.SqlString += t.CreateIfNotExistsSql() + Environment.NewLine;
            }
            return sqlConnection.Execute(query.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static int DropAllTables(this SqlConnection sqlConnection)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in Assembly
            .GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            TQueryableDatabase query = new TQueryableDatabase() { SqlConnection = sqlConnection };
            foreach (var t in types)
            {
                query.SqlString += t.DropSql() + Environment.NewLine;
            }
            query.SqlConnection = sqlConnection;
            return sqlConnection.Execute(query.SqlString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static TQueryableDatabase CreateAllTablesSql(this SqlConnection sqlConnection)
        {
            List<Type> types = new List<Type>();
            var hgy = Assembly
            .GetEntryAssembly().GetName().Name;
            Console.WriteLine(hgy);
            foreach (Type type in Assembly
            .GetEntryAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            TQueryableDatabase query = new TQueryableDatabase() { SqlConnection = sqlConnection };
            foreach (var t in types)
            {
                query.SqlString += t.CreateSql() + Environment.NewLine;
            }
            query.SqlConnection = sqlConnection;
            return query;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static TQueryableDatabase CreateAllTablesIfNotExistsSql(this SqlConnection sqlConnection)
        {
            List<Type> types = new List<Type>();
            var hgy = Assembly
            .GetEntryAssembly().GetName().Name;
            Console.WriteLine(hgy);
            foreach (Type type in Assembly
            .GetEntryAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            TQueryableDatabase query = new TQueryableDatabase() { SqlConnection = sqlConnection };
            foreach (var t in types)
            {
                query.SqlString += t.CreateIfNotExistsSql() + Environment.NewLine;
            }
            query.SqlConnection = sqlConnection;
            return query;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static TQueryableDatabase DropAllTablesSql(this SqlConnection sqlConnection)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in Assembly
            .GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            TQueryableDatabase query = new TQueryableDatabase() { SqlConnection = sqlConnection };
            foreach (var t in types)
            {
                query.SqlString += t.DropSql() + Environment.NewLine;
            }
            query.SqlConnection = sqlConnection;
            return query;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetAllDbTablesType(this SqlConnection sqlConnection)
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
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

        //public static TQueryable<Table> Distinct<Table>(this TQueryable<Table> tQuery, Func<Table> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<Table> Union<Table>(this TQueryable<Table> tQuery, Func<Table> predicate)
        //{

        //    return tQuery;
        //}

        //TODO What is the return for GroupBy ??? the linq lib has two types with diff arg. Check it out deeply.
        //public static TQueryableGroup<Table> GroupBy<Table, TKey>(this TQueryable<Table> tQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    //if (tQuery.EmptyQuery == null) { tQuery.EmptyQuery = Enumerable.Empty<Table>().AsQueryable(); }
        //    tQuery.EmptyQuery = tQuery.EmptyQuery.GroupBy(predicate).SelectMany(x => x);
        //    tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
        //    TQueryableGroup<Table> _groupedQuery = new TQueryableGroup<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
        //    return _groupedQuery;
        //}
        ////TODO What is the return for GroupBy ??? the linq lib has two types with diff arg. Check it out deeply.
        //public static TQueryableGroup<Table> GroupBy<Table, TKey>(this TQueryableFilter<Table> tQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    tQuery.EmptyQuery = tQuery.EmptyQuery.GroupBy(predicate).SelectMany(x => x);
        //    tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
        //    TQueryableGroup<Table> _groupedQuery = new TQueryableGroup<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
        //    return _groupedQuery;
        //}
        ////TODO What is the return for GroupBy ??? the linq lib has two types with diff arg. Check it out deeply.
        //public static TQueryableGroup<Table> GroupBy<Table, TKey>(this TQueryableOrder<Table> tQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    tQuery.EmptyQuery = tQuery.EmptyQuery.GroupBy(predicate).SelectMany(x => x);
        //    tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery);
        //    TQueryableGroup<Table> _groupedQuery = new TQueryableGroup<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
        //    return _groupedQuery;
        //}

        //public static TQueryableJoin<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this TQueryable<TOuter> outer, TQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        //{
        //    if (outer.EmptyQuery == null) { outer.EmptyQuery = Enumerable.Empty<TOuter>().AsQueryable(); }
        //    IQueryable<TResult> empty = outer.EmptyQuery.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
        //    outer.SqlString = new ExpressionToSQL(empty, outer.EmptyQuery.GroupBy(outerKeySelector).Expression, "JOIN");
        //    TQueryableJoin<TResult> _JoinQuery = new TQueryableJoin<TResult>() { SqlConnection = outer.SqlConnection, EmptyQuery = empty, SqlString = outer.SqlString };
        //    return _JoinQuery;
        //}

        //public static TQueryable<Table> Contains<Table>(this TQueryable<Table> tQuery, Func<Table> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<Table> Aggregate<Table>(this TQueryable<Table> tQuery, Func<Table> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryableCalc<Table> Count<Table, TKey>(this TQueryable<Table> tQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Count(predicate));
        //    TQueryableCalc<Table> _calcQuery = new TQueryableCalc<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
        //    return _calcQuery;
        //}
        //public static TQueryableCount<Table> Count<Table>(this TQueryable<Table> tQuery)
        //{
        //    tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.RecCount(0));
        //    TQueryableCount<Table> _calcQuery = new TQueryableCount<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
        //    return _calcQuery;
        //}
        //public static TQueryableCalc<Table> Average<Table, TKey>(this TQueryable<Table> tQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Average(predicate));
        //    TQueryableCalc<Table> _calcQuery = new TQueryableCalc<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
        //    return _calcQuery;
        //}
        //public static TQueryableCalc<Table> Max<Table, TKey>(this TQueryable<Table> tQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Max(predicate));
        //    TQueryableCalc<Table> _calcQuery = new TQueryableCalc<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
        //    return _calcQuery;
        //}
        //public static TQueryableCalc<Table> Sum<Table, TKey>(this TQueryable<Table> tQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Sum(predicate));
        //    TQueryableCalc<Table> _calcQuery = new TQueryableCalc<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
        //    return _calcQuery;
        //}
        //public static TQueryable<Table> ElementAt<Table>(this TQueryable<Table> tQuery, Func<Table> predicate)
        //{

        //    return tQuery;
        //}
        //public static TQueryable<Table> ElementAtOrDefault<Table>(this TQueryable<Table> tQuery, Func<Table> predicate)
        //{

        //    return tQuery;
        //}

        //public static TQueryCreate DropExtraTableColumns<Table>(this TQueryable<Table> tQuery)
        //{
        //    //check if is missing columns
        //    //
        //}
        //public static TQueryCreate AddMissingTableColumns<Table>(this TQueryable<Table> tQuery)
        //{
        //    //check if is missing columns
        //    //
        //}
        //public static TQueryCreate ModifyTableColumnsDataType<Table>(this TQueryable<Table> tQuery)
        //{
        //    //check if is missing columns
        //    //
        //}
        //public static TQueryCreate RenameTableColumns<Table>(this TQueryable<Table> tQuery)
        //{
        //    //check if is missing columns
        //    //
        //}

    }

}
