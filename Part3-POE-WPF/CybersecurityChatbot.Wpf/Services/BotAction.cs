namespace CybersecurityChatbot.Wpf.Services
{
    /// <summary>
    /// An action the GUI may need to perform as a result of understanding the
    /// user's message (the NLP simulation routes the conversation to a feature).
    /// </summary>
    public enum BotAction
    {
        None,
        AddTask,
        SetReminderForLastTask,
        ShowTasks,
        StartQuiz,
        ShowActivityLog
    }
}
