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
using System.Text;
using static Dapper.TQuery.FieldAttributes;

namespace Dapper.TQuery
{

    /// <summary>
    /// Communicate with any database the fastest way with easiest configuration, and strongly typed written.
    /// <br/>This Library was created to replace <see href="https://docs.microsoft.com/en-us/ef/">Entity Framework</see> complexity, Slow-performing, and diagnose issues. Learn more <see href="https://www.iamtimcorey.com/blog/137806/entity-framework">here</see>.
    /// <br/>It is based on the Dapper library, with the following advantages:
    /// <br/>Use Dapper with most of Linq methods as an <see cref="IQueryable"/> object without downloading the records. <see href="https://stackoverflow.com/q/27563096/6509536">Dapper.NET and IQueryable issue</see>.
    /// <br/>Use Code First feature, or/and modify the database from the code, with just one or two lines of code.
    /// <br/>Use the fastest easiest way for CRUD (Create, Read, Update, and Delete) operations, Find by ID, bulk insert/update/delete even with Entity List.
    /// <br/>Gives a strong typed coding experience, to querying the database similar to entity framework, and avoid spelling mistakes on table and field names.
    /// </summary>
    internal static class NamespaceDoc { }
    /// <summary>
    /// Use the fastest easiest way for CRUD (Create, Read, Update, and Delete) operations, Find by ID, bulk insert/update/delete even with Entity List.
    /// </summary>
    public static class TQueryCrudExtensions
    {

       // TODO handle partial results.if some of the ids was found and some not.
       /// <summary>
        /// Returns a list of all entities from table.
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/>.</param>
        /// <returns>
        /// All records in TQuery table.
        /// </returns>
        public static IEnumerable<Table> GetAll<Table>(this TQueryable<Table> tQuery)
        {
            return tQuery.ToList();
        }

        //TODO check if table has key, and remove the keyColumName param.
        /// <summary>
        /// Returns a single entity by a single id from table.
        /// Id must be marked with [Key] attribute.
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/>.</param>
        /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <param name="keyColumnName"> The column name of the primary key. </param>
        /// <returns>
        /// The record in TQuery recordset with the given id, or NULL if id not found.
        /// </returns>
        public static Table Find<Table>(this TQueryable<Table> tQuery, long id, string keyColumnName = "Id")
        {
            var tableName = typeof(Table).Name;
            return tQuery.SqlConnection.QueryFirstOrDefault<Table>($"SELECT * FROM {tableName} WHERE {keyColumnName}=@Id", new { Id = id });
        }

        // TODO handle partial results.if some of the ids was found and some not.
        /// <summary>
        /// Returns a list of entities from table by a list of given ids.
        /// Id must be marked with [Key] attribute.
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/>.</param>
        /// <param name="ids">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <param name="keyColumnName"> The column name of the primary key. </param>
        /// <returns>
        /// The records in TQuery recordset with the given ids, or NULL if no id was found.
        /// </returns>
        public static IEnumerable<Table> Find<Table>(this TQueryable<Table> tQuery, long[] ids, string keyColumnName = "Id")
        {
            var tableName = typeof(Table).Name;
            return tQuery.SqlConnection.Query<Table>($"SELECT * FROM {tableName} WHERE {keyColumnName} IN @Ids", new { Ids = ids });
        }

        /// <summary>
        /// Inserts an entity into table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the insert command.
        /// </param>
        /// <param name="entity">
        /// An single entity to insert.
        /// </param>
        public static void Insert<Table>(this TQueryable<Table> tQuery, Table entity)
        {
            List<Table> entities = new List<Table>();
            entities.Add(entity);
            tQuery.InsertList(entities);
        }

        /// <summary>
        /// Inserts an entity into table and returns identity id.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the insert command.
        /// </param>
        /// <param name="entity">
        /// An single entity to insert.
        /// </param>
        /// <returns>
        /// Identity id of the new inserted record.
        /// </returns>
        public static int InsertAndReturnId<Table>(this TQueryable<Table> tQuery, Table entity)
        {
            return tQuery.SqlConnection.QuerySingle<int>($"{tQuery.InsertQueryBuilder()};SELECT CAST(SCOPE_IDENTITY() as int)", entity);
        }
        /// <summary>
        /// Inserts a list of entities into table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the insert command.
        /// </param>
        /// <param name="entities">
        /// An list of entities to insert.
        /// </param>
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
        /// Inserts a list of entities into table and returns a list of identity ids.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the insert command.
        /// </param>
        /// <param name="entities">
        /// An list of entities to insert.
        /// </param>
        /// <param name="keyColumnName">The column that contains the Key/Id that will be used to return</param>
        /// <returns>
        /// List of identity ids of the new inserted records.
        /// </returns>
        public static IEnumerable<int> InsertListAndReturnIds<Table>(this TQueryable<Table> tQuery, List<Table> entities, string keyColumnName = "Id")
        {
            var tableName = typeof(Table).Name;
            var sql = tQuery.InsertFromTempSql(keyColumnName);
            tQuery.SqlConnection.Open();
            tQuery.SqlConnection.TQuery<Table>().CreateTempTable().Execute();
            using (var copy = new SqlBulkCopy(tQuery.SqlConnection))
            {
                copy.DestinationTableName = $"#{tableName}Temp";
                copy.WriteToServer(ToDataTable(entities));
            }
            IEnumerable<int> result = tQuery.SqlConnection.Query<int>(sql);
            tQuery.SqlConnection.Close();
            return result;
        }

        /// <summary>
        /// Updates an entity in table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the update command.
        /// </param>
        /// <param name="entity">
        /// An single entity to update.
        /// </param>
        /// <param name="keyColumnName"> The column name of the primary key. </param>
        public static void Update<Table>(this TQueryable<Table> tQuery, Table entity, string keyColumnName = "Id")
        {
            List<Table> entities = new List<Table>();
            entities.Add(entity);
            tQuery.UpdateList(entities, keyColumnName);
        }

        /// <summary>
        /// Updates one or more columns on all records of the TQuery recordset to the database table by the predicate assignment(s).  
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the update command.
        /// </param>
        /// <param name="expression">
        /// The columns to be updated with their new value assignment.
        /// Example: TQuery&lt;Sample&gt;().Update(x =&gt; new Sample { MyProperty = x.MyInteger + 5, MyString = null, MyBool = true, MyOtherString = "something" })
        /// </param>
        /// <returns>
        /// The number of records updated successfully.
        /// </returns>        
        public static int Update<Table>(this TQueryable<Table> tQuery, Expression<Func<Table, Table>> expression)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Update(expression));
            return tQuery.SqlConnection.Execute(tQuery.SqlString);
        }

        /// <summary>
        /// Updates one or more columns on all records of the TQuery recordset to the database table by the predicate assignment(s).  
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the update command.
        /// </param>
        /// <param name="assignment">
        /// The columns to be updated with their new value assignment.
        /// Example: TQuery&lt;Sample&gt;().Update(x =&gt; new Sample { MyProperty = x.MyInteger + 5, MyString = null, MyBool = true, MyOtherString = "something" })
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableUpdate{T}"/> instance, which the SQL command will update records in the database, and <see cref="TQueryExecute{T}.Execute"/> will return the number of records updated successfully.
        /// </returns>
        public static TQueryableUpdate<Table> Update<Table>(this TQueryableExtended<Table> tQuery, Expression<Func<Table, Table>> assignment)
        {
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, ExpressionToSQL.Update(assignment));
            TQueryableUpdate<Table> _updatedQuery = new TQueryableUpdate<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return _updatedQuery;
        }

        /// <summary>
        /// Updates a list of entities in table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the update command.
        /// </param>
        /// <param name="entities">
        /// An list of entities to update.
        /// </param>
        /// <param name="keyColumnName">The column that contains the Key/Id that will be used to recognize the record to update</param>
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
                var sql = tQuery.UpdateTableFromTempSql(keyColumnName);
                var cmd = new SqlCommand(sql, tQuery.SqlConnection);
                cmd.ExecuteNonQuery();
            }
            tQuery.SqlConnection.Close();
        }

        /// <summary>
        /// Deletes all records of the TQuery recordset on the database table.  
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the update command.
        /// </param>
        /// <returns>
        /// The number of records deleted successfully.
        /// </returns> 
        public static int Delete<Table>(this TQueryable<Table> tQuery)
        {
            var exp = ExpressionToSQL.Delete(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            return tQuery.SqlConnection.Execute(tQuery.SqlString);
        }

        /// <summary>
        /// Deletes all records of the TQuery recordset on the database table.  
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the update command.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableDelete{T}"/> instance, which the SQL command will delete records in the database, and <see cref="TQueryExecute{T}.Execute"/> will return the number of records deleted successfully.
        /// </returns>
        public static TQueryableDelete<Table> Delete<Table>(this TQueryableExtended<Table> tQuery)
        {
            var exp = ExpressionToSQL.Delete(1);
            tQuery.SqlString = new ExpressionToSQL(tQuery.EmptyQuery, exp);
            var deleteQuery = new TQueryableDelete<Table>() { SqlConnection = tQuery.SqlConnection, EmptyQuery = tQuery.EmptyQuery, SqlString = tQuery.SqlString };
            return deleteQuery;
        }

        /// <summary>
        /// Deletes an entity from table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the delete command.
        /// </param>
        /// <param name="entity">
        /// An single entity to delete.
        /// </param>
        /// <param name="keyColumnName"> The column name of the primary key. </param>
        public static void Delete<Table>(this TQueryable<Table> tQuery, Table entity, string keyColumnName = "Id")
        {
            List<Table> entities = new List<Table>();
            entities.Add(entity);
            tQuery.DeleteList(entities, keyColumnName);
        }

        /// <summary>
        /// Deletes a list of entities from table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the delete command.
        /// </param>
        /// <param name="entities">
        /// An list of entities to delete.
        /// </param>
        /// <param name="keyColumnName">The column that contains the Key/Id that will be used to recognize the record to delete</param>
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
