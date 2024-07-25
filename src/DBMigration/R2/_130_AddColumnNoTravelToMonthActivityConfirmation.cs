using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(130)]
    public class _130_AddColumnNoTravelToMonthActivityConfirmation : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "MonthActivity";
        private const string COLUMN_NAME = "NoTravelConfirmation";

        public override void Up()
        {
            Create.Column(COLUMN_NAME)
                  .OnTable(TABLE_NAME)
                  .AsBoolean()
                  .NotNullable()
                  .WithDefaultValue(false);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}