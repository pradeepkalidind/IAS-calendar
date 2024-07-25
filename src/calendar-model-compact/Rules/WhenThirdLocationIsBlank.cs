namespace Calendar.Model.Compact.Rules
{
    public class WhenThirdLocationIsBlank : WhenStringPropetyIsBlank
    {
        public override void DoApply(DayActivity dayActivity)
        {
            dayActivity.LocationPattern =
                new LocationPattern(dayActivity.LocationPattern.FirstLocation,
                    dayActivity.LocationPattern.FirstLocationArrivalTime,
                    dayActivity.LocationPattern.FirstLocationDepartureTime,
                    dayActivity.LocationPattern.SecondLocation,
                    dayActivity.LocationPattern.SecondLocationArrivalTime,
                    dayActivity.LocationPattern.SecondLocationDepartureTime);
        }

        public override bool Applicable(DayActivity dayActivity)
        {
            return Applicable(dayActivity.LocationPattern.ThirdLocation);
        }
    }
}