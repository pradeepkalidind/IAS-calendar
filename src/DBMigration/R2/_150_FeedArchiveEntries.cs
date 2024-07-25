using System;
using System.Collections.Generic;
using Calendar.Migration.R1;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(150)]
    public class FeedArchiveEntries : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "FeedArchivedEntries";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("ForDay").AsDate().NotNullable()
                .WithColumn("Timestamp").AsInt64().NotNullable();

            TableHelper.CreatePrimaryKey(Execute, TABLE_NAME);
            TableHelper.CreateClusterIndex(Create, TABLE_NAME, "Timestamp", new Dictionary<string, string> { { "Timestamp", "Descending" } });

        }

        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }
    }
}