using System;
using Calendar.Migration.R1;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(160)]
    public class _160_UserActivityArchived : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "UserActivityArchived";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("Date").AsDate().NotNullable()
                .WithColumn("Timestamp").AsInt64().NotNullable();

            TableHelper.CreatePrimaryKey(Execute,TABLE_NAME);

            Create.Index("idx_UserActivityArchived_Timestamp")
                .OnTable(TABLE_NAME)
                .OnColumn("Timestamp").Descending()
                .WithOptions().Clustered();

            TableHelper.DropIndex(Execute,"UserActivity", "idx_UserActivity_UserId");
            TableHelper.DropIndex(Execute, "UserActivity", "idx_UserActivity_Timestamp");

            Create.Index("idx_UserActivity_Timestamp")
                .OnTable("UserActivity")
                .OnColumn("Timestamp").Descending()
                .WithOptions().Clustered();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}