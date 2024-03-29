﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static Dapper.TQuery.Development.FieldAttributes;
using System.Collections;
using static Dapper.TQuery.Development.TQueryExceptions;

namespace Dapper.TQuery.Development
{
    /// <summary>
    /// Handle table defenitions, create/modify/delete tables based on the code table classes, and compare between server database and code tables, and more.
    /// <br/>Use Code First feature, or/and modify the database from the code, with just one or two lines of code.
    /// </summary>
    public static class TQueryTableExtensions
    {

        /// <summary>
        /// Creates a Table on the server database, based on the code table properties.
        /// Supported attributes are: [Key] for Primary Key, [AutoIncrement] for Auto Increment property, [Required] for Not Null property.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the Create Table command.
        /// </param>
        public static void CreateTable<Table>(this TQueryable<Table> tQuery)
        {
            tQuery.SqlConnection.Execute(CreateSql(tQuery));
        }

        /// <summary>
        /// Creates a Table on the server database, based on the code table properties.
        /// Supported attributes are: [Key] for Primary Key, [AutoIncrement] for Auto Increment property, [Required] for Not Null property.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the Create Table command.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableCreate{T}"/> instance, which the SQL command will Create the Table in the database, and <see cref="TQueryExecute{T}.Execute"/> will execute the Create Table command.
        /// </returns>
        public static TQueryableCreate<Table> CreateTable<Table>(this TQueryableExtended<Table> tQuery)
        {
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = CreateSql(tQuery)
            };
        }

        /// <summary>
        /// Overrides a Table on the server database, based on the code table properties. 
        /// Caution! This action will DELETE the current table on the Server with all the records, and then create a new blank Table with the current code properties.
        /// Supported attributes are: [Key] for Primary Key, [AutoIncrement] for Auto Increment property, [Required] for Not Null property.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the Create Table command.
        /// </param>
        public static int OverrideTable<Table>(this TQueryable<Table> tQuery)
        {
            return tQuery.SqlConnection.Execute($"DROP TABLE {tQuery.TableName};{Environment.NewLine}{CreateSql(tQuery)}");
        }

        /// <summary>
        /// Overrides a Table on the server database, based on the code table properties. 
        /// Caution! This action will DELETE the current table on the Server with all the records, and then create a new blank Table with the current code properties.
        /// Supported attributes are: [Key] for Primary Key, [AutoIncrement] for Auto Increment property, [Required] for Not Null property.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the Override Table command.
        /// </param>
        ///<returns>
        /// An <see cref="TQueryableCreate{T}"/> instance, which the SQL command will Override the Table in the database, and <see cref="TQueryExecute{T}.Execute"/> will execute the Override Table command.
        /// </returns>
        public static TQueryableCreate<Table> OverrideTable<Table>(this TQueryableExtended<Table> tQuery)
        {
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"DROP TABLE {tQuery.TableName};{Environment.NewLine}{CreateSql(tQuery)}"
            };
        }

        //TODO add more options on modify column, like column name by specifieng the old name, and change/add/remove attributes.
        /// <summary>
        /// Modifies a table definition on the server database by altering, adding, or dropping columns and column datatypes, based on the code table properties. 
        /// Available modifications: Add field, Remove field, Change field datatype. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the Modify Table command.
        /// </param>
        public static int ModifyTable<Table>(this TQueryable<Table> tQuery)
        {
            return tQuery.SqlConnection.Execute(ModifySql(tQuery.TableType, tQuery.SqlConnection));
        }

        //TODO add more options on modify column, like column name by specifieng the old name, and change/add/remove attributes.
        /// <summary>
        /// Modifies a table definition on the server database by altering, adding, or dropping columns and column datatypes, based on the code table properties. 
        /// Available modifications: Add field, Remove field, Change field datatype. 
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the Modify Table command.
        /// </param>
        ///<returns>
        /// An <see cref="TQueryableCreate{T}"/> instance, which the SQL command will Modify the Table in the database, and <see cref="TQueryExecute{T}.Execute"/> will execute the Modify Table command.
        /// </returns>
        public static TQueryableCreate<Table> ModifyTable<Table>(this TQueryableExtended<Table> tQuery)
        {
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = ModifySql(tQuery.TableType, tQuery.SqlConnection)
            };
        }

        /// <summary>
        /// Creates a Table on the server database if not exists, based on the code table properties.
        /// Supported attributes are: [Key] for Primary Key, [AutoIncrement] for Auto Increment property, [Required] for Not Null property.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the Create Table command.
        /// </param>
        public static void CreateTableIfNotExists<Table>(this TQueryable<Table> tQuery)
        {
            tQuery.SqlConnection.Execute($"IF OBJECT_ID(N'{tQuery.TableName}', 'U') IS NULL {Environment.NewLine}{CreateSql(tQuery)};");
        }

        /// <summary>
        /// Creates a Table on the server database if not exists, based on the code table properties.
        /// Supported attributes are: [Key] for Primary Key, [AutoIncrement] for Auto Increment property, [Required] for Not Null property.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the Create Table command.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableCreate{T}"/> instance, which the SQL command will Create the Table in the database, and <see cref="TQueryExecute{T}.Execute"/> will execute the Create Table command.
        /// </returns>
        public static TQueryableCreate<Table> CreateTableIfNotExists<Table>(this TQueryableExtended<Table> tQuery)
        {
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"IF OBJECT_ID(N'{tQuery.TableName}', 'U') IS NULL {Environment.NewLine}{CreateSql(tQuery)};"
            };
        }

        /// <summary>
        /// Removes a Table from the server database.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the Drop Table command.
        /// </param>
        public static void DropTable<Table>(this TQueryable<Table> tQuery)
        {
            tQuery.SqlConnection.Execute($"DROP TABLE {tQuery.TableName}");
        }

        /// <summary>
        /// Removes a Table from the server database.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the Drop Table command.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableCreate{T}"/> instance, which the SQL command will remove the Table from the database, and <see cref="TQueryExecute{T}.Execute"/> will execute the Drop Table command.
        /// </returns>
        public static TQueryableCreate<Table> DropTable<Table>(this TQueryableExtended<Table> tQuery)
        {
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"DROP TABLE {tQuery.TableName}"
            };
        }

        /// <summary>
        /// Removes a Table from the server database if exists.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the Drop Table command.
        /// </param>
        public static int DropTableIfExists<Table>(this TQueryable<Table> tQuery)
        {
            return tQuery.SqlConnection.Execute($"DROP TABLE IF EXISTS {tQuery.TableName}");
        }

        /// <summary>
        /// Removes a Table from the server database if exists.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryableExtended{T}"/> to perform the Drop Table command.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableCreate{T}"/> instance, which the SQL command will remove the Table from the database, and <see cref="TQueryExecute{T}.Execute"/> will execute the Drop Table command.
        /// </returns>
        public static TQueryableCreate<Table> DropTableIfExists<Table>(this TQueryableExtended<Table> tQuery)
        {
            return new TQueryableCreate<Table>()
            {
                SqlConnection = tQuery.SqlConnection,
                SqlString = $"DROP TABLE IF EXISTS {tQuery.TableName}"
            };
        }

        private static string CreateSql(this TQueryable tQuery)
        {
            var props = tQuery.TableType.GetProperties().ToList();
            List<string> fields = new List<string>();
            List<string> constraints = new List<string>();
            var fieldName = "";
            getPropsNamesAndAttr(props, ref fields, ref constraints, ref fieldName);
            return $"CREATE TABLE {tQuery.TableName} ({Environment.NewLine + fields.Join(Environment.NewLine + ",")});{Environment.NewLine}";
        }
        private static string CreateSql<T>(this TQuery<T> tQuery)
        {
            //TODO order props by decleration order. Code is perfect, but requires to use the [Order] attribute.
            //var props = from property in type.GetProperties()
            //                 where Attribute.IsDefined(property, typeof(OrderAttribute))
            //                 orderby ((OrderAttribute)property
            //                           .GetCustomAttributes(typeof(OrderAttribute), false)
            //                           .Single()).Order
            //                 select property;
            var props = tQuery.TableType.GetProperties().ToList();
            List<string> fields = new List<string>();
            List<string> constraints = new List<string>();
            var fieldName = "";
            getPropsNamesAndAttr(props, ref fields, ref constraints, ref fieldName);
            return $"CREATE TABLE {tQuery.TableName} ({Environment.NewLine + fields.Join(Environment.NewLine + ",")});{Environment.NewLine}";
        }
        private static void getPropsNamesAndAttr(List<PropertyInfo> props, ref List<string> fields, ref List<string> constraints, ref string fieldName)
        {
            foreach (PropertyInfo field in props)
            {
                Type fieldType = field.GetType();
                if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    fieldType = Nullable.GetUnderlyingType(field.GetType());
                }
                //if not mapped then skip
                if (field.IsDefined(typeof(NotMappedAttribute))) continue;
                //if enumerable then try creating foreigenKey
                if (fieldType == typeof(IEnumerable)) { TryCreatingForeigenKey(field); continue; }
                //if user-defined subType
                if (field.PropertyType.Assembly.FullName == field.ReflectedType.Assembly.FullName)
                {
                    if (TQueryDefaults.AllowSubTypeAuto || field.IsDefined(typeof(SubTypeAttribute)))
                    {
                        var subProps = field.PropertyType.GetProperties().ToList();
                        if (field.IsDefined(typeof(SubTypeAttribute)) && field.GetCustomAttribute<SubTypeAttribute>().IncludeParentName) fieldName += field.Name + "_";
                        getPropsNamesAndAttr(subProps, ref fields, ref constraints, ref fieldName);
                        continue;
                    } else { throw new NotSupportedFieldTypeException(field); }
                }
                var tableField = new TableField { };
                //name of the field
                fieldName += field.GetCustomAttribute<ColumnAttribute>()?.Name;
                tableField.Name = string.IsNullOrEmpty(fieldName) ? field.Name : fieldName;

                //data-type of the field. Use smart choice, to maximize performance, with common limits.
                tableField.DataType = field.MatchBestSqlDataType();
                // default field 'id' is primary key, and auto increment
                if (tableField.Name.ToLower() == "id" && fieldType.IsIntType() && TQueryDefaults.IdFieldIsKeyByDefault)
                { tableField.PrimaryKey = true; tableField.AutoIncrement = true; }
                //nullable?
                if (field.IsDefined(typeof(RequiredAttribute)) || field.IsDefined(typeof(KeyAttribute)) || 
                    fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>) && TQueryDefaults.SetNullablePropsInSqlToNull)
                        tableField.Required = true;
                //other attributes
                foreach (var attribute in field.GetCustomAttributes())
                {
                    switch (attribute.GetType().Name) 
                    {
                        case nameof(ColumnAttribute):
                            if (field.GetCustomAttribute<ColumnAttribute>()?.Order > 0)
                                tableField.Order = (int)field.GetCustomAttribute<ColumnAttribute>()?.Order;
                            break;
                        case nameof(KeyAttribute):
                            tableField.PrimaryKey = true;
                            if (TQueryDefaults.PrimaryKeyIsAutoIncrementByDefault) tableField.AutoIncrement = true;
                            break;
                        case nameof(ForeignKeyAttribute):
                            break;
                        case nameof(FKeyAttribute):
                            break;
                        case nameof(IndexAttribute):
                            break;
                        case nameof(MaxLengthAttribute):
                            break;
                        case nameof(StringLengthAttribute):
                            break;
                        case nameof(TimestampAttribute):
                            break;
                        case nameof(DatabaseGeneratedAttribute):
                            break;
                        case nameof(AutoIncrementAttribute):
                            tableField.AutoIncrement = true;
                            break;
                        case nameof(NotAutoIncrementAttribute):
                            tableField.AutoIncrement = false;
                            break;
                        case nameof(RangeAttribute):
                            break;
                        default:
                            break;
                    }
                }
                List<string> attr = new List<string>();
                if (tableField.PrimaryKey) attr.Add("PRIMARY KEY");
                if (tableField.AutoIncrement) attr.Add("IDENTITY");
                if (tableField.Required) attr.Add("NOT NULL");

                fields.Add($"{tableField.Name} {tableField.DataType} {attr?.Join(" ")}");
            }

        }

        private static void TryCreatingForeigenKey(PropertyInfo field)
        {
            throw new NotImplementedException();
        }

        private static string ModifySql(this Type type, SqlConnection sqlConnection)
        {
            var sqlCommands = new List<string>();
            var addColums = new List<string>();
            var dropColums = new List<string>();

            sqlConnection.Open();
            var dtCols = sqlConnection.GetSchema("Columns", new[] { sqlConnection.Database, null, type.Name });
            sqlConnection.Close();
            var typeProps = type.GetProperties().ToList();
            var tableProps = new Dictionary<string, Type>();
            foreach (DataRow field in dtCols.Rows)
            {
                var serverType = ExpressionToSQLExtensions.GetDNetType((string)field[7]);
                var typeProp = typeProps.FirstOrDefault(x => x.Name == (string)field[3]);
                tableProps.Add((string)field[3], serverType);
                //if field not found in code table
                if (typeProp != null)
                    dropColums.Add(typeProp.Name);
                //if datatype in code table is different
                if (typeProp.PropertyType != serverType)
                    sqlCommands.Add($"ALTER TABLE {type.Name}{Environment.NewLine}ALTER COLUMN {(string)field[3]} {typeProp.PropertyType.ToSqlDbTypeInternal()};");
            }
            foreach (PropertyInfo property in typeProps)
            {
                //if field added in code
                if (tableProps.FirstOrDefault(x => x.Key == property.Name).Key == null)
                    addColums.Add($"{property.Name} {property.PropertyType.ToSqlDbTypeInternal()}");
            }
            if (addColums.Any())
                sqlCommands.Add($"ALTER TABLE {type.Name}{Environment.NewLine} ADD {addColums.Join(", ")};");
            if (dropColums.Any())
                sqlCommands.Add($"ALTER TABLE {type.Name}{Environment.NewLine} DROP COLUMN {dropColums.Join(", ")};");

            return sqlCommands.Join(Environment.NewLine);
        }
        private static string CreateIfNotExistsSql<T>(this TQuery<T> tQuery)
        {
            return $"IF OBJECT_ID('{tQuery.TableName}', 'U') IS NULL {Environment.NewLine}{CreateSql(tQuery)};";
        }
        private static string CreateIfNotExistsSql(this TQueryable tQuery)
        {
            return $"IF OBJECT_ID('{tQuery.TableName}', 'U') IS NULL {Environment.NewLine}{CreateSql(tQuery)};";
        }
        private static string DropSql<T>(this TQuery<T> tQuery)
        {
            var table = tQuery.TableName;
            var props = tQuery.TableType.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add(field.Name + " " + field.PropertyType.ToSqlDbTypeInternal()); }
            return $"DROP TABLE {table} ;{Environment.NewLine}";
        }

        private static string DropSql(this TQueryable tQuery)
        {
            var table = tQuery.TableName;
            var props = tQuery.TableType.GetProperties().ToList();
            List<string> fields = new List<string>();
            foreach (PropertyInfo field in props) { fields.Add(field.Name + " " + field.PropertyType.ToSqlDbTypeInternal()); }
            return $"DROP TABLE {table} ;{Environment.NewLine}";
        }

        /// <summary>
        /// Creates all Tables on the server database, based on the code classes with [Table] attribute.
        /// Supported attributes are: [Key] for Primary Key, [AutoIncrement] for Auto Increment property, [Required] for Not Null property.
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryDatabase"/> to perform the Create All Tables command.
        /// </param>
        public static void CreateAllTables(this TQueryDatabase tQuery)
        {
            List<Type> types = new List<Type>();
            foreach (Type type in Assembly
            .GetEntryAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            foreach (var t in types)
            {
                TQueryable query = new TQueryable(t, tQuery.SqlDialect);
                tQuery.SqlString += query.CreateSql() + Environment.NewLine;
            }
            tQuery.SqlConnection.Execute(tQuery.SqlString);
        }

        /// <summary>
        /// Creates all Tables on the server database, based on the code classes with [Table] attribute.
        /// Supported attributes are: [Key] for Primary Key, [AutoIncrement] for Auto Increment property, [Required] for Not Null property.
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryDatabaseExtended"/> to perform the Create All Tables command.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabaseExtended"/> instance, which the SQL command will create all Tables on the database, and <see cref="TQueryExecute{T}.Execute"/> will execute the Create All Tables command.
        /// </returns>
        public static TQueryDatabaseExtended CreateAllTables(this TQueryDatabaseExtended tQuery)
        {
            List<Type> types = new List<Type>();
            foreach (Type type in Assembly
            .GetEntryAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            foreach (var t in types)
            {
                TQueryable query = new TQueryable(t, tQuery.SqlDialect);
                tQuery.SqlString += query.CreateSql() + Environment.NewLine;
            }
            return tQuery;
        }

        /// <summary>
        /// Checks each table if exists on the database, and creates the non-exists Tables on the server database, based on the code classes with [Table] attribute.
        /// Supported attributes are: [Key] for Primary Key, [AutoIncrement] for Auto Increment property, [Required] for Not Null property.
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryDatabase"/> to perform the Create All Tables command.
        /// </param>
        public static void CreateAllTablesIfNotExists(this TQueryDatabase tQuery)
        {
            List<Type> types = new List<Type>();
            foreach (Type type in Assembly
            .GetEntryAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            foreach (var t in types)
            {
                TQueryable query = new TQueryable(t, tQuery.SqlDialect);
                tQuery.SqlString += query.CreateIfNotExistsSql() + Environment.NewLine;
            }
            tQuery.SqlConnection.Execute(tQuery.SqlString);
        }

        /// <summary>
        /// Checks each table if exists on the database, and creates the non-exists Tables on the server database, based on the code classes with [Table] attribute.
        /// Supported attributes are: [Key] for Primary Key, [AutoIncrement] for Auto Increment property, [Required] for Not Null property.
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryDatabaseExtended"/> to perform the Create All Tables command.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabaseExtended"/> instance, which the SQL command will create all non-exists Tables on the database, and <see cref="TQueryExecute{T}.Execute"/> will execute the Create All Tables command.
        /// </returns>
        public static TQueryDatabaseExtended CreateAllTablesIfNotExists(this TQueryDatabaseExtended tQuery)
        {
            List<Type> types = new List<Type>();
            foreach (Type type in Assembly
            .GetEntryAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            foreach (var t in types)
            {
                TQueryable query = new TQueryable(t, tQuery.SqlDialect);
                tQuery.SqlString += query.CreateIfNotExistsSql() + Environment.NewLine;
            }
            return tQuery;
        }

        /// <summary>
        /// Removes all Tables on the server database, based on the code classes with [Table] attribute.
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryDatabase"/> to perform the Drop All Tables command.
        /// </param>
        public static int DropAllTables(this TQueryDatabase tQuery)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in Assembly
            .GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            foreach (var t in types)
            {
                TQueryable query = new TQueryable(t, tQuery.SqlDialect);
                tQuery.SqlString += query.DropSql() + Environment.NewLine;
            }
            return tQuery.SqlConnection.Execute(tQuery.SqlString);
        }

        /// <summary>
        /// Removes all Tables on the server database, based on the code classes with [Table] attribute.
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryDatabase"/> to perform the Drop All Tables command.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabaseExtended"/> instance, which the SQL command will remove all Tables on the database, and <see cref="TQueryExecute{T}.Execute"/> will execute the Drop All Tables command.
        /// </returns>
        public static TQueryDatabaseExtended DropAllTables(this TQueryDatabaseExtended tQuery)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in Assembly
            .GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                    types.Add(type);
            }

            foreach (var t in types)
            {
                TQueryable query = new TQueryable(t, tQuery.SqlDialect);
                tQuery.SqlString += query.DropSql() + Environment.NewLine;
            }
            return tQuery;
        }

        /// <summary>
        /// Returns a list of all table defenitions from the server database.
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryDatabase"/> to get all database tables and their fields.
        /// </param>
        /// <returns>
        /// An <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> which the Key is the table name, and the Value is an Dynamic Class as an <see cref="System.Dynamic.DynamicObject"/> including the table defenition, fields, and attributes. 
        /// </returns>
        public static Dictionary<string, object> GetAllServerDbTablesType(this TQueryDatabase tQuery)
        {
            tQuery.SqlConnection.Open();
            DataTable t = tQuery.SqlConnection.GetSchema("Tables");
            tQuery.SqlConnection.Close();
            var tables = new Dictionary<string, object>();
            foreach (DataRow row in t.Rows)
            {
                string table = (string)row[2];
                tQuery.SqlConnection.Open();
                var dtCols = tQuery.SqlConnection.GetSchema("Columns", new[] { tQuery.SqlConnection.Database, null, table });
                tQuery.SqlConnection.Close();
                var fields = new List<Field>();
                foreach (DataRow field in dtCols.Rows)
                {
                    fields.Add(new Field((string)field[3], ExpressionToSQLExtensions.GetDNetType((string)field[7])));
                }
                object obj = new ClassBuilder(fields);
                tables.Add(table, obj);
            }
            return tables;
        }

        /// <summary>
        /// Compares the server database and the code classes with [Table] attribute, checks each table and their fields and properties.
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryDatabase"/> to compare the server database with all tables on the code classes.  
        /// </param>
        /// <returns>
        /// True if all tables, fields, and properties on the server database matches the code classes with [Table] attribute. Otherwise, returns false.
        /// </returns>
        public static bool IsEqualServerDbToCodeDb(this TQueryDatabase tQuery)
        {
            tQuery.SqlConnection.Open();
            DataTable t = tQuery.SqlConnection.GetSchema("Tables");
            tQuery.SqlConnection.Close();
            var types = new List<Type>();
            var tables = new List<string>();
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
                tables.Add(table);
                tQuery.SqlConnection.Open();
                var dtCols = tQuery.SqlConnection.GetSchema("Columns", new[] { tQuery.SqlConnection.Database, null, table });
                tQuery.SqlConnection.Close();
                var typeProps = types.FirstOrDefault(x => x.Name == table).GetProperties().ToList();
                var tableProps = new Dictionary<string, Type>();
                foreach (DataRow field in dtCols.Rows)
                {
                    var serverType = ExpressionToSQLExtensions.GetDNetType((string)field[7]);
                    var typeProp = typeProps.FirstOrDefault(x => x.Name == (string)field[3]);
                    tableProps.Add((string)field[3], serverType);
                    bool isEqual = typeProp == null || typeProp.PropertyType != serverType;
                    if (!isEqual) return false;
                }
                foreach (PropertyInfo property in typeProps)
                {
                    if (tableProps.FirstOrDefault(x => x.Key == property.Name).Key == null)
                        return false;
                }
            }
            foreach (Type type in types)
            {
                if (tables.Where(x => x == type.Name) == null)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Compares the server database and the code classes with [Table] attribute, checks each table and their fields and properties.
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryDatabase"/> to compare the server database with all tables on the code classes.  
        /// </param>
        /// <returns>
        /// An list of <see cref="CompareDb"/> objects, which includes all differences between the server database and the code classes with [Table] attribute.
        /// </returns>
        public static List<CompareDb> GetDiffServerDbToCodeDb(this TQueryDatabase tQuery)
        {
            tQuery.SqlConnection.Open();
            DataTable t = tQuery.SqlConnection.GetSchema("Tables");
            tQuery.SqlConnection.Close();
            var types = new List<Type>();
            var tables = new List<string>();
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
                tables.Add(table);
                tQuery.SqlConnection.Open();
                var dtCols = tQuery.SqlConnection.GetSchema("Columns", new[] { tQuery.SqlConnection.Database, null, table });
                tQuery.SqlConnection.Close();
                var typeProps = types.FirstOrDefault(x => x.Name == table).GetProperties().ToList();
                var tableProps = new Dictionary<string, Type>();
                foreach (DataRow field in dtCols.Rows)
                {
                    var serverType = ExpressionToSQLExtensions.GetDNetType((string)field[7]);
                    var typeProp = typeProps.FirstOrDefault(x => x.Name == (string)field[3]);
                    tableProps.Add((string)field[3], serverType);
                    string diffDesc = typeProp == null ? $"Field name '{(string)field[3]}' from server cannot be found in code table" :
                    typeProp.PropertyType != serverType ? $"Field type '{serverType.FullName}' from server is different from '{typeProp.PropertyType.FullName}'" : null;
                    if (!String.IsNullOrEmpty(diffDesc))
                        diff.Add(new CompareDb(table, (string)field[3], serverType, typeProp?.Name, typeProp?.PropertyType, diffDesc));
                }
                foreach (PropertyInfo property in typeProps)
                {
                    if (tableProps.FirstOrDefault(x => x.Key == property.Name).Key == null)
                        diff.Add(new CompareDb(table, null, null, property.Name, property.PropertyType, $"Field name '{property.Name}' from code table cannot be found in server table"));
                }

            }
            foreach (Type type in types)
            {
                if (tables.Where(x => x == type.Name) == null)
                    diff.Add(new CompareDb(type.Name, null, null, null, null, $"Table name '{type.Name}' from code table cannot be found in server database"));
            }
            return diff;
        }

        /// <summary>
        /// Compares the server table and the code class with that table name, checks each field and their properties.
        /// </summary>
        /// <typeparam name="Table">The type of the records of table class. need to be a class with the [Table("")] attribute.</typeparam>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> to perform the Drop Table command.
        /// </param>
        /// <returns>
        /// An list of <see cref="TQueryTableExtensions.CompareDb"/> objects, which includes all differences between the server database and the code classes with [Table] attribute.
        /// </returns>
        public static List<CompareDb> GetDiffServerTableToCodeTable<Table>(this TQueryable<Table> tQuery)
        {
            var diff = new List<CompareDb>();
            tQuery.SqlConnection.Open();
            var dtCols = tQuery.SqlConnection.GetSchema("Columns", new[] { tQuery.SqlConnection.Database, null, typeof(Table).Name });
            tQuery.SqlConnection.Close();
            var typeProps = typeof(Table).GetProperties().ToList();
            var tableProps = new Dictionary<string, Type>();
            foreach (DataRow field in dtCols.Rows)
            {
                var serverType = ExpressionToSQLExtensions.GetDNetType((string)field[7]);
                var typeProp = typeProps.FirstOrDefault(x => x.Name == (string)field[3]);
                tableProps.Add((string)field[3], serverType);
                string diffDesc = typeProp == null ? $"Field name '{(string)field[3]}' from server cannot be found in code table" :
                typeProp.PropertyType != serverType ? $"Field type '{serverType.FullName}' from server is different from '{typeProp.PropertyType.FullName}'" : null;
                if (!String.IsNullOrEmpty(diffDesc))
                    diff.Add(new CompareDb(typeof(Table).Name, (string)field[3], serverType, typeProp?.Name, typeProp?.PropertyType, diffDesc));
            }
            foreach (PropertyInfo property in typeProps)
            {
                if (tableProps.FirstOrDefault(x => x.Key == property.Name).Key == null)
                    diff.Add(new CompareDb(typeof(Table).Name, null, null, property.Name, property.PropertyType, $"Field name '{property.Name}' from code table cannot be found in server table"));
            }
            return diff;
        }
        /// <summary>
        /// An object that contains comperasion information between code table fields, properties, and server database table defenitions. 
        /// </summary>
        public class CompareDb
        {
            /// <summary>
            /// Table name
            /// </summary>
            public string TableName { get; set; }
            /// <summary>
            /// Field name on server
            /// </summary>
            public string ServerField { get; set; }
            /// <summary>
            /// Field type on server
            /// </summary>
            public Type ServerType { get; set; }
            /// <summary>
            /// Field name on code class
            /// </summary>
            public string CodeField { get; set; }
            /// <summary>
            /// Field type on code class
            /// </summary>
            public Type CodeType { get; set; }
            /// <summary>
            /// Description of difference if any.
            /// </summary>
            public string Description { get; set; }
            internal CompareDb(string table, string serverFld, Type serverType, string codeFld, Type codeType, string desc)
            {
                TableName = table; ServerField = serverFld; ServerType = serverType; CodeField = codeFld; CodeType = codeType; Description = desc;
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
