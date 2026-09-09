using System.Collections.Generic;
using System.Linq;
using CybersecurityChatbot.Wpf.Models;

namespace CybersecurityChatbot.Wpf.Services
{
    /// <summary>
    /// Keeps an in-memory log of significant actions the chatbot has taken.
    /// The user can ask to see it ("show activity log" / "what have you done for me").
    /// </summary>
    public class ActivityLogService
    {
        private readonly List<ActivityLogEntry> _entries = new List<ActivityLogEntry>();

        public void Add(string description)
        {
            _entries.Add(new ActivityLogEntry(description));
        }

        /// <summary>
        /// Returns the most recent entries (newest last), capped to keep the log concise.
        /// </summary>
        public List<ActivityLogEntry> GetRecent(int count = 10)
        {
            int skip = _entries.Count > count ? _entries.Count - count : 0;
            return _entries.Skip(skip).ToList();
        }

        public List<ActivityLogEntry> GetAll()
        {
            return _entries.ToList();
        }

        public int Count => _entries.Count;
    }
}
