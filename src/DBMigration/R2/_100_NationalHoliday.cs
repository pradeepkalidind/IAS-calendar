using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(100)]
    public class _100_NationalHoliday : FluentMigrator.Migration
    {
        private const string NATIONAL_HOLIDAY_TABLE_NAME = "NationalHoliday";
        private const string NATIONAL_HOLIDAY_I18N_TABLE_NAME = "NationalHolidayI18N";

        public override void Up()
        {
            Create.Table(NATIONAL_HOLIDAY_TABLE_NAME)
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Country").AsString(4).NotNullable()
                .WithColumn("Date").AsDate().NotNullable();

            Create.Table(NATIONAL_HOLIDAY_I18N_TABLE_NAME)
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("NationalHolidayId").AsGuid().NotNullable()
                .WithColumn("Language").AsString(8).NotNullable()
                .WithColumn("Translation").AsString(512).Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}