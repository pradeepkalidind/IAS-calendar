using System.Configuration;

namespace Calendar.General.Configuration
{
    public class UserConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }
        
        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return (string)this["password"]; }
        }
        
        [ConfigurationProperty("role", IsRequired = true)]
        public string Role
        {
            get { return (string)this["role"]; }
        }

        public bool Match(string name, string password)
        {
            return Name.Equals(name) && Password.Equals(password);
        }
    }
}