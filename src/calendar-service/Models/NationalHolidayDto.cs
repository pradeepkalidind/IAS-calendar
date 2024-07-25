namespace Calendar.Service.Models
{
    public class NationalHolidaySettingDto
    {
        public string Country { get; set; }
        public HolidayTranslation[] Holidays { get; set; }
    }

    public class HolidayTranslation
    {
        public string Date { get; set; }
        public string I18n { get; set; }
    }
}

