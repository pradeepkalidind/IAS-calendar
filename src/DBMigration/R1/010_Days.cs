using System;
using FluentMigrator;

namespace Calendar.Migration.R1
{
    [Migration(010)]
    public class Days : FluentMigrator.Migration
    {
        private readonly string tableName = "Days";

        public override void Up()
        {
            Create.Table(tableName)
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("Date").AsDate().NotNullable()
                .WithColumn("Type").AsString().NotNullable()
                .WithColumn("EnterFrom").AsString().Nullable()
                .WithColumn("FirstActivity").AsString().Nullable()
                .WithColumn("SecondActivity").AsString().Nullable()
                .WithColumn("FirstActivityAllocation").AsDouble().Nullable()
                .WithColumn("SecondActivityAllocation").AsString().Nullable()
                .WithColumn("FirstLocation").AsString().Nullable()
                .WithColumn("FirstLocationDepartureTime").AsByte().Nullable()
                .WithColumn("FirstLocationArrivalTime").AsByte().Nullable()
                .WithColumn("SecondLocation").AsString().Nullable()
                .WithColumn("SecondLocationDepartureTime").AsByte().Nullable()
                .WithColumn("SecondLocationArrivalTime").AsByte().Nullable()
                .WithColumn("ThirdLocation").AsString().Nullable()
                .WithColumn("ThirdLocationArrivalTime").AsByte().Nullable()
                .WithColumn("Note").AsString(4000).Nullable()
                .WithColumn("Timestamp").AsInt64().NotNullable();

            TableHelper.CreatePrimaryKey(Execute, tableName);

            TableHelper.Create(Create, tableName);

            CreatePartition();
        }

        private void CreatePartition()
        {
            Execute.Sql(@"
BEGIN TRANSACTION
CREATE PARTITION FUNCTION [BY_CALENDAR_DATE](date) AS RANGE LEFT FOR VALUES (N'2000-01-01', N'2001-01-01', N'2002-01-01', N'2003-01-01', N'2004-01-01', N'2005-01-01', N'2006-01-01', N'2007-01-01', N'2008-01-01', N'2009-01-01', N'2010-01-01', N'2011-01-01', N'2012-01-01', N'2013-01-01', N'2014-01-01', N'2015-01-01', N'2016-01-01', N'2017-01-01', N'2018-01-01', N'2019-01-01', N'2020-01-01')


CREATE PARTITION SCHEME [CALENDAR_DATE] AS PARTITION [BY_CALENDAR_DATE] TO ([PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY])


CREATE CLUSTERED INDEX [UserId_Date_Timestamp] ON [dbo].[Days] 
(
    [UserId] ASC,
	[Date] ASC,
	[Timestamp] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = ON, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [CALENDAR_DATE]([Date])

COMMIT TRANSACTION
");
        }

        public override void Down()
        {
            Delete.Table(tableName);
        }
    }

}
