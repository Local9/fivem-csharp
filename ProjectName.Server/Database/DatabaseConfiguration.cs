using MySqlConnector;

namespace ProjectName.Server.Database
{
    internal class DatabaseConfiguration
    {
        private static string _connectionString;

        public static string ConnectionString()
        {
            if (!string.IsNullOrEmpty(_connectionString))
                return _connectionString;

            DatabaseConfig databaseConfig = ServerConfiguration.GetDatabaseConfig;

            MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder();

            // Commented out because of some bullcrap with MySQL.Data getting involved due to FluentMigrator.Runner
            // pulling all the bull crap, since 2018 they've talked about fixing it, fixed it May 5th 2022, then not released it
            // https://github.com/fluentmigrator/fluentmigrator/issues/903

            // mySqlConnectionStringBuilder.ApplicationName = databaseConfig.ApplicationName;

            mySqlConnectionStringBuilder.Database = databaseConfig.Database;
            mySqlConnectionStringBuilder.Server = databaseConfig.Server;
            mySqlConnectionStringBuilder.Port = databaseConfig.Port;
            mySqlConnectionStringBuilder.UserID = databaseConfig.Username;
            mySqlConnectionStringBuilder.Password = databaseConfig.Password;

            mySqlConnectionStringBuilder.MaximumPoolSize = databaseConfig.MaximumPoolSize;
            mySqlConnectionStringBuilder.MinimumPoolSize = databaseConfig.MinimumPoolSize;
            mySqlConnectionStringBuilder.ConnectionTimeout = databaseConfig.ConnectionTimeout;

            return _connectionString = mySqlConnectionStringBuilder.ToString();
        }
    }
}
