using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Calendar.Client.Schema
{
    [Serializable]
    public class Day
    {
        [XmlAttribute(AttributeName = "Date")]
        public string Date { get; set; }

        [XmlAttribute(AttributeName = "Location")]
        public string Location { get; set; }

        [XmlAttribute(AttributeName = "ArrivalTime")]
        public string ArrivalTime { get; set; }

        [XmlAttribute(AttributeName = "DepartureTime")]
        public string DepartureTime { get; set; }
        
        [XmlAttribute(AttributeName = "IsCovid")]
        public string IsCovid { get; set; }

        [XmlArray(ElementName = "Activities")]
        [XmlArrayItem(ElementName = "Activity", Type = typeof(Activity))]
        public List<Activity> Activities { get; set; }

        [XmlElement(ElementName = "Note")]
        public Note Note { get; set; }

        [XmlArray(ElementName = "Travels")]
        [XmlArrayItem(ElementName = "Travel", Type = typeof(Travel))]
        public List<Travel> Travels { get; set; }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType().Equals(typeof(Day)) && Date.Equals(((Day)obj).Date);
        }

        public override int GetHashCode()
        {
            return Date.GetHashCode();
        }

    }
    
    [Serializable]
    public class DeletedDay
    {
        [XmlAttribute(AttributeName = "Date")]
        public string Date { get; set; }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType().Equals(typeof(DeletedDay)) && Date.Equals(((DeletedDay)obj).Date);
        }

        public override int GetHashCode()
        {
            return Date.GetHashCode();
        }

    }
}