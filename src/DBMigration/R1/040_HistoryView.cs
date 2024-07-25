using FluentMigrator;

namespace Calendar.Migration.R1
{
    [Migration(040)]
    public class HistoryView : FluentMigrator.Migration
    {
        public override void Up()
        {
            ViewHelper.CreateHistoryView(Execute, "Days");
            ViewHelper.CreateHistoryView(Execute, "DeletedElements");
        }

        public override void Down()
        {
            ViewHelper.DeleteHistoryView(Execute, "Days");
            ViewHelper.DeleteHistoryView(Execute, "DeletedElements");
        }
    }
}