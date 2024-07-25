using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Dto;
using Calendar.Model.Compact;
using Calendar.Model.Convertor;
using Calendar.Service.Extensions;
using Calendar.Service.Filters;
using Calendar.Service.Models;
using Calendar.Service.Requests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vialto.NHibernate.Extensions;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class CalendarController : NewApiController
    {
        [HttpPost("Calendar/GetYear")]
        public ActionResult GetYear([FromQuery] string token, [FromForm] CalendarGetYearRequest request)
        {
            var userId = GetUserId(token);
            var monthActivities = session.Query<MonthActivity>().Where(ma => ma.UserId == userId && ma.Year == request.Year).ToArray();
            var notes = session.Query<Note>().Where( note =>note.UserId == userId && note.Date >= new DateTime(request.Year, 1, 1) && note.Date < new DateTime(request.Year + 1, 1, 1)).ToArray();
            return Ok(new { year = new MonthModelToDtoConvertor(monthActivities, notes).GetYearDto(request.Year) });
        }

        [HttpPost("Calendar/GetYears")]
        public ActionResult GetYears([FromQuery] string token, [FromForm] CalendarGetYearsRequest request)
        {
            var userId = GetUserId(token);
            var monthActivities = session.Query<MonthActivity>()
                .BatchFind(request.Years, batch => e => e.UserId == userId && batch.Contains(e.Year)).ToList();
            var notes = session.Query<Note>().Where(note => note.UserId == userId &&
                                                            note.Date >= new DateTime(request.Years.Min(), 1, 1) &&
                                                            note.Date < new DateTime(request.Years.Max() + 1, 1, 1))
                .ToArray();
            var yearsContent = request.Years.Select(theYear => new
                {
                    year = theYear, content = new MonthModelToDtoConvertor(monthActivities, notes).GetYearDto(theYear)
                })
                .ToArray();
            return Ok(yearsContent);
        }

        [HttpPost("Calendar/GetTravelData")]
        public ActionResult GetTravelData([FromBody] TravelDataRequest request)
        {
            var userId = GetUserId(request.Token);
            var queryable = session.Query<MonthActivity>().Where(ma => ma.UserId == userId);
            if (!queryable.Any())
            {
                return Ok(null);
            }

            var travelCountriesInfos = GetTravelCountriesInfos(queryable);
            var latestUserActivityTime = GetLatestUserActivityTime(userId);
            return Ok(new { lastUpdatedTime = latestUserActivityTime, travelCountriesInfos });
        }

        private DateTime? GetLatestUserActivityTime(Guid userId)
        {
            var latestUserActivity = session.Query<UserActivity>().Where(u => u.UserId == userId).OrderByDescending(u => u.Timestamp).FirstOrDefault();
            if (latestUserActivity != null)
            {
                return new DateTime(latestUserActivity.Timestamp, DateTimeKind.Utc);
            }
            var latestUserActivityArchived = session.Query<UserActivityArchived>().Where(u => u.UserId == userId).OrderByDescending(u => u.Timestamp).FirstOrDefault();
            if (latestUserActivityArchived != null)
            {
                return new DateTime(latestUserActivityArchived.Timestamp, DateTimeKind.Utc);
            }
            return null;
        }

        private static List<TravelCountriesInfoDto> GetTravelCountriesInfos(IQueryable<MonthActivity> queryable)
        {
            var monthActivitys = queryable.ToList();
            var years = monthActivitys.Select(r => r.Year).Distinct().OrderByDescending(r => r).ToList();
            var travelCountriesInfos = new List<TravelCountriesInfoDto>();
            foreach (var year in years)
            {
                var travelDaysInCountries = GetTravelDaysInCountriesByYear(monthActivitys, year);
                if (travelDaysInCountries.Count == 0)
                {
                    if (travelCountriesInfos.Count > 0) break;
                    continue;
                }
                travelCountriesInfos.Add(new TravelCountriesInfoDto
                {
                    year = year,
                    countries = travelDaysInCountries
                });
                if (!years.Contains(year - 1) || travelCountriesInfos.Count == 2) break;
            }
            return travelCountriesInfos;
        }

        private static Dictionary<string, double> GetTravelDaysInCountriesByYear(List<MonthActivity> monthActivitys, int year)
        {
            var monthModelToDtoConvertor = new MonthModelToDtoConvertor(monthActivitys.Where(r => r.Year == year).ToArray(), null);
            var travelDaysInCountries = monthModelToDtoConvertor.GetTravelDaysInCountries(year);
            return travelDaysInCountries;
        }

        [HttpPost("Calendar/SaveDays")]
        public ActionResult SaveDays([FromQuery] string token, [FromForm] CalendarSaveDaysRequest request)
        {
            var userId = GetUserId(token);
            var dayActivities = JsonConvert.DeserializeObject<List<CalendarDayDto>>(request.daysBody);

            var dates = dayActivities.Select(d => GetDate(d.D)).ToArray();

            var notes = dayActivities.Where(d => !string.IsNullOrEmpty(d.N))
                .Select(d => new Note(userId, GetDate(d.D), d.N));

            var monthActivities = new MonthActivityCollection(userId, session)
                .AddDayActivities(dayActivities)
                .MonthActivities;

            var daysContext = new DaysContext(userId, dates, notes, monthActivities);
            if (!HttpContext.IsMigrating())
            {
                daysContext.ExtendAction(s => Repository.SaveUserActivity(s, daysContext.UserId, daysContext.Dates));
            }
            daysContext.Save(session);
            return Ok();
        }

        [HttpPost("Calendar/SaveNoTravel")]
        public void SaveNoTravel(string token, [FromForm] SaveNoTravelRequest request)
        {
            var userId = GetUserId(token);
            var monthActivity = session.Query<MonthActivity>().FirstOrDefault(ma => ma.UserId == userId && ma.Year == request.Year && ma.Month == request.Month) ?? new MonthActivity(userId, request.Year, request.Month);
            monthActivity.setNoTravelConfirmation(request.HasNoTravelConfirmation);
            session.SaveOrUpdate(monthActivity);
        }


        private static DateTime GetDate(string d)
        {
            return DateTime.Parse(d);
        }

        private static Guid GetUserId(string userId)
        {
            return new Guid(userId);
        }
    }
}