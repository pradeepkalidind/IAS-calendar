using TableExporter.Common;

namespace Calendar.Model.Compact.Archive.Rule
{
    public class NoteRule : DeleteTableRuleBase
    {
        public override string TableName
        {
            get { return "Note"; }
        }

        public override string PrimaryKey
        {
            get { return "UserId"; }
        }
    }
}