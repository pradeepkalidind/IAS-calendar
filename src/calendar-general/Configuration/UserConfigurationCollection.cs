using System.Configuration;

namespace Calendar.General.Configuration
{
    public class UserConfigurationCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new UserConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UserConfiguration)element).Name;
        }
    }
}