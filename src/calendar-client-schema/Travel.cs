using System;
using System.Xml.Serialization;

namespace Calendar.Client.Schema
{
    [Serializable]
    public class Travel
    {

        [XmlElement(ElementName = "Departure")]
        public Departure Departure { get; set; }

        [XmlElement(ElementName = "Arrival")]
        public Arrival Arrival { get; set; }

    }
}