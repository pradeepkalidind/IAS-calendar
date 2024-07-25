using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(110)]
    public class _110_NationalHolidaySetting : FluentMigrator.Migration
    {
        private const string NATIONAL_HOLIDAY_SETTING_TABLE_NAME = "NationalHolidaySetting";

        public override void Up()
        {
            Create.Table(NATIONAL_HOLIDAY_SETTING_TABLE_NAME)
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("Country").AsString(4).NotNullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}