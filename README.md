# FiveM C# Project Example

This is an example C# Project using [Dapper](https://github.com/DapperLib/Dapper) and [FxEvents](https://github.com/manups4e/FxEvents)

## Project requirements

- Visual Studio 2022
- .NET Framework 4.7.2
- Basic understanding of C#

## Database

This was developed with [MariaDB](https://mariadb.org/) or [MySQL](https://www.mysql.com/) in mind, you can change this to Postgres or any other database you prefer. When creating the database you can use the name given in this project which is `fivemdb` or your own, just make sure to update the `server-config.json`. Recommended that the database is created using the `utf8mb4_unicode_520_ci` coallation to support emojis.

Database user must have all permissions except for `GRANT` as it will run migrations against the database to create tables, procedures, and various other requirements of the database. In the future it would be best to have two users, one for the migration system to use and another for the application when using the database to improve security, this isn't hard to add but as this is just an example you should be fine doing this yourself.

MariaDB Create;

```sql
CREATE DATABASE `fivemdb` /*!40100 COLLATE 'utf8mb4_unicode_520_ci' */
```

Migrations are made using [fluentmigrator](https://github.com/fluentmigrator/fluentmigrator), test all migrations and scripts on your own local development server long before pushing anything to production.

Table migrations can be found in the server project `ProjectName.Server\Database\Migrations`, these are checked to see if they are required to be ran each time the resource is started. In the `resource_files\server\migration_scripts` are the scripts for creating stored procedures or other SQL scripts that the migration cannot generate in code. Read the [fluentmigrator documents](https://fluentmigrator.github.io/) for more information.

## Building

Building the projects will output into a build folder in the root directory, inside the server folder you'll find a `server-config.json-example` rename this file to `server-config.json` and update the contents to connect to your chosen database (MySQL, MariaDB, or any that [Dapper](https://github.com/DapperLib/Dapper) supports). The contents of the build folder can be copied to your FiveM server to be ran.

## Limitations

Currently [FxEvents](https://github.com/manups4e/FxEvents) only allows a single resource to be using it, so it is best to develop with this in mind, it was originally developed for C# Frameworks that run in a single resource.

## Known Issues

- Console Error: `Could not load assembly MySql.Data - loading exceptions: Exception loading assembly MySql.Data: System.IO.FileNotFoundException: Unable to find the specified file.`

  This can be ignored, migrations still run and MySql.Data has currently not shown to be required. Fluent Migrator have had conversations around an [issue](https://github.com/fluentmigrator/fluentmigrator/pull/1600) that is likely related to it, but have yet to release a new version with the changes.

- 'DELIMITER' should not be used with MySqlConnector

  Follow the [URL](https://fl.vu/mysql-delimiter), it explains it, just remove them.
