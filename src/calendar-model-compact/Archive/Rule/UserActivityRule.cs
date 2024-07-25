using TableExporter.Common;

namespace Calendar.Model.Compact.Archive.Rule
{
    public class UserActivityRule : DeleteTableRuleBase
    {
        public override string TableName
        {
            get { return "UserActivity"; }
        }

        public override string PrimaryKey
        {
            get { return "UserId"; }
        }
    }
}