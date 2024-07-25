using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Persistence;
using NHibernate;
using NHibernate.Type;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels
{
    [XUnitCases]
    public class save_and_retrive_note : DbSpec
    {
        private static ISessionWrapper session;

        private static readonly Guid UserId = new Guid("C42D18E1-8B1F-43BD-8DCC-B75055F49AF1");

        private static void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(Note), typeof(UserActivity));
        }

        [Fact]
        public void test_save_and_retrive_note()
        {
            cleanup_db();
            Given("context", () =>
            {
                session = DbHelper.GetSession();
                var note = new Note(UserId, new DateTime(2012, 1, 1), "note");
                session.Save(note);
            });
            Then("should_retrive", () =>
            {
                var notes = session.Query<Note>();
                Assert.True(notes.Any());
                var note = notes.First();
                Assert.Equal("note", note.Content);
            });
            Then("should_delete", () =>
            {
                session.Delete("from Note where Userid=? and date=?",
                    new object[] { UserId, new DateTime(2012, 1, 1) },
                    new IType[] { NHibernateUtil.Guid, NHibernateUtil.DateTime });
                Assert.Equal(0, session.Query<Note>().Count());
            });
        }
    }
}