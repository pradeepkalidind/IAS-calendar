using System;
using System.Xml.Serialization;

namespace Calendar.Client.Schema
{
    [Serializable]
    public class Note
    {
        [XmlAttribute(AttributeName = "Content")]
        public string Content { get; set; }
    }
}