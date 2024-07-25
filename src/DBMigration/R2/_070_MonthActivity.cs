using System;
using Calendar.Migration.R1;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(070)]
    public class _070_MonthActivity : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "MonthActivity";
        private const string LOCATIONPATTEN_TABLE_NAME = "LocationPattern";
        private const string NOTE_TABLE_NAME = "Note";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("Year").AsInt16().NotNullable()
                .WithColumn("Month").AsInt16().NotNullable()
                .WithColumn("ActivityContent").AsBinary(64).NotNullable()
                .WithColumn("FirstLocationActivityAllocationContent").AsBinary(64).NotNullable()
                .WithColumn("LocationPatternContent").AsBinary(256).NotNullable()
                .WithColumn("DayType").AsBinary(32).NotNullable()
                .WithColumn("DataSource").AsBinary(32).NotNullable()
                .WithColumn("TimeStamp").AsCustom("TimeStamp").NotNullable();

            TableHelper.CreatePrimaryKey(Execute,TABLE_NAME);

            Create.Index("idx_MonthActivity_User_Month")
                .OnTable(TABLE_NAME)
                .OnColumn("UserId").Ascending()
                .OnColumn("Year").Descending()
                .OnColumn("Month").Descending()
                .WithOptions().Clustered();

            Create.Table(LOCATIONPATTEN_TABLE_NAME)
                .WithColumn("Hash").AsInt32().NotNullable()
                .WithColumn("FirstLocation").AsString(255).Nullable()
                .WithColumn("FirstLocationDepartureTime").AsInt16().Nullable()
                .WithColumn("FirstLocationArrivalTime").AsInt16().Nullable()
                .WithColumn("SecondLocation").AsString(255).Nullable()
                .WithColumn("SecondLocationDepartureTime").AsInt16().Nullable()
                .WithColumn("SecondLocationArrivalTime").AsInt16().Nullable()
                .WithColumn("ThirdLocation").AsString(255).Nullable()
                .WithColumn("ThirdLocationArrivalTime").AsInt16().Nullable();

            TableHelper.CreatePrimaryKeyAutoIncrement(Execute, LOCATIONPATTEN_TABLE_NAME,"PK_LocationPattern");

            Create.Index("idx_LocationPattern_Hash")
                .OnTable(LOCATIONPATTEN_TABLE_NAME)
                .OnColumn("Hash").Ascending()
                .WithOptions().Clustered();

            Create.Table(NOTE_TABLE_NAME)
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("Date").AsDate().NotNullable()
                .WithColumn("Content").AsString(4000).NotNullable()
                .WithColumn("TimeStamp").AsCustom("TimeStamp").NotNullable(); 

            TableHelper.CreatePrimaryKey(Execute, NOTE_TABLE_NAME);

            Create.Index("idx_Note_User_Date")
               .OnTable(NOTE_TABLE_NAME)
               .OnColumn("UserId").Ascending()
               .OnColumn("Date").Descending()
               .WithOptions().Clustered();

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}