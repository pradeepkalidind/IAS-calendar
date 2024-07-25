using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(180)]
    public class _180_Delete_Days_AndRelated_Views : FluentMigrator.Migration
    {
        private const string DAYS = "Days";
        private const string DaysCreationView = "DaysCreationView";
        private const string DaysDeletionView = "DaysDeletionView";
        private const string DaysHistoryView = "DaysHistoryView";
        private const string DeletedElementsHistoryView = "DeletedElementsHistoryView";
        private const string ExistedNotes = "ExistedNotes";

        public override void Up()
        {
            Execute.Sql(string.Format("DROP VIEW {0}", DaysCreationView));
            Execute.Sql(string.Format("DROP VIEW {0}", DaysDeletionView));
            Execute.Sql(string.Format("DROP VIEW {0}", DaysHistoryView));
            Execute.Sql(string.Format("DROP VIEW {0}", DeletedElementsHistoryView));
            Execute.Sql(string.Format("DROP VIEW {0}", ExistedNotes));
            Delete.Table(DAYS);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}