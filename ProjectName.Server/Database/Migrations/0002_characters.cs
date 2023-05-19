using FluentMigrator;
using System.Data;

namespace ProjectName.Server.Database.Migrations
{
    /// <summary>
    /// Creates the initial tables.
    /// Migration classes must be public and inherit from FluentMigrator.IMigration or FluentMigrator.Migration.
    /// Migration class cannot be attributed with [MigrationAttribute].
    /// </summary>
    [Migration(2)]
    public class MigrationCreateCharacterTables : Migration
    {
        public override void Up()
        {
            Create.Table("user_characters")
                .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt64().NotNullable().Indexed()
                .WithColumn("name").AsString(255).NotNullable()
                .WithColumn("created").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            // create foreign key for user_id column in user_characters table referencing id column in users table
            Create.ForeignKey("fk_user_characters_user_id")
                .FromTable("user_characters").ForeignColumn("user_id")
                .ToTable("users").PrimaryColumn("id")
                .OnDeleteOrUpdate(Rule.Cascade);

            // Outfits are stored as JSON in the database, so we need to use a custom type
            // https://mariadb.com/kb/en/json-data-type/
            // https://dzone.com/articles/using-json-in-mariadb
            // We store outfits in the database so that returning players can have their outfits restored
            // when they log in. This is useful for when a player has to reinstall FiveM or when they
            // play on a different computer.
            // The outfits are also stored locally in the client's cache, so that they can be used
            // without having to wait for the database to respond.
            Create.Table("user_character_outfits")
                .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("character_id").AsInt64().NotNullable().Indexed()
                .WithColumn("outfit").AsCustom("JSON").NotNullable()
                .WithColumn("active").AsBoolean().NotNullable().WithDefaultValue(false);

            // create foreign key for character_id column in user_character_outfits table referencing id column in user_characters table
            Create.ForeignKey("fk_user_character_outfits_character_id")
                .FromTable("user_character_outfits").ForeignColumn("character_id")
                .ToTable("user_characters").PrimaryColumn("id")
                .OnDeleteOrUpdate(Rule.Cascade);

            // TODO: Use Assembly.GetExecutingAssembly().Location to get the correct path
            string resourceName = Natives.GetCurrentResourceName();
            Execute.Script($"resources/[project]/{resourceName}/server/migration_scripts/0002_characters.sql");
        }

        public override void Down()
        {
            Delete.Table("user_character_outfits");
            Delete.Table("user_characters");
        }
    }
}
