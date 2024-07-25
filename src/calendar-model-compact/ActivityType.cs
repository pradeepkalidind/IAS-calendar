namespace Calendar.Model.Compact
{
    public enum ActivityType : byte
    {
        Empty,    //0
        Work,     //1
        Sick,     //2
        Vacation, //3
        National, //4
        NonWork,   //5
        COVID19Work, //6
        COVID19NonWork, //7
    }

    public enum DayType :byte 
    {
        ExtendedTrip, 
        Standard,
        DayTrip,
        Incomplete 
    }
}