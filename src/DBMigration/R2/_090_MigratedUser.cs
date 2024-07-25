using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(090)]
    public class _090_MigratedUser : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "MigratedUser";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("UserId").AsGuid().PrimaryKey()
                .WithColumn("LastMigratedEntryTimestamp").AsInt64().NotNullable()
                .WithColumn("Status").AsByte().NotNullable()
                .WithColumn("Timestamp").AsInt64().NotNullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}