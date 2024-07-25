using System;

namespace Calendar.Model.Compact
{
    public class ActivityAllocationRule
    {
        public DayActivity Day { get; private set; }

        public ActivityAllocationRule(DayActivity day)
        {
            Day = day;
        }

        public virtual bool Applicable()
        {
            return true;
        }

        public byte DefaultFirstActivityAllocation
        {
            get
            {
                if (IsFirstActivityFirstLocationAllocationEmpty())
                {
                    return 0;
                }
                if (Day.HasRoundTravel())
                {
                    return 0;
                }
                return AverageActivityValue > 100 ? (byte)100 : (byte)AverageActivityValue;
            }
        }

        public byte DefaultSecondActivityAllocation
        {
            get
            {
                if (IsSecondActivityFirstLocationAllocationEmpty())
                {
                    return 0;
                }
               if (Day.HasRoundTravel())
                {
                    return 0;
                }

                return Day.IsSecondLocationComplete() ? (byte)(100 - AverageActivityValue) : (byte)100;
            }
        }

        public bool IsSecondActivityFirstLocationAllocationEmpty()
        {
            return Day.IsSecondActivityEmpty() || Day.IsNoLocation();
        }

        private int AverageActivityValue { get { return (FirstActivityValue + SecondActivityValue) / LocationValue; } }

        private int LocationValue { get { return (Day.IsFirstLocationComplete() ? 1 : 0) + (Day.IsSecondLocationComplete() ? 1 : 0); } }

        private int FirstActivityValue { get { return (Day.IsFirstActivityEmpty() ? 0 : 100); } }

        private int SecondActivityValue { get { return (Day.IsSecondActivityEmpty() ? 0 : 100); } }

        public bool IsFirstActivityFirstLocationAllocationEmpty()
        {
            return Day.IsFirstActivityEmpty() || Day.IsNoLocation();
        }

        public double GetFirstActivityFirstLocationAllocation()
        {
            return Day.First.FirstLocationAllocation / 100D;
        }
        
        public double GetSecondActivityFirstLocationAllocation()
        {
            return Day.Second.FirstLocationAllocation / 100D;
        }

        public double GetFirstActivitySecondLocationAllocation()
        {
            return (double)(decimal.One - new decimal(GetFirstActivityFirstLocationAllocation()));
        }

        public double GetSecondActivitySecondLocationAllocation()
        {
            return (double)(decimal.One - new decimal(GetSecondActivityFirstLocationAllocation()));
        }
    }
}