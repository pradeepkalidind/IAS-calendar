using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(120)]
    public class _120_UserWithNewUI : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "UserWithNewUI";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("UserId").AsGuid().PrimaryKey()
                .WithColumn("Timestamp").AsInt64().NotNullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}