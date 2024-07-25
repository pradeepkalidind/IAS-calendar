using System;
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
    public class when_delete_note : DbSpec
    {
        public static ISessionWrapper session;

        private static Guid userId = Guid.NewGuid();
        private static string result;

        private void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(Note), typeof(UserActivity));
        }

        public when_delete_note()
        {
            cleanup_db();
        }

        [Fact]
        public void test_when_delete_note()
        {
            session = DbHelper.GetSession();
            Given("context", () => { session.SaveOrUpdateAndFlush(new Note(userId, new DateTime(2012, 12, 31), "A Note")); });
            When("delete_note", () =>
            {
                var actionResult = new CalendarNoteController().DeleteNote(
                    userId.ToString(),
                    new DeleteNoteRequest { Date = "2012-12-31" }).Result as OkObjectResult;
                 result = actionResult?.Value?.ToString();
            });
            Then("should_delete_note_by_date_and_user", () => Assert.Equal(0, session.Query<Note>().Count()));
            Then("should_return_correct_result", () => Assert.Equal("success", result));
        }
    }
}