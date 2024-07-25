using TableExporter.Common;

namespace Calendar.Model.Compact.Archive.Rule
{
    public class NationalHolidaySettingRule : DeleteTableRuleBase
    {
        public override string TableName
        {
            get { return "NationalHolidaySetting"; }
        }

        public override string PrimaryKey
        {
            get { return "UserId"; }
        }
    }
}