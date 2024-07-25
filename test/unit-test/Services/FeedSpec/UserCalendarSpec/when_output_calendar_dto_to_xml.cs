using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Calendar.Client.Schema;
using Calendar.General.Models;
using Calendar.Models;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.Services.FeedSpec.UserCalendarSpec
{
    [XUnitCases]
    public class when_output_calendar_dto_to_xml : DbSpec
    {
        private static CalendarRoot calendarRoot;
        private static StringBuilder sbr;

        [Fact]
        public void test_when_output_calendar_dto_to_xml()
        {
            Given("context", () =>
            {
                sbr = new StringBuilder();
                calendarRoot = new CalendarRoot
                {
                    Users = new List<UserCalendar>
                    {
                        new UserCalendar
                        {
                            IasPlatformUser = Guid.NewGuid().ToString(),
                            Days = new HashSet<Day>
                            {
                                new Day
                                {
                                    Activities = new List<Activity>
                                    {
                                        new Activity
                                        {
                                            FirstAllocation = null,
                                            SecondAllocation = null,
                                            Type = ActivityType.Vacation
                                        },
                                        new Activity
                                        {
                                            FirstAllocation = 0.3,
                                            SecondAllocation = 0.7,
                                            Type = ActivityType.Work
                                        },
                                    },
                                    ArrivalTime = "0500",
                                    DepartureTime = "0900",
                                    Date = "2011-10-05",
                                    Location = LocationValues.China,
                                    Note = new Note { Content = "note" },
                                    Travels = new List<Travel>
                                    {
                                        new Travel
                                        {
                                            Arrival = new Arrival
                                            {
                                                Date = "2011-10-05",
                                                Location = LocationValues.UnitedKingdom,
                                                Time = "1100"
                                            },
                                            Departure = new Departure
                                            {
                                                Date = "2011-10-05",
                                                Location = LocationValues.UnitedState,
                                                Time = null
                                            }
                                        },
                                    }
                                }
                            }
                        }
                    }
                };
            });
            When("of", () => calendarRoot.OutputXml(new StringWriter(sbr)));
            Then("should_succeed", () =>
            {
                var document = new XmlDocument();
                document.LoadXml(sbr.ToString());
                Assert.Equal(string.Empty, document.NamespaceURI);
                Console.Out.WriteLine(sbr.ToString());
            });
        }
    }
}