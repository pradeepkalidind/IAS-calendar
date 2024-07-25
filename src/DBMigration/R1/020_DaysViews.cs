using FluentMigrator;

namespace Calendar.Migration.R1
{
    [Migration(020)]
    public class DaysViews : FluentMigrator.Migration
    {
        private string tableName = "Days";

        public override void Up()
        {
            ViewHelper.CreatesViews(Execute, tableName, @"q.Type, q.FirstActivity, q.SecondActivity, q.FirstActivityAllocation, q.SecondActivityAllocation,
            q.FirstLocation, q.FirstLocationDepartureTime, q.FirstLocationArrivalTime, q.SecondLocation,
            q.SecondLocationDepartureTime, q.SecondLocationArrivalTime, q.ThirdLocation, q.ThirdLocationArrivalTime, q.Note ");
        }

        public override void Down()
        {
            ViewHelper.DeleteViews(Execute, tableName);
        }
    }
}