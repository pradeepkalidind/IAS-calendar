namespace Calendar.Service.Models;

public class CalendarificHoliday
{
    public string name;
    public string description;
    public CalendarificDate date;
    public string[] type;
}

public class CalendarificDate
{
    public string iso;
    public CalendarificDateTime datetime;
}

public class CalendarificDateTime
{
    public int year;
    public int month;
    public int day;
}

public record CalendarificCountry([property:Newtonsoft.Json.JsonProperty("iso-3166")] string IsoName);
