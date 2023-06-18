using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ProjectName.Server
{
    internal sealed class ServerConfiguration
    {
        private const string SERVER_CONFIG_LOCATION = $"/server/server-config.json";
        private static ServerConfig _serverConfig = null;

        private ServerConfiguration()
        {

        }

        internal static ServerConfig GetServerConfig
        {
            get
            {
                if (_serverConfig is not null)
                    return _serverConfig;

                try
                {
                    string serverConfigFile = Natives.LoadResourceFile((CString)Natives.GetCurrentResourceName(), SERVER_CONFIG_LOCATION);
                    _serverConfig = JsonConvert.DeserializeObject<ServerConfig>(serverConfigFile);
                    return _serverConfig;
                }
                catch (Exception ex)
                {
                    Main.Logger.Error($"Server Configuration was unable to be loaded.");
                    Main.Logger.Info($"{ex}");
                    Main.Logger.Error($"---------------------------------------------.");
                    return default!;
                }
            }
        }

        internal static DatabaseConfig GetDatabaseConfig => GetServerConfig.Database;
    }

    [DataContract]
    public class ServerConfig
    {
        [DataMember(Name = "database")]
        public DatabaseConfig Database;
    }

    [DataContract]
    public class DatabaseConfig
    {
        private uint _connectionTimeout = 5;
        private uint _minimumPoolSize = 10;
        private uint _maximumPoolSize = 50;

        /// <summary>
        /// Name of the application.
        /// </summary>
        [DataMember(Name = "application")]
        public string ApplicationName = "FiveM Project Dapper Connection";

        /// <summary>
        /// Servers hostname or IP.
        /// </summary>
        [DataMember(Name = "server")]
        public string Server = "localhost";

        /// <summary>
        /// Name of the Database.
        /// </summary>
        [DataMember(Name = "databaseName")]
        public string Database = "fivem";

        /// <summary>
        /// Database port.
        /// </summary>
        [DataMember(Name = "port")]
        public uint Port = 3306;

        /// <summary>
        /// Database Username.
        /// </summary>
        [DataMember(Name = "username")]
        public string Username;

        /// <summary>
        /// Database Password.
        /// </summary>
        [DataMember(Name = "password")]
        public string Password;

        /// <summary>
        /// Minimum number of connections in the pool.
        /// </summary>
        [DataMember(Name = "minimumPoolSize")]
        public uint MinimumPoolSize
        {
            get => _minimumPoolSize;
            set => _minimumPoolSize = value > 0 ? value : 10;
        }

        /// <summary>
        /// Maximum number of connections in the pool.
        /// </summary>
        [DataMember(Name = "maximumPoolSize")]
        public uint MaximumPoolSize
        {
            get => _maximumPoolSize;
            set => _maximumPoolSize = value > 0 ? value : 50;
        }

        /// <summary>
        /// Connection timeout in seconds. If your query is taking longer than the timeout setting, review your SQL query before changing the timeout setting.
        /// </summary>
        [DataMember(Name = "connectionTimeout")]
        public uint ConnectionTimeout
        {
            get => _connectionTimeout;
            set => _connectionTimeout = value > 0 ? value : 5;
        }
    }
}
