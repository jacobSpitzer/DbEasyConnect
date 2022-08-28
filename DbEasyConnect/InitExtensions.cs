using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Reflection;
using static DbEasyConnect.Tools.DbEcExceptions;

namespace DbEasyConnect
{
    /// <summary>
    /// The start point of using this library.
    /// <br/>
    /// Initialize a new <see cref="DbEc{T}"/> instanse, to query and/or modify the table with DbEasyConnect method extensions.
    /// Or <see cref="DbEcExtended{T}"/> instanse, for more advanced options.
    /// <br/>
    /// Initialize a new <see cref="IDbEcDatabase"/> instanse, to modify the Database table defenitions with DbEasyConnect method extensions.
    /// Or <see cref="IDbEcDatabaseExtended"/> instanse, for more advanced options.
    /// </summary>
    public static class InitExtensions
    {
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="DbEc{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="DbEc{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static DbEc<Table> IDbEc<Table>(this SqlConnection sqlConnection)
        {
            SqlDialect sqlDialect = DbEcDefaults.SqlDialect;
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            DbEc<Table> query = new DbEc<Table>(sqlConnection, sqlDialect);
            return query;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbEc{T}"/> class when taken a class type that contains the [Table] attribute.
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
        /// An <see cref="DbEc{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static DbEc<Table> IDbEc<Table>(this SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            DbEc<Table> query = new DbEc<Table>(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="DbEcExtended{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="DbEcExtended{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static DbEcExtended<Table> IDbEcExtended<Table>(this SqlConnection sqlConnection)
        {
            SqlDialect sqlDialect = DbEcDefaults.SqlDialect;
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            DbEcExtended<Table> query = new DbEcExtended<Table>(sqlConnection, sqlDialect);
            return query;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbEcExtended{T}"/> class when taken a class type that contains the [Table] attribute.
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
        /// An <see cref="DbEcExtended{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static DbEcExtended<Table> IDbEcExtended<Table>(this SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            
                
            DbEcExtended<Table> query = new DbEcExtended<Table>(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabase"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabase"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static IDbEcDatabase IDbEcDb(this SqlConnection sqlConnection)
        {
            SqlDialect sqlDialect = DbEcDefaults.SqlDialect;
            IDbEcDatabase query = new IDbEcDatabase(sqlConnection, sqlDialect);
            return query;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabase"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabase"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static IDbEcDatabase IDbEcDb(this SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            IDbEcDatabase query = new IDbEcDatabase(sqlConnection, sqlDialect);
            return query;
        }


        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabaseExtended"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static IDbEcDatabaseExtended IDbEcDbExtended(this SqlConnection sqlConnection)
        {
            SqlDialect sqlDialect = DbEcDefaults.SqlDialect;
            IDbEcDatabaseExtended query = new IDbEcDatabaseExtended(sqlConnection, sqlDialect);
            return query;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabaseExtended"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static IDbEcDatabaseExtended IDbEcDbExtended(this SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            IDbEcDatabaseExtended query = new IDbEcDatabaseExtended(sqlConnection, sqlDialect);
            return query;
        }



        /// <summary>
        /// Modifies manually the DbEasyConnect SQL command with any given string.<br/>
        /// </summary>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="sql"> A given string for SQL command.
        /// </param>
        /// <returns>
        /// An <see cref="DbEcExtended{T}"/> instanse with the updated SQL command.
        /// </returns>
        public static DbEcExtended<Table> ModifySqlString<Table>(this DbEcExtended<Table> dbEcQuery, string sql)
        {
            dbEcQuery.SqlString = sql;
            return dbEcQuery;
        }

        /// <summary>
        /// Replaces parts of the DbEasyConnect SQL command.<br/>
        /// Use this method to replace any part of the SQL string according to the Database language.
        /// </summary>
        /// <param name="dbEcQuery">
        /// An <see cref="DbEc{T}"/> that contains the queryable table to apply the predicate to.
        /// </param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        /// <returns>
        /// An <see cref="DbEcExtended{T}"/> instanse with the updated SQL command.
        /// </returns>
        public static DbEcExtended<Table> ReplaceInSqlString<Table>(this DbEcExtended<Table> dbEcQuery, String oldValue, String newValue)
        {
            dbEcQuery.SqlString = dbEcQuery.SqlString.Replace(oldValue, newValue);
            return dbEcQuery;
        }

    }

    /// <summary>
    /// The start point of using this library.
    /// <br/>
    /// Initialize a new <see cref="DbEc{T}"/> instanse, to query and/or modify the table with DbEasyConnect method extensions.
    /// Or <see cref="DbEcExtended{T}"/> instanse, for more advanced options.
    /// <br/>
    /// Initialize a new <see cref="IDbEcDatabase"/> instanse, to modify the Database table defenitions with DbEasyConnect method extensions.
    /// Or <see cref="IDbEcDatabaseExtended"/> instanse, for more advanced options.
    /// </summary>
    public static class IDbEcStartMethods
    {
        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="DbEc{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="DbEc{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static DbEc<Table> IDbEcInit<Table>(SqlConnection sqlConnection)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            DbEc<Table> query = new DbEc<Table>(sqlConnection, DbEcDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="DbEcExtended{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="DbEcExtended{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static DbEcExtended<Table> IDbEcExtendedInit<Table>(SqlConnection sqlConnection)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            
                
            DbEcExtended<Table> query = new DbEcExtended<Table>(sqlConnection, DbEcDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabase"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabase"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static IDbEcDatabase IDbEcDbInit(SqlConnection sqlConnection)
        {
            IDbEcDatabase query = new IDbEcDatabase(sqlConnection, DbEcDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabaseExtended"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static IDbEcDatabaseExtended IDbEcDbExtendedInit(SqlConnection sqlConnection)
        {
            IDbEcDatabaseExtended query = new IDbEcDatabaseExtended(sqlConnection, DbEcDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="DbEc{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="DbEc{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static DbEc<Table> IDbEcInit<Table>(string connectionString)
        {
            DbEc<Table> query = new DbEc<Table>(connectionString, DbEcDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="DbEcExtended{T}"/> class when taken a class type that contains the [Table] attribute.
        /// </summary>
        /// <typeparam name="Table">
        /// The type of the records of table class. need to be a class with the [Table] attribute.
        /// </typeparam>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="DbEcExtended{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static DbEcExtended<Table> IDbEcExtendedInit<Table>(string connectionString)
        {
            DbEcExtended<Table> query = new DbEcExtended<Table>(connectionString, DbEcDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabase"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static IDbEcDatabase IDbEcDbInit(string connectionString)
        {
            IDbEcDatabase query = new IDbEcDatabase(connectionString, DbEcDefaults.SqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabaseExtended"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static IDbEcDatabaseExtended IDbEcDbExtendedInit(string connectionString)
        {
            IDbEcDatabaseExtended query = new IDbEcDatabaseExtended(connectionString, DbEcDefaults.SqlDialect);
            return query;
        }


        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="DbEc{T}"/> class when taken a class type that contains the [Table] attribute.
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
        /// An <see cref="DbEc{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static DbEc<Table> IDbEcInit<Table>(SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            
                
            DbEc<Table> query = new DbEc<Table>(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="DbEcExtended{T}"/> class when taken a class type that contains the [Table] attribute.
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
        /// An <see cref="DbEcExtended{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static DbEcExtended<Table> IDbEcExtendedInit<Table>(SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            if (Attribute.IsDefined(typeof(Table), typeof(TableAttribute)) == false)
                throw new MissingTableAttributeException(typeof(Table));
            
                
            DbEcExtended<Table> query = new DbEcExtended<Table>(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabase"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabase"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static IDbEcDatabase IDbEcDbInit(SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            IDbEcDatabase query = new IDbEcDatabase(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabaseExtended"/> class.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SqlConnection to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static IDbEcDatabaseExtended IDbEcDbExtendedInit(SqlConnection sqlConnection, SqlDialect sqlDialect)
        {
            IDbEcDatabaseExtended query = new IDbEcDatabaseExtended(sqlConnection, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="DbEc{T}"/> class when taken a class type that contains the [Table] attribute.
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
        /// An <see cref="DbEc{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static DbEc<Table> IDbEcInit<Table>(string connectionString, SqlDialect sqlDialect)
        {
            DbEc<Table> query = new DbEc<Table>(connectionString, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="DbEcExtended{T}"/> class when taken a class type that contains the [Table] attribute.
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
        /// An <see cref="DbEcExtended{T}"/> instanse which will be used to query and/or modify the table with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static DbEcExtended<Table> IDbEcExtendedInit<Table>(string connectionString, SqlDialect sqlDialect)
        {
            DbEcExtended<Table> query = new DbEcExtended<Table>(connectionString, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabase"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions.
        /// </returns>
        ///         
        public static IDbEcDatabase IDbEcDbInit(string connectionString, SqlDialect sqlDialect)
        {
            IDbEcDatabase query = new IDbEcDatabase(connectionString, sqlDialect);
            return query;
        }

        //TODO add Exception for wrong type selection, type that does not have a Table attribute.
        /// <summary>
        /// Initializes a new instance of the <see cref="IDbEcDatabaseExtended"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the Server Database.
        /// </param>
        /// <param name="sqlDialect">
        /// The relevant <see cref="SqlDialect"/> for the current database. Available options: SQL Server, MySQL, Oracle, SQLite, and PostgreSQL.
        /// these are all different databases that have their own slightly different SQL dialects. 
        /// </param>
        /// <returns>
        /// An <see cref="IDbEcDatabaseExtended"/> instanse which will be used to modify the Database table defenitions with DbEasyConnect method extensions with advanced options of the DbEasyConnect library, to read/modify the generated SQL command, and more.
        /// </returns>
        ///         
        public static IDbEcDatabaseExtended IDbEcDbExtendedInit(string connectionString, SqlDialect sqlDialect)
        {
            IDbEcDatabaseExtended query = new IDbEcDatabaseExtended(connectionString, sqlDialect);
            return query;
        }

    }

}
