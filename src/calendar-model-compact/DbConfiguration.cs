using System;
using Calendar.Persistence;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;

namespace Calendar.Model.Compact
{
    public class DbConfiguration : IDbConfiguration
    {
        protected readonly ISessionFactory sessionFactory;
        protected readonly Configuration configuration;

        protected DbConfiguration(IPersistenceConfigurer configurer)
        {
            var autoPersistenceModel = AutoMap.AssemblyOf<MonthActivity>(new CalendarDbMappingConfig())
                .Conventions.Add(ConventionBuilder.Id.Always(c => c.GeneratedBy.Assigned()))
                .Override<LocationPattern>(m =>
                {
                    m.Id(l => l.Id).GeneratedBy.Identity();
                    m.Not.LazyLoad();
                })
                .Override<MonthActivity>(m => m.Not.LazyLoad())
                .Override<Note>(m => m.Not.LazyLoad())
                .Override<DefaultWorkDays>(m => m.Not.LazyLoad())
                .Override<LastModifiedYearMonth>(m => m.Not.LazyLoad())
                .Override<VisitedUser>(m => m.Not.LazyLoad())
                .Override<NationalHolidaySetting>(m => m.Not.LazyLoad())
                .Override<UserActivity>(u => u.Not.LazyLoad());

            configuration = Fluently.Configure()
                .Database(configurer)
                .Mappings(m => m.AutoMappings.Add(autoPersistenceModel))
                .BuildConfiguration();

            sessionFactory = configuration.BuildSessionFactory();
        }

        public virtual ISessionWrapper GetSession()
        {
            lock (typeof(DbConfiguration))
            {
                return new PersistSession(sessionFactory.OpenSession());
            }
        }

        public virtual ISession GetNHibernateSession()
        {
            lock (typeof(DbConfiguration))
            {
                return sessionFactory.OpenSession();
            }
        }

        public IStatelessSession GetStatelessSession()
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
                if (member.IsProperty && member.IsPrivate)
                {
                    return true;
                }

                return base.ShouldMap(member);
            }

            public override string GetComponentColumnPrefix(Member member)
            {
                return "";
            }

            public override bool ShouldMap(Type type)
            {
                return typeof(MonthActivity).IsAssignableFrom(type)
                       || typeof(LocationPattern).IsAssignableFrom(type)
                       || typeof(Note).IsAssignableFrom(type)
                       || typeof(UserActivity).IsAssignableFrom(type)
                       || typeof(UserActivityArchived).IsAssignableFrom(type)
                       || typeof(NationalHolidaySetting).IsAssignableFrom(type)
                       || typeof(DefaultWorkDays).IsAssignableFrom(type)
                       || typeof(LastModifiedYearMonth).IsAssignableFrom(type)
                       || typeof(VisitedUser).IsAssignableFrom(type);
            }
        }
    }
}