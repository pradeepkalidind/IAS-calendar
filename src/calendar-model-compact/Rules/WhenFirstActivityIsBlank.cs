namespace Calendar.Model.Compact.Rules
{
    public class WhenFirstActivityIsBlank : WhenStringPropetyIsBlank
    {
        public override void DoApply(DayActivity dayActivity)
        {
            dayActivity.DayType = DayType.Incomplete;
        }

        public override bool Applicable(DayActivity dayActivity)
        {
            return ActivityType.Empty.Equals(dayActivity.First.Type);
        }
    }
}