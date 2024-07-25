namespace Calendar.Model.Compact.Rules
{
    public class WhenSecondLocationIsBlank : WhenStringPropetyIsBlank
    {
        public override void DoApply(DayActivity dayActivity)
        {
            dayActivity.LocationPattern =
                new LocationPattern(dayActivity.LocationPattern.FirstLocation,
                    dayActivity.LocationPattern.FirstLocationArrivalTime,
                    dayActivity.LocationPattern.FirstLocationDepartureTime);

            if (!string.IsNullOrEmpty(dayActivity.LocationPattern.FirstLocation))
            {
                dayActivity.DayType = DayType.Standard;
            }
        }

        public override bool Applicable(DayActivity dayActivity)
        {
            return Applicable(dayActivity.LocationPattern.SecondLocation);
        }
    }
}