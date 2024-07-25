namespace Calendar.Model.Compact.Rules
{
    public class WhenFirstLocationIsBlank : WhenStringPropetyIsBlank
    {
        public override void DoApply(DayActivity dayActivity)
        {
            dayActivity.LocationPattern = LocationPattern.Empty();
            dayActivity.DayType = DayType.Incomplete;
        }

        public override bool Applicable(DayActivity dayActivity)
        {
            return Applicable(dayActivity.LocationPattern.FirstLocation);
        }
    }
}