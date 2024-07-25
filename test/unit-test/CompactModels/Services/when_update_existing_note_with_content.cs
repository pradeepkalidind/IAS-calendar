using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Service.Controller;
using Calendar.Service.Requests;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.Services
{
    [XUnitCases]
    public class when_update_existing_note_with_content : DbSpec
    {
        private const string NEW_CONTENT = "New Content";
        private static ISessionWrapper session;
        private static readonly Guid UserId = Guid.NewGuid();
        private static CalendarNoteController controller;

        private static object GetNotesFromJsonResponse(IActionResult result)
        {
            return ((OkObjectResult)result).Value;
        }
        
        private static object GetValueFromObject(object instance, string propName)
        {
            return instance?.GetType().GetProperty(propName)?.GetValue(instance, null);
        }

        private void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(Note), typeof(UserActivity));
        }

        [Fact]
        public void test_when_update_existing_note_with_content()
        {
            cleanup_db();
            Given("context", () =>
            {
                session = DbHelper.GetSession();
                session.SaveOrUpdateAndFlush(new Note(UserId, new DateTime(2012, 12, 31), "A Note"));
                controller = new CalendarNoteController();
            });
            Thread.Sleep(1);
            var properties = new Dictionary<string, string> { { "date", "2012-12-31" } };
            var result = controller.Update(UserId.ToString(), new UpdateNoteRequest
            {
                Content = NEW_CONTENT,
                Properties = properties
            });
            session.Clear();
            Then("should_update_note_content", () => Assert.Equal(NEW_CONTENT, session.Query<Note>().First().Content));
            Then("should_create_user_activity", () => Assert.Equal(1, session.Query<UserActivity>().Count()));
            Then("should_return_correct_json", () => Assert.Equal(NEW_CONTENT, GetValueFromObject(GetNotesFromJsonResponse(result.Result), "content")));
        }
    }
}