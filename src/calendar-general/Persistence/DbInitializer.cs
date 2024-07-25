using System;
using Calendar.Models.Feed;
using Calendar.Persistence;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using Inflector;
using NHibernate;

namespace Calendar.General.Persistence
{
    public class DbInitializer : IDbConfiguration
    {
        protected readonly ISessionFactory sessionFactory;
        protected readonly NHibernate.Cfg.Configuration configuration;

        public DbInitializer(IPersistenceConfigurer configurer)
        {
            var autoPersistenceModel = AutoMap.AssemblyOf<FeedArchivedEntry>(new CalendarDbMappingConfig())
                .Conventions.Add(Table.Is(c => c.EntityType.Name.Pluralize()),
                    ConventionBuilder.Id.Always(c => c.GeneratedBy.Assigned()))
                .Override<FeedArchivedEntry>(m => m.Table("FeedArchivedEntries"));

            configuration = Fluently.Configure()
                .Database(configurer)
                .Mappings(m => m.AutoMappings.Add(autoPersistenceModel))
                .BuildConfiguration();
            sessionFactory = configuration.BuildSessionFactory();
        }

        public virtual ISession GetNHibernateSession()
        {
            lock (sessionFactory)
            {
                return sessionFactory.OpenSession();
            }
        }

        public virtual ISessionWrapper GetSession()
        {
            lock (sessionFactory)
            {
                return new PersistSession(sessionFactory.OpenSession());
            }
        }

        public virtual IStatelessSession GetStatelessSession()
        {
            return sessionFactory.OpenStatelessSession();
        }

        private class CalendarDbMappingConfig : DefaultAutomappingConfiguration
        {
            public override bool IsVersion(Member member)
            {
                return false;
            }

            public override bool ShouldMap(Member member)
            {
                if (member.IsProperty && !member.CanWrite)
                {
                    return false;
                }

                return base.ShouldMap(member);
            }

            public override bool ShouldMap(Type type)
            {
                return type == typeof(FeedEntry) || type == typeof(FeedArchivedEntry);
            }
        }
    }
}