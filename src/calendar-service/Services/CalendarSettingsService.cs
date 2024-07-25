using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Service.Models;
using Newtonsoft.Json;
using NLog;

namespace Calendar.Service.Services
{
    public interface ICalendarSettingsService
    {
        DefaultWorkDaysDto GetCalendarSettings(Guid userId);
        object GetCalendarSettingsDto(Guid userId, string culture);
        void SaveWorkWeekDefaults(Guid userId, int[] workWeekDefaults);
        void SaveNationalHoliday(Guid userId, string country);
    }

    public class CalendarSettingsService : ICalendarSettingsService
    {
        public ISessionWrapper session;
        public INationalHolidayService holidayService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public CalendarSettingsService(ISessionWrapper session)
        {
            this.session = session;
            holidayService = new NationalHolidayService();
        }

        public DefaultWorkDaysDto GetCalendarSettings(Guid userId)
        {
            var defaultWorkDays = GetSettings(userId);
            return defaultWorkDays.ToCalendarSettingDto();
        }

        public object GetCalendarSettingsDto(Guid userId, string culture)
        {
            return new
            {
                WorkDays = GetDefaultWorkDays(userId).wwd,
                NationalHoliday = GetNationalHolidaySettingDto(userId, culture)
            };
        }

        public DefaultWorkDaysDto GetDefaultWorkDays(Guid userId)
        {
            var defaultWorkDays = GetSettings(userId);
            return defaultWorkDays.ToCalendarSettingDto();
        }

        public NationalHolidaySettingDto GetNationalHolidaySettingDto(Guid userId, string culture)
        {
            var record = session.Query<NationalHolidaySetting>().FirstOrDefault(setting => setting.UserId == userId);
            if (record == null) return null;

            var holidays = holidayService.GetNationalHolidaysDetailByCountry(record.Country, culture);
            var holidayTranslations = holidays.Select(x =>
            {
                try
                {
                    var date = new DateTime(x.date.datetime.year, x.date.datetime.month, x.date.datetime.day).ToString("yyyy-MM-dd");
                    return new HolidayTranslation
                    {
                        Date = date,
                        I18n = x.name
                    };
                }
                catch (Exception)
                {
                    var errorMessage = string.Format("error-type: national-holiday-date-format, country: {0}, holiday-name: {1}", record.Country, x.name);
                    Logger.Error(errorMessage);
                    return null;
                }
            })
                .Where(x => x != null)
                .ToArray();

            return new NationalHolidaySettingDto {Country = record.Country, Holidays = holidayTranslations};
        }

        public void SaveWorkWeekDefaults(Guid userId, int[] workWeekDefaults)
        {
            var settings = GetSettings(userId);
            settings.Days = GetDaysByte(workWeekDefaults);
            settings.Timestamp = DateTime.UtcNow.Ticks;
            session.SaveOrUpdateAndFlush(settings);
        }

        public void SaveNationalHoliday(Guid userId, string country)
        {
            var record = session.Query<NationalHolidaySetting>().FirstOrDefault(setting => setting.UserId == userId);
            record ??= new NationalHolidaySetting(userId, country);
            record.Country = country;
            session.SaveOrUpdateAndFlush(record);
        }

        public DefaultWorkDays GetSettings(Guid userId)
        {
            return session.Query<DefaultWorkDays>().FirstOrDefault(r => r.UserId == userId) ??
                new DefaultWorkDays() {UserId = userId, Days = 124};
        }

        public static byte GetDaysByte(int[] weekWorkDays)
        {
            const int WEEK_DAYS = 7;
            var daysByte = 0;
            if (weekWorkDays.Contains(0))
            {
                weekWorkDays = weekWorkDays.Where(day => day != 0).ToArray();
                daysByte = 1;
            }

            daysByte += weekWorkDays.Sum(workDay => (int) Math.Pow(2, WEEK_DAYS - workDay));
            return (byte) daysByte;
        }

        public List<General.Dto.DefaultWorkDaysDto> GetDefaultWorkingDaysForMobile(Guid userId)
        {
            var record = GetSettings(userId);
            var records = new List<DefaultWorkDays>();
            if (record != null)
            {
                records.Add(record);
            }

            return records.Select(ToJsonDto).ToList();
        }

        private static General.Dto.DefaultWorkDaysDto ToJsonDto(DefaultWorkDays model)
        {
            return new General.Dto.DefaultWorkDaysDto()
            {
                userId = model.UserId,
                days = model.Days
            };
        }

        public void CreateOrUpdate(string json, string userId)
        {
            General.Dto.DefaultWorkDaysDto daysDto;
            try
            {
                daysDto = Deserialize(json);
                daysDto.userId = userId != null ? Guid.Parse(userId) : daysDto.userId;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }

            using (var transaction = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                CreateOrUpdate(daysDto);
                transaction.Commit();
            }
        }

        private void CreateOrUpdate(General.Dto.DefaultWorkDaysDto dto)
        {
            var record = session.Query<DefaultWorkDays>().FirstOrDefault(r => r.UserId == dto.userId);
            if (record == null)
            {
                session.CreateAndFlush(new DefaultWorkDays()
                {
                    Days = dto.days,
                    UserId = dto.userId
                });
                return;
            }

            if (Update(record, dto))
            {
                session.SaveOrUpdateAndFlush(record);
            }
        }

        private static bool Update(DefaultWorkDays recordFromDB, General.Dto.DefaultWorkDaysDto newRecord)
        {
            if (recordFromDB.Days == newRecord.days) return false;
            recordFromDB.Days = newRecord.days;
            recordFromDB.Timestamp = DateTime.UtcNow.Ticks;
            return true;
        }

        private General.Dto.DefaultWorkDaysDto Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<General.Dto.DefaultWorkDaysDto>(json);
        }
    }
}
