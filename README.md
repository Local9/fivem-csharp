# FiveM C# Project Example

This is an example C# Project using Dapper and FxEvents

## Project requirements

- Visual Studio 2022
- .NET Framework 4.7.2
- Basic understanding of C#

## Building

Building the projects will output into a build folder in the root directory, inside the server folder you'll find a `server-config.json-example` rename this file to `server-config.json` and update the contents to connect to your chosen database (MySQL, MariaDB, or any that [Dapper](https://github.com/DapperLib/Dapper) supports). The contents of the build folder can be copied to your FiveM server to be ran.

## Limitations

Currently FxEvents only allows a single resource to be using it, so it is best to build this with this in mind, it was originally developed for C# Frameworks that run in a single resource.
