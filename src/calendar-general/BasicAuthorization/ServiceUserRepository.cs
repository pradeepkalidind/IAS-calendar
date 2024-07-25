using System.Linq;
using Calendar.Authentication;
using Calendar.General.Configuration;

namespace Calendar.General.BasicAuthorization
{
    public class ServiceUserRepository : IUserRepository
    {
        private readonly UserConfigurationCollection users;

        public ServiceUserRepository() : this(CalendarServiceAuthenticationConfiguration.Instance.Users) {}

        private ServiceUserRepository(UserConfigurationCollection users)
        {
            this.users = users;
        }

        public IUser Authenticate(string userName, string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            return (from UserConfiguration user in users
                    where user.Match(userName, password)
                    select new ServiceUser(user.Name, new[] { user.Role })).FirstOrDefault();
        }
    }
}