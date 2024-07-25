using Calendar.Authentication;

namespace Calendar.General.BasicAuthorization
{
    public class ServiceUser : IUser
    {
        public string UserName { get; private set; }

        public string Identity
        {
            get { return UserName; }
        }

        public string[] Roles { get; private set; }

        public ServiceUser(string userName, string[] roles)
        {
            UserName = userName;
            Roles = roles;
        }
    }
}