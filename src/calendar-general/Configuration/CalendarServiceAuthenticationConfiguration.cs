using System.Configuration;

namespace Calendar.General.Configuration
{
    public class CalendarServiceAuthenticationConfiguration : ConfigurationSection
    {

        private static CalendarServiceAuthenticationConfiguration instance;

        public static CalendarServiceAuthenticationConfiguration Instance
        {
            get
            {
                return instance ??
                    (instance = (CalendarServiceAuthenticationConfiguration)ConfigurationManager.GetSection("authentication"));
            }
        }

        [ConfigurationProperty("users", IsRequired = true)]
        public UserConfigurationCollection Users
        {
            get { return (UserConfigurationCollection)this["users"]; }
        }
    }
}