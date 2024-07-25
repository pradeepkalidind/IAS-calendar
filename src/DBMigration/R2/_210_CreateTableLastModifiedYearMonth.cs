using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(210)]
    public class _210_CreateTableLastModifiedYearMonth : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "LastModifiedYearMonth";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable().Unique()
                .WithColumn("LastModifiedMonth").AsInt16().NotNullable()
                .WithColumn("LastModifiedYear").AsInt16().NotNullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}