using System;
using System.Xml.Serialization;

namespace Calendar.Client.Schema
{
    [Serializable]
    public class Arrival
    {
        [XmlAttribute(AttributeName = "Location")]
        public string Location { get; set; }

        [XmlAttribute(AttributeName = "Date")]
        public string Date { get; set; }

        [XmlAttribute(AttributeName = "Time")]
        public string Time { get; set; }
    }
}