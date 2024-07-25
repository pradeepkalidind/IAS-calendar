using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace Calendar.Client.Schema
{
    [Serializable]
    [XmlRoot(ElementName = "Calendar")]
    public class UserCalendar
    {
        [XmlArray(ElementName = "Days")]
        [XmlArrayItem(ElementName = "Day", Type = typeof(Day))]
        public HashSet<Day> Days = new HashSet<Day>();

        [XmlAttribute(AttributeName = "IasPlatformUser")]
        public string IasPlatformUser { get; set; }

        public UserCalendar(string iasPlatformUser)
        {
            IasPlatformUser = iasPlatformUser;
        }

        public UserCalendar()
        {
            
        }

        private void AddRange(IEnumerable<Day> days)
        {
            days.ToList().ForEach(d => Days.Add(d));
        }

        public static UserCalendar Build(Guid userId, IEnumerable<Day> days)
        {
            var calendar = new UserCalendar(userId.ToString());
            calendar.AddRange(days);
            calendar.SortDays();
            return calendar;
        }

        private void SortDays()
        {
            var days = Days.OrderBy(d => DateTime.Parse(d.Date));
            Days = new HashSet<Day>();
            foreach (var day in days)
            {
                Days.Add(day);
            }
        }
    }
    
    [Serializable]
    [XmlRoot(ElementName = "Calendar")]
    public class UserCalendarWithDeleted
    {
        [XmlArray(ElementName = "Days")]
        [XmlArrayItem(ElementName = "Day", Type = typeof(Day))]
        public HashSet<Day> Days = new HashSet<Day>();

        [XmlArray(ElementName = "DeletedDays")]
        [XmlArrayItem(ElementName = "DeletedDay", Type = typeof(DeletedDay))]
        public HashSet<DeletedDay> DeletedDays = new HashSet<DeletedDay>();

        [XmlAttribute(AttributeName = "IasPlatformUser")]
        public string IasPlatformUser { get; set; }

        public UserCalendarWithDeleted(UserCalendar userCalendar)
        {
            Days = userCalendar.Days;
            IasPlatformUser = userCalendar.IasPlatformUser;
        }

        public UserCalendarWithDeleted()
        {
            
        }

        public static UserCalendarWithDeleted Build(UserCalendar userCalendar, IEnumerable<DateTime> changedDays)
        {
            var userCalendarWithDeleted = new UserCalendarWithDeleted(userCalendar);
            var deletedDays = changedDays.Select(d=>d.ToString("yyyy-MM-dd"))
                .Except(userCalendar.Days.Select(d=>d.Date)).OrderBy(d=>d);
            deletedDays.ToList().ForEach(d => userCalendarWithDeleted.DeletedDays.Add(new DeletedDay() { Date = d }));
            return userCalendarWithDeleted;
        }
    }
}