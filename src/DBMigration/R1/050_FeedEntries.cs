using System;
using System.Collections.Generic;
using FluentMigrator;

namespace Calendar.Migration.R1
{
    [Migration(050)]
    public class FeedEntries : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "FeedEntries";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("ForDay").AsDate().NotNullable()
                .WithColumn("Timestamp").AsInt64().NotNullable();

            TableHelper.CreatePrimaryKey(Execute, TABLE_NAME);
            TableHelper.CreateClusterIndex(Create, TABLE_NAME, "Timestamp", new Dictionary<string, string> { { "Timestamp", "Descending" } });

            CreatePartition();

        }

        private void CreatePartition()
        {
            var partionFunctionStr = "CREATE PARTITION FUNCTION [BY_TIMESTAMP](bigint) AS RANGE LEFT FOR VALUES (";
            var partionSchemaStr = "CREATE PARTITION SCHEME [TIMESTAMP] AS PARTITION [BY_TIMESTAMP] TO (";
            for (var date = new DateTime(2011, 08, 01); date <= new DateTime(2021, 1, 1); date = date.AddMonths(1))
            {
                partionFunctionStr = partionFunctionStr + date.Ticks;
                partionFunctionStr = date == new DateTime(2021, 1, 1) ? partionFunctionStr + ")" : partionFunctionStr + ", ";
                partionSchemaStr = partionSchemaStr + "[PRIMARY], ";
            }
            partionSchemaStr = partionSchemaStr + "[PRIMARY])";
            var partionSql = string.Format(@"
BEGIN TRANSACTION
{0}
{1}
CREATE CLUSTERED INDEX [Timestamp] ON [dbo].[FeedEntries] 
(
	[Timestamp] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = ON, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TIMESTAMP]([Timestamp])
COMMIT TRANSACTION
", partionFunctionStr, partionSchemaStr);
            Execute.Sql(partionSql);
        }

        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }
    }
}