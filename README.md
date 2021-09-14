# Dapper.TQuery Library

[![Latest Version](https://img.shields.io/nuget/v/Dapper.TQuery.svg)](https://www.nuget.org/packages/Dapper.TQuery)
[![NuGet](https://img.shields.io/nuget/dt/Dapper.TQuery.svg)](https://www.nuget.org/packages/Dapper.TQuery)

## Description
``Dapper.TQuery`` is a c# based [NuGet Library](https://github.com/jacobSpitzer/Dapper.TQuery) package that provides database connection services with the focus on the following three goals:
* Get the fastest performance
* Avoid configuration as much as possible
* Support strong typed coding and avoid bugs and spelling mistakes.

## background:
I was using [Entity Framework](https://github.com/dotnet/ef6) for a long time and found it difficult to configure the database schema, binding, migrations, and repositories in addition to writing Data Models.

I also found it difficult to modify any table / field in the database after the first creation and configuration, that requires to update the database directly, and use migration.

[See a great article by Tim Corey about using Entity Framework](https://www.iamtimcorey.com/blog/137806/entity-framework). including the performance speed.

And then, I found out about the [Dapper library](https://github.com/DapperLib/Dapper), a great open-source library created by the Stack Overflow team.

But, even that I found hard to use, because it's missing a lot of features provided by EF. like CRUD features, working with batch insert/update, and it has the need of writing SQL and remember all table/field names, without any warnings on spelling mistakes, and missing a LINQ QUERYABLE option, so I need to download the whole table before querying with linq, which uses extra memory and lowers the performance.

There are alot of Dapper Extensions, where each extension has their own set of features, and thier limits and configurations. I was looking for a simpler solution with less configurations, less required packages, simple, and queryable method extensions that are similar to Linq.

So I started to create a new library that is based on ADO.NET, and Dapper. But gives you a strong typed coding experience, with all the features listed below.

## Features
* Using Dapper as the object mapper, with high fast performance
* Gives a strong typed coding experience, to querying the database similar to entity framework, to avoid spelling mistakes, and to show the table fields while writing the code.
* Stop writing any extra code for creating / migration. You just need to create classes with [Table] attribute, and some Data Annotations Attributes.
* Use Dapper with LINQ QUERYABLE features. With the most (but not all) common features of Linq extensions.
* Use the fastest easiest way for CRUD (Create, Read, Update, and Delete) operations, Find by ID, batch insert/update/delete even with Entity List.

## Beta
This project is still in beta version, since not all planned features are already finished. but it's tested and ready to use it as is.
The following planned features are still not finished:
#### Support for other Database platforms.
The current version 0.1.2 is supporting only MS-SQL SERVER. 

Planned Support for LocalDb, Ms Sql Server, Oracle, MySql, SqLite, PostgreSql.

P. S. : most of the methods will already work on the other common Database Platforms.

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

Install via Dotnet CLI

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

Please note: currently it doesn't matter what table name you specify in the Table attribute. the library will use the Class name to create/find the table on the database server.

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
[DefaultValue(true)] | DEFAULT |
[StringLength(50)] | LENGTH |

#### Create / Modify Tables

#### Code Tables VS Server Database

#### Supported Linq Methods

#### Get, Find, Insert, Update, Delete

#### InsertList, UpdateList, DeleteList

#### Join Multiple Tables

#### Read / Update generated SQL commands
