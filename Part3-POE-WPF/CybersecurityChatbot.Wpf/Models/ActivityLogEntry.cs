using System;

namespace CybersecurityChatbot.Wpf.Models
{
    /// <summary>
    /// One recorded action the chatbot has taken (task added, reminder set,
    /// quiz started, NLP command recognised, and so on). Used by the activity log.
    /// </summary>
    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }

        public ActivityLogEntry(string description)
        {
            Timestamp = DateTime.Now;
            Description = description;
        }

        public override string ToString()
        {
            return Timestamp.ToString("dd MMM HH:mm") + "  -  " + Description;
        }
    }
}
