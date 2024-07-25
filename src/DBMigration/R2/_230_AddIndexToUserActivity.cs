using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(230)]
    public class _230_AddIndexToUserActivity : FluentMigrator.Migration
    {
        private const string TableName = "UserActivity";
        private const string IndexName = "Idx_UserId";
        private const string ColumnName = "UserId";

        public override void Up()
        {
            if (!Schema.Table(TableName).Index(IndexName).Exists())
            {
                Create.Index(IndexName).OnTable(TableName).OnColumn(ColumnName);
            }
        }

        public override void Down()
        {
            if (Schema.Table(TableName).Index(IndexName).Exists())
            {
                Delete.Index(IndexName).OnTable(TableName);
            }
        }
    }
}