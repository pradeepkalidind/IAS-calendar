using System;
using System.Collections.Generic;
using Calendar.Model.Compact;

namespace Calendar.Tests.Unit.CompactModels
{
    public class DayActivityBuilder
    {
        private ActivityType firstActivityType = ActivityType.Empty;
        private ActivityType secondActivityType = ActivityType.Empty;
        private byte firstActivityFirstLocationAllocation = 0;
        private byte secondActivityFirstLocationAllocation = 0;
        private int day =1;
        private LocationPattern locationPattern = LocationPattern.Empty();
        
        private LocationPatternBuilder locationPatternBuilder;

        public DayActivity Build()
        {
            return new DayActivity(new Activity(firstActivityType,firstActivityFirstLocationAllocation),
                new Activity(secondActivityType,secondActivityFirstLocationAllocation),
                locationPatternBuilder == null ? locationPattern : locationPatternBuilder.Build(),
                day, new List<string>());
        }

        public DayActivityBuilder FirstActivityType(ActivityType activityType)
        {
            firstActivityType = activityType;
            return this;
        }

        public DayActivityBuilder SecondActivityType(ActivityType activityType)
        {
            secondActivityType = activityType;
            return this;
        }
        public DayActivityBuilder Location(LocationPattern location)
        {
            locationPattern = location;
            return this;
        }

        public DayActivityBuilder Location(Action<LocationPatternBuilder> locationBuilder)
        {
            locationPatternBuilder = locationPatternBuilder??new LocationPatternBuilder();
            locationBuilder(locationPatternBuilder);
            return this;
        }

        public DayActivityBuilder SecondActivityFirstLocationAllocation(byte allocation)
        {
            secondActivityFirstLocationAllocation = allocation;
            return this;
        }
        public DayActivityBuilder FirstActivityFirstLocationAllocation(byte allocation)
        {
            firstActivityFirstLocationAllocation = allocation;
            return this;
        }
        public DayActivityBuilder Day(int day)
        {
            this.day = day;
            return this;
        }  
    }

    public class LocationPatternBuilder
    {
        private string firstLocation;
        private int? firstLocationArrivalTime;
        private int? firstLocationDepartureTime;
        private string secondLocation;
        private int? secondLocationArrivalTime;
        private int? secondLocationDepartureTime;
        private string thirdLocation;
        private int? thirdLocationArrivalTime;

        public LocationPattern Build()
        {
            return new LocationPattern(firstLocation,firstLocationArrivalTime,firstLocationDepartureTime,
                secondLocation,secondLocationArrivalTime,secondLocationDepartureTime,
                thirdLocation,thirdLocationArrivalTime);
        }

        public LocationPatternBuilder FirstLocation(string location)
        {
            firstLocation = location;
            return this;
        }
        public LocationPatternBuilder SecondLocation(string location)
        {
            secondLocation = location;
            return this;
        }
        public LocationPatternBuilder ThirdLocation(string location)
        {
            thirdLocation = location;
            return this;
        }

        public LocationPatternBuilder FirstLocationArrivalTime(int? time)
        {
            firstLocationArrivalTime = time;
            return this;
        }

        public LocationPatternBuilder FirstLocationDepartureTime(int? time)
        {
            firstLocationDepartureTime = time;
            return this;
        }

        public LocationPatternBuilder SecondLocationArrivalTime(int? time)
        {
            secondLocationArrivalTime = time;
            return this;
        }

        public LocationPatternBuilder SecondLocationDepartureTime(int? time)
        {
            secondLocationDepartureTime = time;
            return this;
        }

        public LocationPatternBuilder ThirdLocationArrivalTime(int? time)
        {
            thirdLocationArrivalTime = time;
            return this;
        }
        
    }
}