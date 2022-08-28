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
using static DbEasyConnect.Tools.FieldAttributes;
using Dapper;
using DbEasyConnect.Tools;

namespace DbEasyConnect.Crud
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
    public static class CrudExtensions
    {

       // TODO handle partial results.if some of the ids was found and some not.
       /// <summary>
        /// Returns a list of all entities from table.
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/>.</param>
        /// <returns>
        /// All records in IDbEc table.
        /// </returns>
        public static IEnumerable<Table> GetAll<Table>(this DbEc<Table> dbEcQuery)
        {
            return dbEcQuery.ToList();
        }

        //TODO check if table has key, and remove the keyColumName param.
        /// <summary>
        /// Returns a single entity by a single id from table.
        /// Id must be marked with [Key] attribute.
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/>.</param>
        /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <param name="keyColumnName"> The column name of the primary key. </param>
        /// <returns>
        /// The record in IDbEc recordset with the given id, or NULL if id not found.
        /// </returns>
        public static Table Find<Table>(this DbEc<Table> dbEcQuery, long id, string keyColumnName = "Id")
        {
            var tableName = typeof(Table).Name;
            return dbEcQuery.SqlConnection.QueryFirstOrDefault<Table>($"SELECT * FROM {tableName} WHERE {keyColumnName}=@Id", new { Id = id });
        }

        // TODO handle partial results.if some of the ids was found and some not.
        /// <summary>
        /// Returns a list of entities from table by a list of given ids.
        /// Id must be marked with [Key] attribute.
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/>.</param>
        /// <param name="ids">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <param name="keyColumnName"> The column name of the primary key. </param>
        /// <returns>
        /// The records in IDbEc recordset with the given ids, or NULL if no id was found.
        /// </returns>
        public static IEnumerable<Table> Find<Table>(this DbEc<Table> dbEcQuery, long[] ids, string keyColumnName = "Id")
        {
            var tableName = typeof(Table).Name;
            return dbEcQuery.SqlConnection.Query<Table>($"SELECT * FROM {tableName} WHERE {keyColumnName} IN @Ids", new { Ids = ids });
        }

        /// <summary>
        /// Inserts an entity into table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/> to perform the insert command.
        /// </param>
        /// <param name="entity">
        /// An single entity to insert.
        /// </param>
        public static void Insert<Table>(this DbEc<Table> dbEcQuery, Table entity)
        {
            List<Table> entities = new List<Table>();
            entities.Add(entity);
            dbEcQuery.InsertList(entities);
        }

        /// <summary>
        /// Inserts an entity into table and returns identity id.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/> to perform the insert command.
        /// </param>
        /// <param name="entity">
        /// An single entity to insert.
        /// </param>
        /// <returns>
        /// Identity id of the new inserted record.
        /// </returns>
        public static int InsertAndReturnId<Table>(this DbEc<Table> dbEcQuery, Table entity)
        {
            return dbEcQuery.SqlConnection.QuerySingle<int>($"{dbEcQuery.InserdbEcQueryBuilder()};SELECT CAST(SCOPE_IDENTITY() as int)", entity);
        }
        /// <summary>
        /// Inserts a list of entities into table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/> to perform the insert command.
        /// </param>
        /// <param name="entities">
        /// An list of entities to insert.
        /// </param>
        public static void InsertList<Table>(this DbEc<Table> dbEcQuery, List<Table> entities)
        {
            using (var copy = new SqlBulkCopy(dbEcQuery.SqlConnection))
            {
                copy.DestinationTableName = typeof(Table).Name;
                dbEcQuery.SqlConnection.Open();
                copy.WriteToServer(ToDataTable(entities));
                dbEcQuery.SqlConnection.Close();
            }
        }

        /// <summary>
        /// Inserts a list of entities into table and returns a list of identity ids.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/> to perform the insert command.
        /// </param>
        /// <param name="entities">
        /// An list of entities to insert.
        /// </param>
        /// <param name="keyColumnName">The column that contains the Key/Id that will be used to return</param>
        /// <returns>
        /// List of identity ids of the new inserted records.
        /// </returns>
        public static IEnumerable<int> InsertListAndReturnIds<Table>(this DbEc<Table> dbEcQuery, List<Table> entities, string keyColumnName = "Id")
        {
            var tableName = typeof(Table).Name;
            var sql = dbEcQuery.InsertFromTempSql(keyColumnName);
            dbEcQuery.SqlConnection.Open();
            dbEcQuery.SqlConnection.IDbEc<Table>().CreateTempTable().Execute();
            using (var copy = new SqlBulkCopy(dbEcQuery.SqlConnection))
            {
                copy.DestinationTableName = $"#{tableName}Temp";
                copy.WriteToServer(ToDataTable(entities));
            }
            IEnumerable<int> result = dbEcQuery.SqlConnection.Query<int>(sql);
            dbEcQuery.SqlConnection.Close();
            return result;
        }

        /// <summary>
        /// Updates an entity in table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/> to perform the update command.
        /// </param>
        /// <param name="entity">
        /// An single entity to update.
        /// </param>
        /// <param name="keyColumnName"> The column name of the primary key. </param>
        public static void Update<Table>(this DbEc<Table> dbEcQuery, Table entity, string keyColumnName = "Id")
        {
            List<Table> entities = new List<Table>();
            entities.Add(entity);
            dbEcQuery.UpdateList(entities, keyColumnName);
        }

        /// <summary>
        /// Updates one or more columns on all records of the IDbEc recordset to the database table by the predicate assignment(s).  
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/> to perform the update command.
        /// </param>
        /// <param name="expression">
        /// The columns to be updated with their new value assignment.
        /// Example: IDbEc&lt;Sample&gt;().Update(x =&gt; new Sample { MyProperty = x.MyInteger + 5, MyString = null, MyBool = true, MyOtherString = "something" })
        /// </param>
        /// <returns>
        /// The number of records updated successfully.
        /// </returns>        
        public static int Update<Table>(this DbEc<Table> dbEcQuery, Expression<Func<Table, Table>> expression)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.Update(expression));
            return dbEcQuery.SqlConnection.Execute(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Updates one or more columns on all records of the IDbEc recordset to the database table by the predicate assignment(s).  
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the update command.
        /// </param>
        /// <param name="assignment">
        /// The columns to be updated with their new value assignment.
        /// Example: IDbEc&lt;Sample&gt;().Update(x =&gt; new Sample { MyProperty = x.MyInteger + 5, MyString = null, MyBool = true, MyOtherString = "something" })
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableUpdate{T}"/> instance, which the SQL command will update records in the database, and <see cref="TQueryExecute{T}.Execute"/> will return the number of records updated successfully.
        /// </returns>
        public static DbEcUpdate<Table> Update<Table>(this DbEcExtended<Table> dbEcQuery, Expression<Func<Table, Table>> assignment)
        {
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.Update(assignment));
            DbEcUpdate<Table> _updatedQuery = new DbEcUpdate<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return _updatedQuery;
        }

        /// <summary>
        /// Updates a list of entities in table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/> to perform the update command.
        /// </param>
        /// <param name="entities">
        /// An list of entities to update.
        /// </param>
        /// <param name="keyColumnName">The column that contains the Key/Id that will be used to recognize the record to update</param>
        public static void UpdateList<Table>(this DbEc<Table> dbEcQuery, List<Table> entities, string keyColumnName = "Id")
        {
            if (typeof(Table).GetProperty(keyColumnName) == null)
            {
                throw new NullReferenceException($"Missing column name '{keyColumnName}' in table '{typeof(Table).Name}'.");
            }
            var tableName = typeof(Table).Name;
            dbEcQuery.SqlConnection.Open();
            dbEcQuery.SqlConnection.IDbEc<Table>().CreateTempTable().Execute();
            using (var copy = new SqlBulkCopy(dbEcQuery.SqlConnection))
            {
                copy.DestinationTableName = $"#{tableName}Temp";
                copy.WriteToServer(ToDataTable(entities));
                var sql = dbEcQuery.UpdateTableFromTempSql(keyColumnName);
                var cmd = new SqlCommand(sql, dbEcQuery.SqlConnection);
                cmd.ExecuteNonQuery();
            }
            dbEcQuery.SqlConnection.Close();
        }

        /// <summary>
        /// Deletes all records of the IDbEc recordset on the database table.  
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/> to perform the update command.
        /// </param>
        /// <returns>
        /// The number of records deleted successfully.
        /// </returns> 
        public static int Delete<Table>(this DbEc<Table> dbEcQuery)
        {
            var exp = ExpressionToSQL.Delete(1);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, exp);
            return dbEcQuery.SqlConnection.Execute(dbEcQuery.SqlString);
        }

        /// <summary>
        /// Deletes all records of the IDbEc recordset on the database table.  
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the update command.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableDelete{T}"/> instance, which the SQL command will delete records in the database, and <see cref="TQueryExecute{T}.Execute"/> will return the number of records deleted successfully.
        /// </returns>
        public static DbEcDelete<Table> Delete<Table>(this DbEcExtended<Table> dbEcQuery)
        {
            var exp = ExpressionToSQL.Delete(1);
            dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, exp);
            var deleteQuery = new DbEcDelete<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
            return deleteQuery;
        }

        /// <summary>
        /// Deletes an entity from table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/> to perform the delete command.
        /// </param>
        /// <param name="entity">
        /// An single entity to delete.
        /// </param>
        /// <param name="keyColumnName"> The column name of the primary key. </param>
        public static void Delete<Table>(this DbEc<Table> dbEcQuery, Table entity, string keyColumnName = "Id")
        {
            List<Table> entities = new List<Table>();
            entities.Add(entity);
            dbEcQuery.DeleteList(entities, keyColumnName);
        }

        /// <summary>
        /// Deletes a list of entities from table.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="dbEcQuery">
        /// An <see cref="TQueryable{T}"/> to perform the delete command.
        /// </param>
        /// <param name="entities">
        /// An list of entities to delete.
        /// </param>
        /// <param name="keyColumnName">The column that contains the Key/Id that will be used to recognize the record to delete</param>
        public static void DeleteList<Table>(this DbEc<Table> dbEcQuery, List<Table> entities, string keyColumnName = "Id")
        {
            if (typeof(Table).GetProperty(keyColumnName) == null)
            {
                throw new NullReferenceException($"Missing column name '{keyColumnName}' in table '{typeof(Table).Name}'.");
            }
            var tableName = typeof(Table).Name;
            dbEcQuery.SqlConnection.Open();
            dbEcQuery.SqlConnection.IDbEc<Table>().CreateTempTable().Execute();
            using (var copy = new SqlBulkCopy(dbEcQuery.SqlConnection))
            {
                copy.DestinationTableName = $"#{tableName}Temp";
                copy.WriteToServer(ToDataTable(entities));
                var sql = dbEcQuery.DeleteRecordsFromTempSql(keyColumnName).SqlString;
                var cmd = new SqlCommand(sql, dbEcQuery.SqlConnection);
                cmd.ExecuteNonQuery();
            }
            dbEcQuery.SqlConnection.Close();
        }
        internal class OrderAttribute : Attribute
        {
            private int v;

            public OrderAttribute(int v)
            {
                this.v = v;
            }
        }
        internal static DataTable ToDataTable<Table>(this IList<Table> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(Table));
                    //TODO find a way to order the fileds by written order.
                    //.OrderBy(p => ((Order)p.GetCustomAttributes(typeof(Order), false)[0]).Order);
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

        internal static string InserdbEcQueryBuilder<Table>(this DbEc<Table> dbEcQuery)
        {
            var props = dbEcQuery.TableType.GetProperties().ToList();
            StringBuilder columns = new StringBuilder();
            StringBuilder values = new StringBuilder();
            foreach (PropertyInfo field in props)
            {
                //TODO fix code for custom field name
                columns.Append($"{field.Name}, ");
                values.Append($"@{field.Name}, ");
            }

            string inserdbEcQuery = $"({ columns.ToString().TrimEnd(',', ' ')}) VALUES ({ values.ToString().TrimEnd(',', ' ')}) ";
            return inserdbEcQuery;
        }
        internal static string UpdateTableFromTempSql<Table>(this DbEc<Table> dbEcQuery, string keyColumnName = "Id")
        {
            var table = dbEcQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            var props = table.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add($"[T].[{field.Name}] = [Temp].[{field.Name}]"); }
            return $"UPDATE T SET {fields.Join(", ")} FROM [{table.Name}] T INNER JOIN [#{table.Name}Temp] Temp ON [T].[{keyColumnName}] = [Temp].[{keyColumnName}]; DROP TABLE [#{table.Name}Temp];";
        }
        internal static string InsertFromTempSql<Table>(this DbEc<Table> dbEcQuery, string keyColumnName = "Id")
        {
            var table = dbEcQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            var props = table.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add($"[T].[{field.Name}]"); }
            return $"INSERT INTO [{table.Name}] OUTPUT INSERTED.{keyColumnName} SELECT {fields.Join(", ")} FROM [#{table.Name}Temp] T; DROP TABLE [#{table.Name}Temp]; ";
        }
        internal static DbEcCreate<Table> DeleteRecordsFromTempSql<Table>(this DbEc<Table> dbEcQuery, string keyColumnName = "Id")
        {
            var table = dbEcQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            return new DbEcCreate<Table>()
            {
                SqlConnection = dbEcQuery.SqlConnection,
                SqlString = $"DELETE T FROM [{table.Name}] T INNER JOIN [#{table.Name}Temp] Temp ON [T].[{keyColumnName}] = [Temp].[{keyColumnName}]; DROP TABLE [#{table.Name}Temp];"
            };
        }
        internal static DbEcCreate<Table> CreateTempTable<Table>(this DbEc<Table> dbEcQuery)
        {
            var table = dbEcQuery.EmptyQuery.GetType().GenericTypeArguments[0];
            var props = table.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add(field.Name + " " + field.PropertyType.ToSqlDbTypeInternal()); }
            return new DbEcCreate<Table>()
            {
                SqlConnection = dbEcQuery.SqlConnection,
                SqlString = $"CREATE TABLE #{table.Name}Temp ({Environment.NewLine + fields.Join(Environment.NewLine + ",")});"
            };
        }


        //public static DbEc<Table> Distinct<Table>(this DbEc<Table> dbEcQuery, Func<Table> predicate)
        //{

        //    return dbEcQuery;
        //}
        //public static DbEc<Table> Union<Table>(this DbEc<Table> dbEcQuery, Func<Table> predicate)
        //{

        //    return dbEcQuery;
        //}

        //TODO What is the return for GroupBy ??? the linq lib has two types with diff arg. Check it out deeply.
        //public static DbEcGroup<Table> GroupBy<Table, TKey>(this DbEc<Table> dbEcQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    //if (dbEcQuery.EmptyQuery == null) { dbEcQuery.EmptyQuery = Enumerable.Empty<Table>().AsQueryable(); }
        //    dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.GroupBy(predicate).SelectMany(x => x);
        //    dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
        //    DbEcGroup<Table> _groupedQuery = new DbEcGroup<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
        //    return _groupedQuery;
        //}
        ////TODO What is the return for GroupBy ??? the linq lib has two types with diff arg. Check it out deeply.
        //public static DbEcGroup<Table> GroupBy<Table, TKey>(this DbEcFilter<Table> dbEcQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.GroupBy(predicate).SelectMany(x => x);
        //    dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
        //    DbEcGroup<Table> _groupedQuery = new DbEcGroup<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
        //    return _groupedQuery;
        //}
        ////TODO What is the return for GroupBy ??? the linq lib has two types with diff arg. Check it out deeply.
        //public static DbEcGroup<Table> GroupBy<Table, TKey>(this DbEcOrder<Table> dbEcQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    dbEcQuery.EmptyQuery = dbEcQuery.EmptyQuery.GroupBy(predicate).SelectMany(x => x);
        //    dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery);
        //    DbEcGroup<Table> _groupedQuery = new DbEcGroup<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
        //    return _groupedQuery;
        //}

        //public static DbEcJoin<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this DbEc<TOuter> outer, DbEc<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        //{
        //    if (outer.EmptyQuery == null) { outer.EmptyQuery = Enumerable.Empty<TOuter>().AsQueryable(); }
        //    IQueryable<TResult> empty = outer.EmptyQuery.Join(Enumerable.Empty<TInner>().AsQueryable(), outerKeySelector, innerKeySelector, resultSelector).AsQueryable();
        //    outer.SqlString = new ExpressionToSQL(empty, outer.EmptyQuery.GroupBy(outerKeySelector).Expression, "JOIN");
        //    DbEcJoin<TResult> _JoinQuery = new DbEcJoin<TResult>() { SqlConnection = outer.SqlConnection, EmptyQuery = empty, SqlString = outer.SqlString };
        //    return _JoinQuery;
        //}

        //public static DbEc<Table> Contains<Table>(this DbEc<Table> dbEcQuery, Func<Table> predicate)
        //{

        //    return dbEcQuery;
        //}
        //public static DbEc<Table> Aggregate<Table>(this DbEc<Table> dbEcQuery, Func<Table> predicate)
        //{

        //    return dbEcQuery;
        //}
        //public static DbEcCalc<Table> Count<Table, TKey>(this DbEc<Table> dbEcQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.Count(predicate));
        //    DbEcCalc<Table> _calcQuery = new DbEcCalc<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
        //    return _calcQuery;
        //}
        //public static DbEcCount<Table> Count<Table>(this DbEc<Table> dbEcQuery)
        //{
        //    dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.RecCount(0));
        //    DbEcCount<Table> _calcQuery = new DbEcCount<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
        //    return _calcQuery;
        //}
        //public static DbEcCalc<Table> Average<Table, TKey>(this DbEc<Table> dbEcQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.Average(predicate));
        //    DbEcCalc<Table> _calcQuery = new DbEcCalc<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
        //    return _calcQuery;
        //}
        //public static DbEcCalc<Table> Max<Table, TKey>(this DbEc<Table> dbEcQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.Max(predicate));
        //    DbEcCalc<Table> _calcQuery = new DbEcCalc<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
        //    return _calcQuery;
        //}
        //public static DbEcCalc<Table> Sum<Table, TKey>(this DbEc<Table> dbEcQuery, Expression<Func<Table, TKey>> predicate)
        //{
        //    dbEcQuery.SqlString = new ExpressionToSQL(dbEcQuery.EmptyQuery, ExpressionToSQL.Sum(predicate));
        //    DbEcCalc<Table> _calcQuery = new DbEcCalc<Table>() { SqlConnection = dbEcQuery.SqlConnection, EmptyQuery = dbEcQuery.EmptyQuery, SqlString = dbEcQuery.SqlString };
        //    return _calcQuery;
        //}
        //public static DbEc<Table> ElementAt<Table>(this DbEc<Table> dbEcQuery, Func<Table> predicate)
        //{

        //    return dbEcQuery;
        //}
        //public static DbEc<Table> ElementAtOrDefault<Table>(this DbEc<Table> dbEcQuery, Func<Table> predicate)
        //{

        //    return dbEcQuery;
        //}

        //public static IDbEcCreate DropExtraTableColumns<Table>(this DbEc<Table> dbEcQuery)
        //{
        //    //check if is missing columns
        //    //
        //}
        //public static IDbEcCreate AddMissingTableColumns<Table>(this DbEc<Table> dbEcQuery)
        //{
        //    //check if is missing columns
        //    //
        //}
        //public static IDbEcCreate ModifyTableColumnsDataType<Table>(this DbEc<Table> dbEcQuery)
        //{
        //    //check if is missing columns
        //    //
        //}
        //public static IDbEcCreate RenameTableColumns<Table>(this DbEc<Table> dbEcQuery)
        //{
        //    //check if is missing columns
        //    //
        //}

    }

}
