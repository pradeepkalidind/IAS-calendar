using TableExporter.Common;

namespace Calendar.Model.Compact.Archive.Rule
{
    public class MonthActivityRule : DeleteTableRuleBase
    {
        public override string TableName
        {
            get { return "MonthActivity"; }
        }

        public override string PrimaryKey
        {
            get { return "UserId"; }
        }
    }
}