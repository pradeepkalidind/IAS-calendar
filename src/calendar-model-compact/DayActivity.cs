using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Model.Compact.Validators;

namespace Calendar.Model.Compact
{
    public class DayActivity
    {
        private readonly List<string> errorMessages;

        public DayActivity(Activity first, Activity second, LocationPattern locationPattern, int day, List<string> errorMessages)
        {
            LocationPattern = locationPattern;
            Day = day;
            First = first;
            Second = second;
            this.errorMessages = errorMessages;
        }

        public Activity First { get; private set; }

        public Activity Second { get; private set; }

        public DayType DayType { get; set; }
        
        public EnterFromType EnterFrom { get; set; }

        public LocationPattern LocationPattern{get; internal set; }

        public int Day { get; private set; }

        public static DayActivity Empty(int day)
        {
            return new DayActivity(Activity.Empty(),Activity.Empty(),LocationPattern.Empty(),day, new List<string>());
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", First, Second);
        }

        public bool Equals(DayActivity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.First, First) && Equals(other.Second, Second) && Equals(other.LocationPattern, LocationPattern);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (DayActivity)) return false;
            return Equals((DayActivity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = First.GetHashCode();
                result = (result*397) ^ Second.GetHashCode();
                result = (result*397) ^ LocationPattern.GetHashCode();
                return result;
            }
        }

        public bool IsEmpty(Func<Note> getNote)
        {
            return First.Type == ActivityType.Empty &&
                Second.Type == ActivityType.Empty &&
                    LocationPattern.Equals(LocationPattern.Empty()) &&
                        getNote() == null;
        }

        public bool IsFirstLocationComplete()
        {
            return LocationPattern.IsFirstLocationComplete();
        }

        public bool IsSecondLocationComplete()
        {
            return LocationPattern.IsSecondLocationComplete();
        }

        public bool IsSecondActivityEmpty()
        {
            return Second.Type.Equals(ActivityType.Empty) ;
        }

        public bool IsFirstActivityEmpty()
        {
            return First.Type.Equals(ActivityType.Empty);
        }

        public bool IsThirdLocationComplete()
        {
            return LocationPattern.IsThirdLocationComplete();
        }

        public bool IsNoLocation()
        {
            return LocationPattern.IsNoLocation();
        }

        public bool HasRoundTravel()
        {
            return LocationPattern.HasRoundTravel();
        }

        public bool HasLocationAndActivity()
        {
            return !IsFirstActivityEmpty() && !IsSecondActivityEmpty() && !IsNoLocation();
        }

        private bool HasMissingTravel(DayActivity nextDay)
        {
            return nextDay != null && LocationPattern.NextDayFirstLocationNotEqualsLastLocation(nextDay.LocationPattern);
        }

        public bool IsComplete(Func<DayActivity, DayActivity> getNextDay)
        {
            return HasLocationAndActivity() && !HasMissingTravel(getNextDay(this));
        }

        public bool IsValid()
        {
            return  Validators.Where(v => v.Applicable()).All(r => r.IsValid());
        }

        public virtual List<IValidator> Validators
        {
            get
            {
                return new List<IValidator>
                         {
                             new SecondLocationValidator(LocationPattern, errorMessages),
                             new ThirdLocationValidator(LocationPattern, errorMessages),
                             new LocationValidator(LocationPattern.FirstLocation, errorMessages),
                             new LocationValidator(LocationPattern.SecondLocation, errorMessages),
                             new LocationValidator(LocationPattern.ThirdLocation, errorMessages),
                             new TimeValidator(LocationPattern.FirstLocationArrivalTime, errorMessages),
                             new TimeValidator(LocationPattern.FirstLocationDepartureTime, errorMessages),
                             new TimeValidator(LocationPattern.SecondLocationDepartureTime, errorMessages),
                             new TimeValidator(LocationPattern.SecondLocationArrivalTime, errorMessages),
                             new TimeValidator(LocationPattern.ThirdLocationArrivalTime, errorMessages),
                         };
            }
        }

    }
}