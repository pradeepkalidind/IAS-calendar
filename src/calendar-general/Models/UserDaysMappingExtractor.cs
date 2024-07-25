using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Calendar.General.Models
{
    public static class UserDaysMappingExtractor
    {

        public static Dictionary<Guid, List<DateTime>> Extract(string input)
        {
            var pairs = JsonConvert.DeserializeObject<List<UserAndDayPair>>(input);
            return pairs.ToUserDaysMapping();
        }
    }
}