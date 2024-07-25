using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(200)]
    public class _200_CreateTableVisitedUser : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "VisitedUser";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable().Unique();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}