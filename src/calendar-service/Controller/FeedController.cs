using System;
using System.ServiceModel.Syndication;
using System.Text;
using Calendar.Feed.Services;
using Calendar.General.Configuration;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Service.Filters;
using Calendar.Service.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SyndicationFeedFormatter = Calendar.Service.Formatters.SyndicationFeedFormatter;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class FeedController : NewApiController
    {
        public int Duration { get; set; } 
        private readonly FeedEntryRepository repo;
        private readonly ISessionWrapper oldSession;

        public FeedController()
        {
            Duration = FeedConfiguration.Instance.Duration;
            oldSession = DbInitializerFactory.Get().GetSession();
            repo = new FeedEntryRepository(oldSession);
        }

        protected override void DisposeManagedResource()
        {
            oldSession?.Dispose();
        }

        [HttpGet("CalendarChanged.atom")]
        public ActionResult Index()
        {
            var lastUpdatedTime = GetLastUpdatedTime();
            if (lastUpdatedTime == DateTime.MinValue)
            {
                var feed = new SyndicationFeed(new SyndicationItem[0]) { LastUpdatedTime = DateTime.UtcNow };
                return Content(SyndicationFeedFormatter.Format(feed), SyndicationFeedFormatter.ATOM_MEDIA_TYPE, Encoding.UTF8);
            }

            return Archive(lastUpdatedTime.Year, lastUpdatedTime.Month, lastUpdatedTime.Day, lastUpdatedTime.Hour, lastUpdatedTime.Minute);
        }

        private DateTime GetLastUpdatedTime()
        {
            var dateTime = DateTime.UtcNow;
            var finishTimestamp = dateTime.ToUniversalTime().Ticks;
            var lastUpdatedTime = GetLastUpdatedTime(dateTime);
            var lastUpdatedTimeForNewApi = GetLastUpdatedTimeForNewApi(finishTimestamp);
            return lastUpdatedTimeForNewApi > lastUpdatedTime ? lastUpdatedTimeForNewApi : lastUpdatedTime;
        }

        private DateTime GetLastUpdatedTime(DateTime dateTime)
        {
            var entry = repo.FindMostRecentBefore(dateTime);
            return entry == null ? DateTime.MinValue: entry.LastUpdatedTime;
        }

        private DateTime GetLastUpdatedTimeForNewApi(long finishTimestamp)
        {
            var newFeed = new Repository(session).FindMostRecentBefore(finishTimestamp);
            return newFeed == null ? DateTime.MinValue : new DateTime(newFeed.Timestamp, DateTimeKind.Utc);
        }

        [HttpGet("{year}/{month}/{day}/{hour}/{minute}/CalendarChanged.atom")]
        public ActionResult Archive(int year, int month, int day, int hour, int minute)
        {
            var dateTime = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
            var currentPeriod = PeriodCalculator.Calculate(dateTime, Duration);

            var prevArchiveUri = GetPrevArchiveUri(currentPeriod.StartTime);
            var userFeedCollector = new UserFeedCollector(currentPeriod, session, repo);
            var syndicationFeed = userFeedCollector
                .ToSyndicationFeed(GetHost(), prevArchiveUri, dateTime);

            return Content(SyndicationFeedFormatter.Format(syndicationFeed), SyndicationFeedFormatter.ATOM_MEDIA_TYPE, Encoding.UTF8);
        }

        private Uri GetPrevArchiveUri(DateTime startTime)
        {
            var previousUpdateTime = GetPreviousUpdateTime(startTime);

            Uri prevArchiveUri = null;
            if (previousUpdateTime != null)
            {
                var previousPeriod = PeriodCalculator.Calculate(previousUpdateTime.Value, Duration);
                prevArchiveUri = new Uri(string.Format("{0}/{1}/{2}/{3}/{4}/{5}/CalendarChanged.atom",
                                                       GetHost(),
                                                       previousPeriod.StartTime.Year,
                                                       previousPeriod.StartTime.Month,
                                                       previousPeriod.StartTime.Day,
                                                       previousPeriod.StartTime.Hour,
                                                       previousPeriod.StartTime.Minute));
            }
            return prevArchiveUri;
        }

        private DateTime? GetPreviousUpdateTime(DateTime startTime)
        {
            var newFeed = new Repository(session).FindMostRecentBefore(startTime.ToUniversalTime().Ticks);
            var prevMostRecentEntry = repo.FindMostRecentBefore(startTime);

            if (newFeed != null && prevMostRecentEntry != null)
            {
                return new DateTime(Math.Max(newFeed.Timestamp, prevMostRecentEntry.LastUpdatedTime.Ticks));
            }

            if (prevMostRecentEntry != null)
            {
                return prevMostRecentEntry.LastUpdatedTime;
            }

            if (newFeed != null)
            {
                return new DateTime(newFeed.Timestamp);
            }
            return null;
        }

        private string GetHost()
        {
            var url = new Uri(Request.GetEncodedUrl());
            return $"{url.Scheme}://{url.Authority}";
        }
    }
}