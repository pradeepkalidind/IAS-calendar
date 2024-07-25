using Calendar.Persistence;

namespace Calendar.General.Persistence
{
    public static class RepositoryExtensions
    {
        public static void CreateAndFlush(this ISessionWrapper session, object entity)
        {
            session.Save(entity);
        }

        public static void UpdateAndFlush(this ISessionWrapper session, object entity)
        {
            session.Update(entity);
        }

        public static void SaveOrUpdateAndFlush(this ISessionWrapper session, object entity)
        {
            session.SaveOrUpdate(entity);
        }
    }
}