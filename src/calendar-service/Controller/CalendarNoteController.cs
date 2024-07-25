using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Service.Filters;
using Calendar.Service.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class CalendarNoteController : NewApiController
    {
        [HttpPost("CalendarNote/Retrieve")]
        public ActionResult<List<object>> Retrieve([FromQuery] string token)
        {
            var userId = GetUserId(token);
            var notes = session.Query<Note>().Where(n => n.UserId == userId).OrderBy(n => n.Date).ToList();
            return Ok(notes.Select(JsonSerializeCalendarNote).ToList());
        }

        [HttpPost("CalendarNote/Count")]
        public ActionResult Count([FromQuery] string token)
        {
            var userId = GetUserId(token);
            var notes = session.Query<Note>().Where(n => n.UserId == userId).ToList();
            return Ok(notes.Count);
        }

        [HttpPost("CalendarNote/RetrieveByYear")]
        public ActionResult RetrieveByYear([FromQuery] string token, [FromQuery] int year)
        {
            // did not find any usages in other services
            var userId = GetUserId(token);
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1).AddDays(-1);
            var notes = session.Query<Note>().Where(n => n.UserId == userId && n.Date >= startDate && n.Date <= endDate).ToList();
            return Ok(notes.Select(JsonSerializeCalendarNote).ToList());
        }

        [HttpPost("CalendarNote/Update")]
        public ActionResult<object> Update([FromQuery] string token, [FromForm] UpdateNoteRequest request)
        {
            var userId = GetUserId(token);
            var date = DateTime.Parse(request.Properties["date"]).Date;
            if (request.Content.Equals(string.Empty))
            {
                DeleteNote(userId, date);
                return Ok(JsonSerializeEmptyCalendarNote());
            }

            var note = session.Query<Note>().FirstOrDefault(n => n.UserId == userId && n.Date == date) ?? new Note(userId, date, request.Content);
            note.Content = request.Content;
            new Repository(session).SaveNote(userId, date, note);
            return Ok(JsonSerializeCalendarNote(note));
        }

        [HttpPost("CalendarNote/DeleteNote")]
        public ActionResult<string> DeleteNote([FromQuery] string token, [FromForm] DeleteNoteRequest request)
        {
            var userId = GetUserId(token);
            var noteDate = DateTime.Parse(request.Date);
            DeleteNote(userId, noteDate);

            return Ok("success");
        }

        private void DeleteNote(Guid userId, DateTime noteDate)
        {
            new Repository(session).DeleteNote(userId, noteDate);
        }

        private static Guid GetUserId(string userId)
        {
            return new Guid(userId);
        }

        private static object JsonSerializeCalendarNote(Note note)
        {
            return new { id = note.Id, content = note.Content, date = note.Date.ToString("yyyy-MM-dd") };
        }

        private static object JsonSerializeEmptyCalendarNote()
        {
            return new { id = string.Empty };
        }
    }
}
