using System;

namespace SignalRChat.Models
{
    public class Message
    {
        public string User { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public Message(string user, string content)
        {
            User = user;
            Content = content;
            Timestamp = DateTime.Now;
        }
    }
}