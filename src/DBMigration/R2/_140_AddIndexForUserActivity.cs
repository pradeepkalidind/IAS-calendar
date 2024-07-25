using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(140)]
    public class _140_AddIndexForUserActivity : FluentMigrator.Migration
    {
        public override void Up()
        {
            Create.Index("idx_UserActivity_Timestamp")
                .OnTable("UserActivity")
                .OnColumn("Timestamp")
                .Descending();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}