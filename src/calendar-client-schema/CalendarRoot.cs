using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Calendar.Client.Schema
{
    [Serializable]
    [XmlRoot(ElementName = "CalendarRoot")]
    public class CalendarRoot
    {
        [XmlArray(ElementName = "UserCalendars")]
        [XmlArrayItem(ElementName = "Calendar", Type = typeof(UserCalendar))]
        public List<UserCalendar> Users = new List<UserCalendar>();
    }
}