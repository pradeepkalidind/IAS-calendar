using System;

namespace Calendar.Model.Compact
{
    public class Activity
    {
        public Activity(ActivityType type, byte allocation)
        {
            Type = type;
            FirstLocationAllocation = allocation;
        }
        public static Activity Empty()
        {
            return new Activity(ActivityType.Empty,0);
        }
        public ActivityType Type { get; private set; }

        public byte FirstLocationAllocation { get; set; }

        public double GetFirstLocationAllocation()
        {
            return FirstLocationAllocation/100D;
        }

        public bool Equals(Activity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && SameAlloc(other);
        }

        private bool SameAlloc(Activity other)
        {
            return Type == ActivityType.Empty || Equals(other.FirstLocationAllocation, FirstLocationAllocation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Activity)) return false;
            return Equals((Activity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode()*397) ^ FirstLocationAllocation.GetHashCode();
            }
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}", Type, FirstLocationAllocation);
        }
 
        public double GetAllocationPercentage()
        {
            return FirstLocationAllocation/100D;
        }
    }
}