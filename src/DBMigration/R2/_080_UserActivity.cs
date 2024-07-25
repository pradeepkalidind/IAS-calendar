using System;
using Calendar.Migration.R1;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(080)]
    public class _080_UserActivity : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "UserActivity";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("Date").AsDate().NotNullable()
                .WithColumn("Timestamp").AsInt64().NotNullable();

            TableHelper.CreatePrimaryKey(Execute,TABLE_NAME);

            Create.Index("idx_UserActivity_UserId")
                .OnTable(TABLE_NAME)
                .OnColumn("UserId").Ascending()
                .OnColumn("Timestamp").Descending()
                .WithOptions().Clustered();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}