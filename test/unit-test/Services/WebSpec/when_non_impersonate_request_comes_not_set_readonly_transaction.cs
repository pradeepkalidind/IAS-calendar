using System.Security.Principal;
using Calendar.Authentication;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    internal class BasicUser : IPrincipal
    {
        public bool IsInRole(string role)
        {
            return role.Equals(Roles.BASIC_USER);
        }

        public IIdentity Identity
        {
            get { return null; }
        }
    }
}