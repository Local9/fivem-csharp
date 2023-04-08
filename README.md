# FiveM C# Project Example

This is an example C# Project using [Dapper](https://github.com/DapperLib/Dapper) and [FxEvents](https://github.com/manups4e/FxEvents)

## Project requirements

- Visual Studio 2022
- .NET Framework 4.7.2
- Basic understanding of C#

## Database

This was developed with [MariaDB](https://mariadb.org/) or [MySQL](https://www.mysql.com/) in mind, you can change this to Postgres or any other database you prefer. When creating the database you can use the name given in this project which is `fivemdb` or your own, just make sure to update the `server-config.json`. The database must use the `utf8mb4_unicode_520_ci` coallation.

Database user must have all permissions except for `GRANT` as it will run migrations against the database to create tables, procedures, and various other requirements of the database.

MariaDB Create;

```sql
CREATE DATABASE `fivemdb` /*!40100 COLLATE 'utf8mb4_unicode_520_ci' */
```

## Building

Building the projects will output into a build folder in the root directory, inside the server folder you'll find a `server-config.json-example` rename this file to `server-config.json` and update the contents to connect to your chosen database (MySQL, MariaDB, or any that [Dapper](https://github.com/DapperLib/Dapper) supports). The contents of the build folder can be copied to your FiveM server to be ran.

## Limitations

Currently [FxEvents](https://github.com/manups4e/FxEvents) only allows a single resource to be using it, so it is best to develop with this in mind, it was originally developed for C# Frameworks that run in a single resource.
