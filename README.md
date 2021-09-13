# Dapper.TQuery Library
## Description
Dapper.TQuery is a c# based library package that provides database connection services with the focus on the following three goals:
* to get the high fastest performance
* to avoid configuration the most possible
* to support strong typed coding and avoid bugs and spelling mistakes.

## The short history:
I was using Entity Framework for a long time and founded difficult to configure the database schema, binding, migrations, and repositories in addition to writing Data Models.

I also found it difficult to modify any table / filed in the database after the first creation and configuration, that requires to update the database directly, and use migration.
[Look here a great article by Tim Corey about why to not use Entity Framework](https://www.iamtimcorey.com/blog/137806/entity-framework). including the performance speed.

And then, I found out about the Dapper library, a great open-source library created by the Stack Overflow team.

But, I found it even hard, because of missing a lot of features provided by EF. like CRUD features on OOP, working with batch insert/update, and the need of writing SQL language, and remember all table/field names, without any warning on spelling mistakes, and missing a LINQ QUERYABLE option, so I need to download the whole table before querying with linq, uses extra memory and lowers the performance. 

So I have started to create a new library that is based on ADO.NET, and Dapper. But gives you a strong typed coding experience, with all the features listed below.

## Features
* Using Dapper as the object mapper, with high fast performance
* using strong typed coding to querying the database simular to entity framework, to avoid spelling mistakes
* without any extra code for creating / migration. You just need to create classes with [Table] attribute, and some Data Annotations Attributes.
* use Dapper with LINQ QUERYABLE features. With the most (but not all) common features of linq extensions.
* use the fastest easyest way for CRUD (Create, Read, Update, and Delete) operations, Find by ID, batch insert/update/delete even with Entity List.

## Installation

## Basic Usage

#### Write Table Class

#### Data Annotations Attributes

#### Create / Modify Tables

#### Code Tables VS Server Database

#### Supported Linq Methods

#### Get, Find, Insert, Update, Delete

#### InsertList, UpdateList, DeleteList

#### Join Multiple Tables

#### Read / Update generated SQL commands
