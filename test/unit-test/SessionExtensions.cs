using System;
using Calendar.Persistence;

namespace Calendar.Tests.Unit
{
    public static class SessionExtensions
    {
        public static void DeleteAll(this ISessionWrapper session, Type type)
        {
            session.Delete(String.Format("from {0}", type.Name));
        }
    }
}