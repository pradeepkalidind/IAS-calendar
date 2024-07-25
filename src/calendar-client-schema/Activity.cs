using System;
using System.Xml.Serialization;

namespace Calendar.Client.Schema
{
    [Serializable]
    public class Activity
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlIgnore]
        public double? FirstAllocation { get; set; }

        [XmlIgnore]
        public double? SecondAllocation { get; set; }

        [XmlAttribute(AttributeName = "FirstAllocation")]
        public string FirstAllocationAttr
        {
            get { return FirstAllocation.HasValue ? FirstAllocation.Value.ToString() : null; }
            set
            {
                double result;
                FirstAllocation = double.TryParse(value, out result) ? result : (double?)null;
            }
        }

        [XmlAttribute(AttributeName = "SecondAllocation")]
        public string SecondAllocationAttr
        {
            get { return SecondAllocation.HasValue ? SecondAllocation.Value.ToString() : null; }
            set
            {
                double result;
                SecondAllocation = double.TryParse(value, out result) ? result : (double?)null;
            }
        }
    }
}