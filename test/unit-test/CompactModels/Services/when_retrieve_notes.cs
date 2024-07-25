using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Service.Controller;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.Services
{
    [XUnitCases]
    public class when_retrieve_notes : DbSpec
    {
        private static ISessionWrapper session;
        private static readonly Guid UserId = Guid.NewGuid();
        private static IList<object> notes;

        private static IList<object> GetNotesFromJsonResponse(IActionResult result)
        {
            return ((OkObjectResult)result)?.Value as List<object>;
        }

        private static object GetValueFromObject(object instance, string propName)
        {
            return instance?.GetType().GetProperty(propName)?.GetValue(instance, null);
        }

        private static void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(Note));
        }

        [Fact]
        public void test_when_retrieve_notes()
        {
            cleanup_db();
            Given("context", () =>
            {
                session = DbHelper.GetSession();
                session.SaveOrUpdateAndFlush(new Note(UserId, new DateTime(2011, 6, 9), "A Note"));
                session.SaveOrUpdateAndFlush(new Note(UserId, new DateTime(2012, 1, 1), "B Note"));
                session.SaveOrUpdateAndFlush(new Note(UserId, new DateTime(2012, 12, 31), "C Note"));
                session.SaveOrUpdateAndFlush(new Note(UserId, new DateTime(2013, 1, 1), "D Note"));
            });
            When("retrieve_notes", () =>
            {
                var result = new CalendarNoteController().Retrieve(UserId.ToString());
                notes = GetNotesFromJsonResponse(result.Result);
            });
            Then("should_retrieve_all_notes_by_user", () =>
            {
                Assert.Equal(4, notes.Count);
                var firstNote = notes.FirstOrDefault();
                Assert.Equal("A Note", GetValueFromObject(firstNote,"content"));
                Assert.Equal("2011-06-09", GetValueFromObject(firstNote, "date"));
            });
            Then("should_count_all_notes_by_user", () =>
            {
                var result = new CalendarNoteController().Count(UserId.ToString());
                var count = ((OkObjectResult)result).Value;
                Assert.Equal(4, count);
            });
            
            Then("should_retrieve_notes_by_year_and_user", () =>
            {
                var result = new CalendarNoteController().RetrieveByYear(UserId.ToString(), 2012);
                notes = GetNotesFromJsonResponse(result);
                var dates = new[]
                {
                    GetValueFromObject(notes.ElementAt(0), "date"),
                    GetValueFromObject(notes.ElementAt(1), "date"),
                };
                Assert.Equal(2, notes.Count);
                Assert.Contains("2012-01-01", dates);
                Assert.Contains("2012-12-31", dates);
            });
        }
    }
}