using System;

namespace Calendar.Model.Compact
{
    public class Note
    {
        public DateTime Date { get; private set; }
        public string Content { get; set; }

        public Note()
        {
            Id = Guid.NewGuid();
        }

        public Note(Guid userId,DateTime date,string content):this()
        {
            Date = date;
            Content = content;
            UserId = userId;
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
    }
}