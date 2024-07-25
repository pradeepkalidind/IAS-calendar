using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Calendar.Model.Compact.Concurrency;
using NHibernate.Exceptions;

namespace Calendar.Model.Compact
{
    public class LocationPatternMap
    {
        private static volatile List<LocationPattern> locationPatternMaps = new List<LocationPattern>();

        protected static readonly ReaderWriterLockSlim lockSlim =
            new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public static void Init()
        {
            locationPatternMaps = new List<LocationPattern>();
        }

        public LocationPattern GetFromMap(Int64 id)
        {
            return GetFromMapById(id) ?? LocationPattern.Empty();
        }

        public void AddToMap(LocationPattern locationPattern)
        {
            try
            {
                var hash = locationPattern.Hash;
                if (ExistInCache(locationPattern)) return;

                InsertToDbWhenNotExist(locationPattern, hash);
            }
            catch (GenericADOException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("Cannot insert duplicate key "))
                {
                    AddToMap(locationPattern);
                }
                else
                {
                    throw;
                }
            }
        }

        private static LocationPattern GetFromMapById(Int64 id)
        {
            return GetFromCache(id) ?? GetFromDb(id);
        }

        private static void InsertToDbWhenNotExist(LocationPattern locationPattern, int hash)
        {
            var dbLocationPatterns = GetFromDbByHash(hash);
            var dbCount = dbLocationPatterns.Count();
            var dbExist = dbCount > 0 && dbLocationPatterns.Any(l => l.Equals(locationPattern));
            if (dbExist)
            {
                locationPattern.Id = dbLocationPatterns.First(l => l.Equals(locationPattern)).Id;
                return;
            }

            Insert(locationPattern);
        }

        private static bool ExistInCache(LocationPattern locationPattern)
        {
            var locationPatterns = GetFromCacheByHash(locationPattern.Hash);
            var count = locationPatterns.Count();
            var existInCache = count > 0 && locationPatterns.Any(l => l.Equals(locationPattern));
            if (existInCache)
            {
                locationPattern.Id = locationPatterns.First(l => l.Equals(locationPattern)).Id;
            }
            return existInCache;
        }

        private static LocationPattern GetFromCache(Int64 id)
        {
            using (new AutoReadLock(lockSlim))
            {
                return locationPatternMaps.FirstOrDefault(l => l.Id == id);
            }
        }

        private static IEnumerable<LocationPattern> GetFromCacheByHash(int hash)
        {
            using (new AutoReadLock(lockSlim))
            {
                return locationPatternMaps.Where(l => l.Hash == hash).ToArray();
            }
        }

        private static LocationPattern GetFromDb(Int64 id)
        {
            using (var session = DbConfigurationFactory.Get().GetSession())
            {
                var locationPattern = session.Get<LocationPattern>(id);
                if (locationPattern != null)
                {
                    using (new AutoWriteLock(lockSlim))
                    {
                        locationPatternMaps.Add(locationPattern);
                    }
                }

                return locationPattern;
            }
        }

        private static IEnumerable<LocationPattern> GetFromDbByHash(int hash)
        {
            using (var session = DbConfigurationFactory.Get().GetSession())
            {
                var locationPatterns = session.Query<LocationPattern>().Where(l => l.Hash == hash).ToArray();
                using (new AutoWriteLock(lockSlim))
                {
                    locationPatternMaps.AddRange(locationPatterns);
                }
                return locationPatterns;
            }
        }

        private static void Insert(LocationPattern locationPattern)
        {
            using (var session = DbConfigurationFactory.Get().GetSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(locationPattern);
                    transaction.Commit();
                }
            }
        }
    }
}