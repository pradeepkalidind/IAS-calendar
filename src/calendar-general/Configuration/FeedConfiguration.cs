using System.Configuration;

namespace Calendar.General.Configuration
{
    public class FeedConfiguration : ConfigurationSection
    {

        private static FeedConfiguration instance;

        public static FeedConfiguration Instance
        {
            get
            {
                return instance ??
                    (instance = (FeedConfiguration)ConfigurationManager.GetSection("feed"));
            }
        }

        [ConfigurationProperty("duration", IsRequired = true)]
        public int Duration
        {
            get { return (int)this["duration"]; }
        }
    }
}