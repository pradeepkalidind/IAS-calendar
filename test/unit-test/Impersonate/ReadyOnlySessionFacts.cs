using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Tests.Unit.CompactModels;
using Xunit;

namespace Calendar.Tests.Unit.Impersonate
{
    [XUnitCases]
    public class ReadyOnlySessionFacts : DbSpec
    {
        private static readonly Guid UserId = new Guid("C42D18E1-8B1F-43BD-8DCC-B75055F49AF1");

        private static void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(Note));
        }

        [Fact]
        public void test_ReadyOnlySessionFacts()
        {
            cleanup_db();

            Given("context", () =>
            {
                var session = DbHelper.GetSession();
                var note = new Note(UserId, new DateTime(2012, 1, 1), "note");
                session.Save(note);
            });
            Then("should_retrive", () =>
            {
                var sessionWrapper = new ReadyOnlySession(DbHelper.GetSession());
                Assert.Equal(1, sessionWrapper.Query<Note>().Count());
            });
            Then("should_not_save_new_object", () =>
            {
                var sessionWrapper = new ReadyOnlySession(DbHelper.GetSession());
                sessionWrapper.Save(new Note(UserId, new DateTime(2012, 1, 2), "note"));
                Assert.Equal(1, DbHelper.GetSession().Query<Note>().Count());
                sessionWrapper.SaveOrUpdate(new Note(UserId, new DateTime(2012, 1, 2), "note"));
                Assert.Equal(1, DbHelper.GetSession().Query<Note>().Count());
            });
            Then("should_not_save_exist_object", () =>
            {
                var sessionWrapper = new ReadyOnlySession(DbHelper.GetSession());
                var note = sessionWrapper.Query<Note>().First();
                note.Content = "new note";
                sessionWrapper.Save(note);
                Assert.Equal("note", DbHelper.GetSession().Query<Note>().First().Content);
                sessionWrapper.SaveOrUpdate(note);
                Assert.Equal("note", DbHelper.GetSession().Query<Note>().First().Content);
                sessionWrapper.Update(note);
                Assert.Equal("note", DbHelper.GetSession().Query<Note>().First().Content);
            });
            Then("should_not_delete_exist_object", () =>
            {
                var sessionWrapper = new ReadyOnlySession(DbHelper.GetSession());
                var note = sessionWrapper.Query<Note>().First();
                note.Content = "new note";
                sessionWrapper.Delete(note);
                Assert.Equal(1, DbHelper.GetSession().Query<Note>().Count());
            });
        }
    }
}