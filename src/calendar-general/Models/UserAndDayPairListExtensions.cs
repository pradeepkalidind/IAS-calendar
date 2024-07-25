using System;
using System.Collections.Generic;

namespace Calendar.General.Models
{
    public static class UserAndDayPairListExtensions
    {
        public static Dictionary<Guid, List<DateTime>> ToUserDaysMapping(this IEnumerable<UserAndDayPair> pairs)
        {
            var result = new Dictionary<Guid, List<DateTime>>();
            foreach (var pair in pairs)
            {
                result.AppendUserAndDayPair(pair);
            }
            return result;
        }

        private static void AppendUserAndDayPair(this IDictionary<Guid, List<DateTime>> result, UserAndDayPair pair)
        {
            if (!result.ContainsKey(pair.UserId))
            {
                result.Add(pair.UserId, new List<DateTime>());
            }
            result[pair.UserId].Add(pair.Day);
        }
    }
}