using System;
using System.Collections.Generic;
using System.Linq;

namespace Calendar.Model.Compact
{
    public static class ActivityTypeConvertor
    {
        private static readonly IDictionary<ActivityType, string> ActivityTypeMaps =
            new Dictionary<ActivityType, string>
            {
                {ActivityType.Work, "W"},
                {ActivityType.NonWork, "NW"},
                {ActivityType.Sick, "S"},
                {ActivityType.Vacation, "V"},
                {ActivityType.National, "N"},
                {ActivityType.COVID19Work, "CW"},
                {ActivityType.COVID19NonWork, "CNW"},
                                                                                         };

        private static readonly IDictionary<ActivityType, Tuple<string, bool>> ActivityTypeLongNameMaps =
            new Dictionary<ActivityType, Tuple<string, bool>>
            {
                {ActivityType.Work, new Tuple<string, bool>("/ActivityType/Work", false)},
                {ActivityType.NonWork, new Tuple<string, bool>("/ActivityType/NonWork", false)},
                {ActivityType.Sick, new Tuple<string, bool>("/ActivityType/Sick", false)},
                {ActivityType.Vacation, new Tuple<string, bool>("/ActivityType/Vacation", false)},
                {ActivityType.National, new Tuple<string, bool>("/ActivityType/National", false)},
                {ActivityType.Empty, new Tuple<string, bool>("/ActivityType/Empty", false)},
                {ActivityType.COVID19Work, new Tuple<string, bool>("/ActivityType/Work", true)},
                {ActivityType.COVID19NonWork, new Tuple<string, bool>("/ActivityType/NonWork", true)},
            };

        public static ActivityType GetActivityType(string activityAbbr)
        {
            var activityMap = ActivityTypeMaps.FirstOrDefault(a => a.Value.Equals(activityAbbr));
            return activityMap.Key;
        }
 
        public static ActivityType GetActivityTypeByLongName(string activityAbbr, bool isCovid)
        {
            var activityMap = ActivityTypeLongNameMaps.FirstOrDefault(a => a.Value.Item1.Equals(activityAbbr, StringComparison.OrdinalIgnoreCase) && a.Value.Item2 == isCovid);
            return activityMap.Key;
        }
        
        public static bool Exist(string activityAbbr)
        {
            return ActivityTypeLongNameMaps.Values.Any(v => v.Item1 == activityAbbr);
        }

        public static string GetActivityAbbr(ActivityType activity)
        {
            return ActivityTypeMaps.ContainsKey(activity) ? ActivityTypeMaps[activity] : "E";
        }

        public static string GetActivityLongNameByShortName(string shortName)
        {
            return GetActivityLongName(GetActivityType(shortName));
        }

        public static string GetActivityLongName(ActivityType activityType)
        {
            return ActivityTypeLongNameMaps.ContainsKey(activityType) ? ActivityTypeLongNameMaps[activityType].Item1 : "/ActivityType/Empty";
        }

        public static bool GetIsCovidByActivityShortName(string shortName)
        {
            var activityType = GetActivityType(shortName);
            return GetIsCovidByActivityType(activityType);
        }

        public static bool GetIsCovidByActivityType(ActivityType activityType)
        {
            return activityType == ActivityType.COVID19Work || activityType == ActivityType.COVID19NonWork;
        }
    }
}