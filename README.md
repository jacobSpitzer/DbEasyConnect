# Dapper.TQuery Library

[![NuGet](https://img.shields.io/nuget/v/Dapper.TQuery.svg)](https://www.nuget.org/packages/Dapper.TQuery)
[![NuGet](https://img.shields.io/nuget/dt/Dapper.TQuery.svg)](https://www.nuget.org/packages/Dapper.TQuery)

## Description
``Dapper.TQuery`` is a c# based [NuGet Library](https://github.com/jacobSpitzer/Dapper.TQuery) package that provides database connection services with the focus on the following three goals:
* get the high fastest performance
* avoid configuration the most possible
* support strong typed coding and avoid bugs and spelling mistakes.

## background:
I was using [Entity Framework](https://github.com/dotnet/ef6) for a long time and founded difficult to configure the database schema, binding, migrations, and repositories in addition to writing Data Models.

I also found it difficult to modify any table / filed in the database after the first creation and configuration, that requires to update the database directly, and use migration.
[Look here a great article by Tim Corey about why to not use Entity Framework](https://www.iamtimcorey.com/blog/137806/entity-framework). including the performance speed.

And then, I found out about the [Dapper library](https://github.com/DapperLib/Dapper), a great open-source library created by the Stack Overflow team.

But, I found it even hard, because of missing a lot of features provided by EF. like CRUD features, working with batch insert/update, and the need of writing SQL language, and remember all table/field names, without any warning on spelling mistakes, and missing a LINQ QUERYABLE option, so I need to download the whole table before querying with linq, uses extra memory and lowers the performance.

There are alot of Dapper Extensions, where each extension has his own feature, and his limits and configuration. I was looking for a more simple solution with less configuration, less required packages, simple, and queryable method extensions that are most simular to Linq.

So I have started to create a new library that is based on ADO.NET, and Dapper. But gives you a strong typed coding experience, with all the features listed below.

## Features
* Using Dapper as the object mapper, with high fast performance
* using strong typed coding to querying the database simular to entity framework, to avoid spelling mistakes
* without any extra code for creating / migration. You just need to create classes with [Table] attribute, and some Data Annotations Attributes.
* use Dapper with LINQ QUERYABLE features. With the most (but not all) common features of linq extensions.
* use the fastest easyest way for CRUD (Create, Read, Update, and Delete) operations, Find by ID, batch insert/update/delete even with Entity List.

## Beta
This project is still on beta version, since not all planned features are already finished. but it's tested and ready to use as it is.
The following planned features are still not finished:
#### Support for other Database platforms.
The current version 0.2.1 is supporting only MS-SQL SERVER. 

Planned Support for LocalDb, Ms Sql Server, MySql, SqLite, PostgreSql.

PS: most of the methods will already work on the other common Database Platforms.

Here are a list for all known methods that will work only with MS-SQL SERVER and the Explanation of the limit.

Method       | Limit         | Explanation
------------ | ------------- | ------------------------------
CreateTable | limit | exp
#### Coming Methods
GroupBy, Add/Modify/Delete Column,  
#### Support to specifieng a table name different than the class name

## Installation
Install with the Package Manager Console in Visual Studio (PowerShell)

```sh
PM> Install-Package Dapper.TQuery

```

install via Dotnet CLI

```sh
dotnet add package Dapper.TQuery

```


## Basic Usage

First create a class
```sh
[Table("Sample")]
public class Sample
{
    public int Id { get; set; }
    public string Field { get; set; }
}
```

and then:

```sh
// Create a SqlConnection
var con = new SqlConnection("Server=YOUR.SERVER.COM; Database=DATABASE_NAME; User ID=USER_ID;Password=*******; Trusted_Connection=False; MultipleActiveResultSets=True");
// Get all records from the Sample Table
con.TQuery<Sample>().ToList();          
```

#### How to write a Table Class

In order to get defined by the Library, you need to write all Table classes with the [Table("TableName")] Attribute. only classes within your current project assembly will work.
Add the relevant references for that. like:

```sh
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
```

Please note: currently is no matter the table name you specify in the Table attribute. the library will use the Class name to create/find the table on the database server.

##### Data Types


##### Data Annotations Attributes

You can use some attributes above each field to specify more properties on the field:

Attribute | Sql Property | MS-SQL SERVER | MySql | SqLite | PostgreSql | LocalDb
----------|--------------|---------------|-------|--------|------------|---------|
[Required] | NOT NULL |
[Index] | |
[Index(IsUnique=true)] | UNIQUE
[Key] | PRIMARY KEY | 
[ForeignKey("TableName")] | FOREIGN KEY |
[AutoIncrement] | 

#### Create / Modify Tables

#### Code Tables VS Server Database

#### Supported Linq Methods

#### Get, Find, Insert, Update, Delete

#### InsertList, UpdateList, DeleteList

#### Join Multiple Tables

#### Read / Update generated SQL commands
