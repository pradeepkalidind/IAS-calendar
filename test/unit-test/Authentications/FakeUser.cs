using Calendar.Authentication;

namespace Calendar.Tests.Unit.Authentications
{
    public class FakeUser : IUser
    {
        private readonly string identify;
        private readonly string[] roles;

        public FakeUser(string identify, string[] roles)
        {
            this.identify = identify;
            this.roles = roles;
        }

        public string Identity
        {
            get { return identify; }
        }

        public string[] Roles
        {
            get { return roles; }
        }
    }
}