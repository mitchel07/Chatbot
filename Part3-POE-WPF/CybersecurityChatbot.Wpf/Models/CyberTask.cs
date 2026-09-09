using System;

namespace CybersecurityChatbot.Wpf.Models
{
    /// <summary>
    /// A single cybersecurity task the user wants to remember to do,
    /// for example "Enable two-factor authentication". Stored in MySQL.
    /// </summary>
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // Optional reminder date. Null means no reminder was set.
        public DateTime? ReminderDate { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        // Display-only helpers used by the WPF task list (GridView bindings).
        public string ReminderText => ReminderDate.HasValue
            ? ReminderDate.Value.ToString("dd MMM yyyy")
            : "None";

        public string StatusText => IsCompleted ? "Completed" : "Pending";

        /// <summary>
        /// A short human-readable summary used in the task list and chat replies.
        /// </summary>
        public string Summary()
        {
            string status = IsCompleted ? "[Done] " : string.Empty;
            string reminder = ReminderDate.HasValue
                ? "  (Reminder: " + ReminderDate.Value.ToString("dd MMM yyyy") + ")"
                : "  (No reminder)";
            return status + Title + reminder;
        }
    }
}
