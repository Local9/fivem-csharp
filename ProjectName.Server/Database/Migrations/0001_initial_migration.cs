using FluentMigrator;
using System.Data;

namespace ProjectName.Server.Database.Migrations
{
    /// <summary>
    /// Creates the initial tables.
    /// Migration classes must be public and inherit from FluentMigrator.IMigration or FluentMigrator.Migration.
    /// Migration class cannot be attributed with [MigrationAttribute].
    /// </summary>
    [Migration(1)]
    public class InitialMigration : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("last_name").AsString(255).NotNullable()
                .WithColumn("created").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("last_seen").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Table("user_identities")
                .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt64().NotNullable().Indexed()
                .WithColumn("type").AsString(50).NotNullable().Indexed()
                .WithColumn("identity").AsString(255).NotNullable().Indexed()
                .WithColumn("created").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Table("user_tokens")
                .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt64().NotNullable().Indexed()
                .WithColumn("token").AsString(255).NotNullable().Indexed()
                .WithColumn("created").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("fk_user_identities_user_id")
                .FromTable("user_identities").ForeignColumn("user_id")
                .ToTable("users").PrimaryColumn("id")
                .OnDeleteOrUpdate(Rule.Cascade);

            Create.ForeignKey("fk_user_tokens_user_id")
                .FromTable("user_tokens").ForeignColumn("user_id")
                .ToTable("users").PrimaryColumn("id")
                .OnDeleteOrUpdate(Rule.Cascade);

            // TODO: Use Assembly.GetExecutingAssembly().Location to get the correct path
            Execute.Script($"resources/[project]/{GetCurrentResourceName()}/server/migration_scripts/0001_initial_migration.sql");
        }

        public override void Down()
        {
            Delete.Table("user_tokens");
            Delete.Table("user_identities");
            Delete.Table("users");
        }
    }
}
