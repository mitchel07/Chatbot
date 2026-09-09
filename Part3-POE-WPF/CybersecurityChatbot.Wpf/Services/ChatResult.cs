using System;

namespace CybersecurityChatbot.Wpf.Services
{
    /// <summary>
    /// What the chatbot returns for a single user message: the text reply plus
    /// any action the GUI should carry out, with values extracted from the input.
    /// </summary>
    public class ChatResult
    {
        public string Reply { get; set; }
        public BotAction Action { get; set; } = BotAction.None;

        // Filled in for AddTask.
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }

        // Filled in for SetReminderForLastTask (or AddTask with an inline reminder).
        public DateTime? ReminderDate { get; set; }

        public ChatResult(string reply)
        {
            Reply = reply;
        }
    }
}
