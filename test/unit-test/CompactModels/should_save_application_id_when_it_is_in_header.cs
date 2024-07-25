using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Dto;
using Calendar.Model.Compact;
using Calendar.Service.Controller;
using Calendar.Service.Requests;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels
{
    [XUnitCases]
    public class should_save_application_id : CompactModelSpec
    {
        private static ChangedDaysDto GetUpdateDays()
        {
            return new ChangedDaysDto
            {
                deletedDays = new List<DeletedDayDto>(),
                updatedDays = new List<DayDto>
                {
                    new DayDto { firstActivity = "/ActivityType/Work", date = "2012/01/01" },
                    new DayDto { firstActivity = "/ActivityType/Work", date = "2012/01/02" }
                }
            };
        }

        [Fact]
        public void when_application_id_is_in_header()
        {
            var daysController = default(DaysController);

            var userId = default(Guid);
            var updateDays = default(ChangedDaysDto);
            Given("context", () =>
            {
                daysController = new DaysController
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new HttpContextMock().SetupUrl("http://localhost")
                            .SetupRequestHeaders(new Dictionary<string, StringValues>()
                            {
                                { "Application_Id", new StringValues("mobile_sync") }
                            })
                    }
                };

                userId = Guid.NewGuid();
                updateDays = GetUpdateDays();
            });
            When("save_data_by_user", () =>
            {
                var jsonData = JsonConvert.SerializeObject(updateDays);
                daysController.SaveByUser(userId.ToString(), new SaveDaysRequest
                {
                    Params = jsonData
                });
            });
            Then("should_get_user_activity", () =>
            {
                var userActivities = Session.Query<UserActivity>().Where(user => user.UserId == userId).ToList();
                Assert.Equal(2, userActivities.Count);
                userActivities.ForEach(user => Assert.Equal("mobile_sync", user.ApplicationId));
            });
        }

        [Fact]
        public void when_application_id_is_not_in_header()
        {
            var daysController = default(DaysController);
            var userId = default(Guid);
            var updateDays = default(ChangedDaysDto);
            
            Given("context", () =>
            {
                daysController = new DaysController
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new HttpContextMock().SetupUrl("http://localhost")
                    }
                };
                userId = Guid.NewGuid();
                updateDays = GetUpdateDays();
            });
            When("save_data_by_user", () =>
            {
                var jsonData = JsonConvert.SerializeObject(updateDays);
                daysController.SaveByUser(userId.ToString(), new SaveDaysRequest
                {
                    Params = jsonData
                });
            });
            Then("should_get_user_activity", () =>
            {
                var userActivities = Session.Query<UserActivity>().Where(user => user.UserId == userId).ToList();
                Assert.Equal(2, userActivities.Count);
                userActivities.ForEach(user => Assert.Null(user.ApplicationId));
            });
        }
    }
}