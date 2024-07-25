using System.Collections.Generic;

namespace Calendar.Service.Requests
{
    public class UpdateNoteRequest
    {
        public Dictionary<string, string> Properties { get; set; }
        public string Content { get; set; }
    }
}