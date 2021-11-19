using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Reflection;
using static Dapper.TQuery.Development.TQueryExceptions;

namespace Dapper.TQuery.Development
{
    /// <summary>
    /// The start point of using this library.
    /// <br/>
    /// Initialize a new <see cref="TQueryable{T}"/> instanse, to query and/or modify the table with TQuery method extensions.
    /// Or <see cref="TQueryableExtended{T}"/> instanse, for more advanced options.
    /// <br/>
    /// Initialize a new <see cref="TQueryDatabase"/> instanse, to modify the Database table defenitions with TQuery method extensions.
    /// Or <see cref="TQueryDatabaseExtended"/> instanse, for more advanced options.
    /// </summary>
    public static class TQueryStartExtensions
    {
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryable{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryable{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryable<Table> TQuery<Table>(this SqlConnection sqlConnection)
        {
            SqlDialect sqlDialect = TQueryDefaults.SqlDialect;
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            TQueryable<Table> query = new TQueryable<Table>(sqlConnection, sqlDialect);
            return query;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryable{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryable{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryable<Table> TQuery<Table>(this SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            TQueryable<Table> query = new TQueryable<Table>(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryableExtended{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableExtended{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryableExtended<Table> TQueryExtended<Table>(this SqlConnection sqlConnection)
        {
            SqlDialect sqlDialect = TQueryDefaults.SqlDialect;
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            TQueryableExtended<Table> query = new TQueryableExtended<Table>(sqlConnection, sqlDialect);
            return query;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryableExtended{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableExtended{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryableExtended<Table> TQueryExtended<Table>(this SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));


            TQueryableExtended<Table> query = new TQueryableExtended<Table>(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabase"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabase"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryDatabase TQueryDb(this SqlConnection sqlConnection)
        {
            SqlDialect sqlDialect = TQueryDefaults.SqlDialect;
            TQueryDatabase query = new TQueryDatabase(sqlConnection, sqlDialect);
            return query;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabase"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabase"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryDatabase TQueryDb(this SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            TQueryDatabase query = new TQueryDatabase(sqlConnection, sqlDialect);
            return query;
        }


        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabaseExtended"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryDatabaseExtended TQueryDbExtended(this SqlConnection sqlConnection)
        {
            SqlDialect sqlDialect = TQueryDefaults.SqlDialect;
            TQueryDatabaseExtended query = new TQueryDatabaseExtended(sqlConnection, sqlDialect);
            return query;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabaseExtended"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryDatabaseExtended TQueryDbExtended(this SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            TQueryDatabaseExtended query = new TQueryDatabaseExtended(sqlConnection, sqlDialect);
            return query;
        }



        /// <summary>
        /// Modifies manually the TQuery SQL command with any given string.<br/>
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="sql"> A given string for SQL command.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableExtended{T}"/> instanse with the updated SQL command.
        /// </returns>
        public static TQueryableExtended<Table> ModifySqlString<Table>(this TQueryableExtended<Table> tQuery, string sql)
        {
            tQuery.SqlString = sql;
            return tQuery;
        }

        /// <summary>
        /// Replaces parts of the TQuery SQL command.<br/>
        /// Use this method to replace any part of the SQL string according to the Database language.
        /// </summary>
        /// <param name="tQuery">
        /// An <see cref="TQueryable{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        /// <returns>
        /// An <see cref="TQueryableExtended{T}"/> instanse with the updated SQL command.
        /// </returns>
        public static TQueryableExtended<Table> ReplaceInSqlString<Table>(this TQueryableExtended<Table> tQuery, String oldValue, String newValue)
        {
            tQuery.SqlString = tQuery.SqlString.Replace(oldValue, newValue);
            return tQuery;
        }

    }

    /// <summary>
    /// The start point of using this library.
    /// <br/>
    /// Initialize a new <see cref="TQueryable{T}"/> instanse, to query and/or modify the table with TQuery method extensions.
    /// Or <see cref="TQueryableExtended{T}"/> instanse, for more advanced options.
    /// <br/>
    /// Initialize a new <see cref="TQueryDatabase"/> instanse, to modify the Database table defenitions with TQuery method extensions.
    /// Or <see cref="TQueryDatabaseExtended"/> instanse, for more advanced options.
    /// </summary>
    public static class TQueryStartMethods
    {
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryable{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryable{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryable<Table> TQueryInit<Table>(SqlConnection sqlConnection)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            TQueryable<Table> query = new TQueryable<Table>(sqlConnection, TQueryDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryableExtended{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableExtended{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryableExtended<Table> TQueryExtendedInit<Table>(SqlConnection sqlConnection)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));


            TQueryableExtended<Table> query = new TQueryableExtended<Table>(sqlConnection, TQueryDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabase"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabase"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryDatabase TQueryDbInit(SqlConnection sqlConnection)
        {
            TQueryDatabase query = new TQueryDatabase(sqlConnection, TQueryDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabaseExtended"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryDatabaseExtended TQueryDbExtendedInit(SqlConnection sqlConnection)
        {
            TQueryDatabaseExtended query = new TQueryDatabaseExtended(sqlConnection, TQueryDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryable{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryable{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryable<Table> TQueryInit<Table>(string connectionString)
        {
            TQueryable<Table> query = new TQueryable<Table>(connectionString, TQueryDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryableExtended{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableExtended{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryableExtended<Table> TQueryExtendedInit<Table>(string connectionString)
        {
            TQueryableExtended<Table> query = new TQueryableExtended<Table>(connectionString, TQueryDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabase"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryDatabase TQueryDbInit(string connectionString)
        {
            TQueryDatabase query = new TQueryDatabase(connectionString, TQueryDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabaseExtended"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryDatabaseExtended TQueryDbExtendedInit(string connectionString)
        {
            TQueryDatabaseExtended query = new TQueryDatabaseExtended(connectionString, TQueryDefaults.SqlDialect);
            return query;
        }


        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryable{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryable{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryable<Table> TQueryInit<Table>(SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));


            TQueryable<Table> query = new TQueryable<Table>(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryableExtended{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableExtended{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryableExtended<Table> TQueryExtendedInit<Table>(SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));


            TQueryableExtended<Table> query = new TQueryableExtended<Table>(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabase"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabase"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryDatabase TQueryDbInit(SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            TQueryDatabase query = new TQueryDatabase(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabaseExtended"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryDatabaseExtended TQueryDbExtendedInit(SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            TQueryDatabaseExtended query = new TQueryDatabaseExtended(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryable{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryable{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryable<Table> TQueryInit<Table>(string connectionString, SqlDialect sqlDialect)
        {
            TQueryable<Table> query = new TQueryable<Table>(connectionString, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryableExtended{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryableExtended{T}"/> instanse which will be used to query and/or modify the table with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryableExtended<Table> TQueryExtendedInit<Table>(string connectionString, SqlDialect sqlDialect)
        {
            TQueryableExtended<Table> query = new TQueryableExtended<Table>(connectionString, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabase"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions.
        /// </returns>
        ///         
        public static TQueryDatabase TQueryDbInit(string connectionString, SqlDialect sqlDialect)
        {
            TQueryDatabase query = new TQueryDatabase(connectionString, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="TQueryDatabaseExtended"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="TQueryDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with TQuery method extensions with advanced options of the TQuery library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static TQueryDatabaseExtended TQueryDbExtendedInit(string connectionString, SqlDialect sqlDialect)
        {
            TQueryDatabaseExtended query = new TQueryDatabaseExtended(connectionString, sqlDialect);
            return query;
        }

    }

}
