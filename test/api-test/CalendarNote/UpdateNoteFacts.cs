using System;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.API.Test.CalendarNote
{
    public class UpdateNoteFacts : ApiFactBase
    {
        private readonly Guid userId = Guid.NewGuid();

        [Fact]
        public async Task should_update_and_delete_note()
        {
            const string date = "2011/01/01";
            const string content = "note";
            await Update("new", date, content);
            Assert.Equal("[{\"content\":\"note\",\"date\":\"2011-01-01\"}]", await GetNote("area"));
            Assert.Equal(1, await GetNoteCount("new"));
            await Delete("new", date);
            Assert.Equal("[]", await GetNote("area"));
            Assert.Equal(0, await GetNoteCount("new"));
        }

        private async Task Delete(string area, string date)
        {
            var dateParam = "date=" + date;
            await GetClient(area).RetrieveData("CalendarNote/DeleteNote", userId, dateParam, false);
        }

        private async Task<string> GetNote(string area)
        {
            var note = await GetClient(area).RetrieveData("CalendarNote/Retrieve", userId, false);
            return RemoveId(note);
        }

        private static string RemoveId(string note)
        {
            var startIndex = note.IndexOf("\"id\":\"", StringComparison.Ordinal);
            return startIndex < 0 ? note : note.Remove(startIndex, 44);
        }

        private async Task<int> GetNoteCount(string area)
        {
            return int.Parse(await GetClient(area).RetrieveData("CalendarNote/Count", userId, false));
        }

        private async Task Update(string area, string date, string content)
        {
            var daysBodyParam = "properties[0].key=date&properties[0].value=" + date;
            var contentParam = "content=" + content;
            await GetClient(area).RetrieveData("CalendarNote/Update", userId, daysBodyParam + "&" + contentParam, false);
        }
    }
}