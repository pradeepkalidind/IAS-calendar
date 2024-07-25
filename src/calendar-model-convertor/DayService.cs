using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Dto;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Newtonsoft.Json;

namespace Calendar.Model.Convertor
{
    public class DayService
    {
        private readonly ISessionWrapper session;

        public DayService(ISessionWrapper session)
        {
            this.session = session;
        }

        public void SaveOrDelete(string json, string userId, string applicationId)
        {
            try
            {
                var guid = new Guid(userId);
                var models = JsonConvert.DeserializeObject<ChangedDaysDto>(json);
                models.updatedDays.ForEach(d => d.enterFrom = models.enterFrom);
                var daysContext = BuildDaysContext(guid,models.updatedDays,models.deletedDays);
                daysContext.ExtendAction(sp=>Repository.SaveUserActivity(sp,daysContext.UserId,daysContext.Dates, applicationId))
                    .Save(session);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public DaysContext BuildDaysContext(Guid userId, IEnumerable<DayDto> updatedDayDtos, IEnumerable<DeletedDayDto> deletedDayDtos)
        {
            var datesOfUpdated = updatedDayDtos.Select(d => GetDate(d.date)).ToArray();
            var datesOfDeleted = deletedDayDtos.Select(d => GetDate(d.date)).ToArray();
            var datesAll = datesOfUpdated.Concat(datesOfDeleted).ToArray();

            var notes = updatedDayDtos.Where(d => !string.IsNullOrEmpty(d.note))
                .Select(d => new Note(userId, GetDate(d.date), d.note));

            var monthActivities = new MonthActivityCollection(userId, session)
                .AddDayActivities(updatedDayDtos, deletedDayDtos)
                .MonthActivities;

            return new DaysContext(userId, datesAll, notes, monthActivities);
        }

        private static DateTime GetDate(string d)
        {
            return DateTime.Parse(d);
        }
    }
}