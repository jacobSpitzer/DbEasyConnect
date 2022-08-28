# DbEasyConnect Library

[![NuGet](https://img.shields.io/nuget/v/Dapper.TQuery.svg)](https://www.nuget.org/packages/Dapper.TQuery)
[![NuGet](https://img.shields.io/nuget/dt/Dapper.TQuery.svg)](https://www.nuget.org/packages/Dapper.TQuery)

## Description
``DbEasyConnect`` is a c# based [NuGet Library](https://github.com/jacobSpitzer/DbEasyConnect) package that provides database connection services with the focus on the following three goals:
* Get the fastest performance
* Avoid configuration as much as possible
* Support strong typed coding and avoid bugs and spelling mistakes.

## background:
I was using [Entity Framework](https://github.com/dotnet/ef6) for a long time and found it difficult to configure the database schema, binding, migrations, and repositories in addition to writing Data Models.

I also found it difficult to modify any table / field in the database after the first creation and configuration, that requires to update the database directly, and use migration.

[See a great article by Tim Corey about using Entity Framework](https://www.iamtimcorey.com/blog/137806/entity-framework). including the performance speed.

And then, I found out about the [Dapper library](https://github.com/DapperLib/Dapper), a great open-source library created by the Stack Overflow team.

But, even that I found hard to use, because it's missing a lot of features provided by EF. like CRUD features, working with bulk insert/update, and it has the need of writing SQL and remember all table/field names, without any warnings on spelling mistakes, and missing a LINQ QUERYABLE option, so I need to download the whole table before querying with linq, which uses extra memory and lowers the performance.

There are alot of Dapper Extensions, where each extension has their own set of features, and thier limits and configurations. I was looking for a simpler solution with less configurations, less required packages, simple, and queryable method extensions that are similar to Linq.

So I started to create a new library that is based on ADO.NET, and Dapper. But gives you a strong typed coding experience, with all the features listed below.

## Features
* Using Dapper as the object mapper, with high fast performance
* Gives a strong typed coding experience, to querying the database similar to entity framework, to avoid spelling mistakes, and to show the table fields while writing the code.
* Stop writing any extra code for creating / migration. You just need to create classes with [Table] attribute, and some Data Annotations Attributes.
* Use Dapper with LINQ QUERYABLE features. With the most (but not all) common features of Linq extensions.
* Use the fastest easiest way for CRUD (Create, Read, Update, and Delete) operations, Find by ID, bulk insert/update/delete even with Entity List.

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
This section will be updated soon...

#### Coming Methods
GroupBy, Count, Sum, Max, Min, Add/Modify/Delete Column, and more  
#### Support to specifieng a table name different than the class name

## Installation
Install with the Package Manager Console in Visual Studio (PowerShell)

```sh
PM> Install-Package DbEasyConnect

```

Install via Dotnet CLI

```sh
dotnet add package DbEasyConnect

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
con.IDbEc<Sample>().GetAll();          
```
[Check out the 'Start' wiki](https://github.com/jacobSpitzer/DbEasyConnect/wiki/IDbEcStartExtensions)

#### How to write a Table Class

In order to get defined by the Library, you need to write all Table classes with the [Table("TableName")] Attribute. only classes within your current project assembly will work.
Add the relevant references for that. like:

```sh
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
```

Please note: currently it doesn't matter what table name you specify in the Table attribute. the library will use the Class name to create/find the table on the database server.

##### Data Types

This section will be updated soon...

##### Data Annotations Attributes

You can use most of the [Data Annotations](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations) attributes to specify more properties on the Table/Field as explained in the following table:

Attribute | Description | Example | Sql Property | Supported | MS-SQL SERVER | MySql | SqLite | PostgreSql | LocalDb
----------|-------------|---------|--------------|-----------|---------------|-------|--------|------------|---------|
Database Schema related Attributes 
Table | name of the table and define the schema | `Table("TableName", Schema = "dbo")]` | TABLE NAME | :heavy_check_mark: | :heavy_check_mark: | 
Column | name, data type, and order of the column | `[Column("Name", Order = 2, TypeName = "Varchar(100)")]` |
Key | set field(s) as the table primary key | `[Key]` | PRIMARY KEY | :heavy_check_mark: | :heavy_check_mark: |
Timestamp
ConcurrencyCheck
ForeignKey
InverseProperty
Index
DatabaseGenerated
ComplexType
NotMapped
[Required] | NOT NULL | :heavy_check_mark: | :heavy_check_mark: |
[Index(IsUnique=true)] | UNIQUE
[ForeignKey("TableName")] | FOREIGN KEY |
[AutoIncrement] | Identity(1,1) | :heavy_check_mark: | :heavy_check_mark: |
[DefaultValue(true)] | DEFAULT |
[StringLength(50)] | LENGTH |
Validation Attributes
Required
MinLength
MaxLength
StringLength


#### Create / Modify Tables & Database, Compare / Migrate Code Tables VS Server Database

[Check out the 'Table & Database Methods' wiki](https://github.com/jacobSpitzer/DbEasyConnect/wiki/IDbEcTableExtensions)

#### Supported Linq Methods

[Check out the 'Linq Methods' wiki](https://github.com/jacobSpitzer/DbEasyConnect/wiki/IDbEcLinqExtensions)

#### Get, Find, Insert, Update, Delete, InsertList, UpdateList, DeleteList

[Check out the 'Crud Methods' wiki](https://github.com/jacobSpitzer/DbEasyConnect/wiki/IDbEcCrudExtensions)

#### Full Library Reference

[Check out the wiki](https://github.com/jacobSpitzer/DbEasyConnect/wiki/Home)
