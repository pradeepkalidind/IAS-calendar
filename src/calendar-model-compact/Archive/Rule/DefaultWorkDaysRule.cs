using TableExporter.Common;

namespace Calendar.Model.Compact.Archive.Rule
{
    public class DefaultWorkDaysRule : DeleteTableRuleBase
    {
        public override string TableName
        {
            get { return "DefaultWorkDays"; }
        }

        public override string PrimaryKey
        {
            get { return "UserId"; }
        }
    }
}