using System;
using System.Collections.Generic;
using System.Linq;
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
    public class when_update_note_with_empty_content : DbSpec
    {
        private static ISessionWrapper session;
        private static readonly Guid UserId = Guid.NewGuid();
        private static CalendarNoteController controller;

        private static object GetNotesFromJsonResponse(IActionResult result)
        {
            return ((OkObjectResult)result).Value;
        }

        private static void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(Note), typeof(UserActivity));
        }

        [Fact]
        public void test_when_update_note_with_empty_content()
        {
            cleanup_db();
            Given("context", () =>
            {
                session = DbHelper.GetSession();
                session.SaveOrUpdateAndFlush(new Note(UserId, new DateTime(2012, 12, 31), "A Note"));
                controller = new CalendarNoteController();
            });
            var properties = new Dictionary<string, string> { { "date", "2012-12-31" } };
            var emptyContent = String.Empty;
            var result = controller.Update(UserId.ToString(), new UpdateNoteRequest
            {
                Content = emptyContent,
                Properties = properties
            });
            session.Clear();
            Then("should_delete_note", () => Assert.Equal(0, session.Query<Note>().Count()));
            Then("should_get_corret_json_data", () => Assert.False(GetNotesFromJsonResponse(result.Result).GetType().GetProperty("creator") != null));
            Then("should_create_user_activity", () => Assert.Equal(1, session.Query<UserActivity>().Count()));
        }
    }
}