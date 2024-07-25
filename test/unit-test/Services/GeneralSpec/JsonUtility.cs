namespace Calendar.Tests.Unit.Services.GeneralSpec
{
    static class JsonUtility
    {
        const string DeleteStringPattern = "[<'userId':'{0}', 'date':'{1}'>]";
        const string ActivityStringPattern = "[<'userId':'{0}', 'date':'{1}', 'activity':'{2}'>]";
        const string ActivityAllocationStringPattern = "[<'userId':'{0}', 'date':'{1}', 'firstAllocation':'{2}'>]";
        const string FirstLocationStringPattern = "[<'userId':'{0}', 'date':'{1}', 'location':'{2}', 'departureTime':'{3}','arrivalTime':'{4}'>]";
        const string SecondLocationStringPattern = "[<'userId':'{0}', 'date':'{1}', 'location':'{2}', 'departureTime':'{3}','arrivalTime':'{4}'>]";
        const string NoteStringPattern = "[<'userId':'{0}', 'date':'{1}', 'content':'{2}'>]";
        const string DefaultWorkDaysStringPattern = "[<'userId':'{0}', 'days':'{1}'>]";

        const string ThirdLocationStringPattern =
            "[<'userId':'{0}', 'date':'{1}', 'location':'{2}', 'arrivalTime':'{3}'>]";

        private const string BatchStringPattern = "<'firstActivities':{0},'secondActivities':{1},'firstActivityAllocations':{2},'secondActivityAllocations':{3},'firstLocations':{4},'firstTravels':{5},'secondTravels':{6},'notes':{7}>";


        public static string ToDeleteJson(string userId, string date)
        {
            return FormatToJson(string.Format(DeleteStringPattern, userId, date));
        }

        private static string FormatToJson(string data)
        {
            return data.Replace('\'', '\"').Replace('<', '{').Replace('>', '}');
        }
    }
}
