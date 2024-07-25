using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Feed.Models;
using Calendar.Models.Feed;

namespace Calendar.Feed
{
    public class ModelTypes
    {
        private static readonly Type[] Models = new[]
                                                    {
                                                        typeof(FeedEntry),
                                                        typeof(FeedArchivedEntry),
                                                    };

        public static List<Type> Get()
        {
            return Models.ToList();
        }
    }
}