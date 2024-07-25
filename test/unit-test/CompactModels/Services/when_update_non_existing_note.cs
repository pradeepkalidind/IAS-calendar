using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Service.Controller;
using Calendar.Service.Requests;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.Services
{
    [XUnitCases]
    public class when_update_non_existing_note : DbSpec
    {
        private const string NEW_CONTENT = "New Content";
        private static ISessionWrapper session;

        private static object GetNotesFromJsonResponse(IActionResult result)
        {
            return ((OkObjectResult)result).Value;
        }

        private static object GetValueFromObject(object instance, string propName)
        {
            return instance?.GetType().GetProperty(propName)?.GetValue(instance, null);
        }

        private static Guid userId = Guid.NewGuid();
        private static CalendarNoteController controller;

        private static void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(Note), typeof(UserActivity));
        }

        [Fact]
        public void test_when_update_non_existing_note()
        {
            cleanup_db();
            session = DbHelper.GetSession();
            controller = new CalendarNoteController();
            var properties = new Dictionary<string, string> { { "date", "2012-12-31" } };
            var result = controller.Update(userId.ToString(), new UpdateNoteRequest
            {
                Content = NEW_CONTENT,
                Properties = properties
            });
            Then("should_create_note_if_note_is_not_exist", () => Assert.Equal(NEW_CONTENT, session.Query<Note>().First().Content));
            Then("should_return_correct_json", () => Assert.Equal(NEW_CONTENT, GetValueFromObject(GetNotesFromJsonResponse(result.Result), "content")));
        }
    }
}