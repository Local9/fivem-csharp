using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectName.Server.Database
{
    internal class DatabaseMigration
    {
        public static async Coroutine RunMigrations()
        {
            using (ServiceProvider serviceProvider = CreateServices())
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        /// <summary>
        /// Configure the dependency injection services.
        /// </summary>
        /// <returns></returns>
        private static ServiceProvider CreateServices()
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddMySql5()
                    .WithGlobalConnectionString(DatabaseConfiguration.ConnectionString())
                    .ScanIn(typeof(Main).Assembly).For.Migrations()
                )
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Updates the database.
        /// </summary>
        /// <param name="serviceProvider"></param>
        internal static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}
