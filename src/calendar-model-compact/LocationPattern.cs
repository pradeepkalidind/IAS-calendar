using System;

namespace Calendar.Model.Compact
{
    public class LocationPattern
    {
        public LocationPattern()
        {
            Hash = GetHashCode();
        }

        public LocationPattern(string firstLocation = null, int? firstLocationArrivalTime = null, int? firstLocationDepartureTime = null,
            string secondLocation = null, int? secondLocationArrivalTime = null, int? secondLocationDepartureTime = null,
            string thirdLocation = null,  int? thirdLocationArrivalTime = null)
        {
            FirstLocation = firstLocation;
            FirstLocationArrivalTime = firstLocationArrivalTime;
            FirstLocationDepartureTime = firstLocationDepartureTime;
            SecondLocation = secondLocation;
            SecondLocationArrivalTime = secondLocationArrivalTime;
            SecondLocationDepartureTime = secondLocationDepartureTime;
            ThirdLocation = thirdLocation;
            ThirdLocationArrivalTime = thirdLocationArrivalTime;
            Hash = GetHashCode();
        }

        public virtual Int64 Id { get; internal protected set; }

        public int Hash { get; set; }
        public string FirstLocation { get; private set; }
        public int? FirstLocationArrivalTime { get; private set; }
        public int? FirstLocationDepartureTime { get; private set; }
        public string SecondLocation { get; private set; }
        public int? SecondLocationArrivalTime { get; private set; }
        public int? SecondLocationDepartureTime { get; private set; }
        public string ThirdLocation { get; private set; }
        public int? ThirdLocationArrivalTime { get; private set; }

        public  bool Equals(LocationPattern other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.FirstLocation, FirstLocation) && other.FirstLocationDepartureTime.Equals(FirstLocationDepartureTime) && other.FirstLocationArrivalTime.Equals(FirstLocationArrivalTime) && Equals(other.SecondLocation, SecondLocation) && other.SecondLocationDepartureTime.Equals(SecondLocationDepartureTime) && other.SecondLocationArrivalTime.Equals(SecondLocationArrivalTime) && Equals(other.ThirdLocation, ThirdLocation) && other.ThirdLocationArrivalTime.Equals(ThirdLocationArrivalTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (LocationPattern)) return false;
            return Equals((LocationPattern) obj);
        }

        public override int GetHashCode()
        {
           return ToString().GetHashCode();
        }

        public static LocationPattern Empty()
        {
            return new LocationPattern();
        }

        public static int EMPTY_ID = Empty().Hash;

        public override string ToString()
        {
            return String.Format("{0}:{1}-{2} {3}:{4}-{5} {6}:{7}",
                FirstLocation, FormatHour(FirstLocationArrivalTime), FormatHour(FirstLocationDepartureTime),
                SecondLocation, FormatHour(SecondLocationArrivalTime), FormatHour(SecondLocationDepartureTime),
                ThirdLocation, FormatHour(ThirdLocationArrivalTime));
        }

        private static string FormatHour(int? hour)
        {
            return hour.HasValue ? hour.Value.ToString("00") : "NA";
        }

       
        public static LocationPattern From(int index, byte[] source)
        {
            var patternId = BitConverter.ToInt64(source, index * 8);
            var locationPatternMap = new LocationPatternMap();
            return locationPatternMap.GetFromMap(patternId);
        }

        public void WriteTo(int index, byte[] dest)
        {
            new LocationPatternMap().AddToMap(this);
            Buffer.BlockCopy(BitConverter.GetBytes(Id), 0, dest, index * 8, 8);
        }

        public bool IsFirstLocationComplete()
        {
            return IsLocationComplete(FirstLocation);
        }

        public bool IsSecondLocationComplete()
        {
            return IsLocationComplete(SecondLocation);
        }

        public bool IsThirdLocationComplete()
        {
            return IsLocationComplete(ThirdLocation); ;
        }
        
        public bool IsLastLocationComplete()
        {
            return IsLocationComplete(LastLocation()); ;
        }

        private static bool IsLocationComplete(string location)
        {
            return !IsLocationEmpty(location) && !IsLocationInTransit(location);
        }

        private static bool IsLocationEmpty(string location)
        {
            return String.IsNullOrEmpty(location);
        }

        private static bool IsLocationInTransit(string location)
        {
            return Intransit.Equals(location);
        }

        public bool IsNoLocation()
        {
            return IsLocationEmpty(FirstLocation) && IsLocationEmpty(SecondLocation) && IsLocationEmpty(ThirdLocation);
        }

        public bool HasRoundTravel()
        {
            return IsFirstLocationComplete() && IsSecondLocationComplete() && IsThirdLocationComplete() && 
                   FirstLocation.Equals(ThirdLocation);
        }

        private string LastLocation()
        {
            return !IsLocationEmpty(ThirdLocation) ? ThirdLocation : !IsLocationEmpty(SecondLocation) ? SecondLocation : FirstLocation;
        }

        public bool IsFristLocationEmpty()
        {
            return String.IsNullOrEmpty(FirstLocation);
        }

        public const string Intransit = "/Country/INTRANSIT";

        public bool NextDayFirstLocationNotEqualsLastLocation(LocationPattern nextLocationPattern)
        {
            if (nextLocationPattern.IsFristLocationEmpty())
                return false;

            if (!IsLastLocationComplete())
                return false;

            return LastLocation() != nextLocationPattern.FirstLocation;
        }
    }

    public class PatternId
    {
        public bool Equals(PatternId other)
        {
            return other.Hash == Hash && other.Offset == Offset;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (PatternId)) return false;
            return Equals((PatternId) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Hash*397) ^ Offset.GetHashCode();
            }
        }

        public PatternId()
        {
        }

        public PatternId(int hash, byte offset)
        {
            Hash = hash;
            Offset = offset;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Hash, Offset);
        }

        public  int Hash { get;  set; }
        public  byte Offset { get;  set; }
    } 
}