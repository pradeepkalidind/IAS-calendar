using System;
using System.Threading.Tasks;
using PWC.IAS.Calendar.Client;

namespace Calendar.API.Test.Common
{
    public class NoteApi
    {
        public async Task SaveNote(string content, IasCalendarResource iasCalendarResource, string noteDate, Guid userId)
        {
            var daysBodyParam = "properties[0].key=date&properties[0].value=" + noteDate;
            var contentParam = "content=" + content;
            await iasCalendarResource.RetrieveData("CalendarNote/Update", userId, daysBodyParam + "&" + contentParam, false);
        }

        public static async Task<string> GetNote(IasCalendarResource iasCalendarResource, Guid userId)
        {
            var note = await iasCalendarResource.RetrieveData("CalendarNote/Retrieve", userId, false);
            return RemoveId(note);
        }

        public static async Task DeleteNote(IasCalendarResource iasCalendarResource, string noteDate, Guid userId)
        {
            var dateParam = "date=" + noteDate;
            await iasCalendarResource.RetrieveData("CalendarNote/DeleteNote", userId, dateParam, false);
        }

        public async Task<int> GetNoteCount(IasCalendarResource iasCalendarResource, Guid userId)
        {
            return int.Parse(await iasCalendarResource.RetrieveData("CalendarNote/Count", userId, false));
        }

        private static string RemoveId(string note)
        {
            var startIndex = note.IndexOf("\"id\":\"", StringComparison.Ordinal);
            return startIndex < 0 ? note : note.Remove(startIndex, 44);
        }
    }
}