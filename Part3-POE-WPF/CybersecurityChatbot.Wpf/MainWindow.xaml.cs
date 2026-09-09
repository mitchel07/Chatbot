using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CybersecurityChatbot.Wpf.Models;
using CybersecurityChatbot.Wpf.Services;

namespace CybersecurityChatbot.Wpf
{
    /// <summary>
    /// The single window that hosts every feature: chat, tasks, quiz and the
    /// activity log. The window is intentionally thin: it collects user input
    /// and displays results, while the services hold the real logic.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _db = new DatabaseService();
        private TaskService _taskService;
        private readonly ActivityLogService _activityLog = new ActivityLogService();
        private readonly QuizService _quiz = new QuizService();
        private readonly VoiceService _voice = new VoiceService();
        private ChatbotEngine _engine;

        // When MySQL is not reachable, tasks are kept in memory so the app still
        // runs for demonstration. The database remains the primary store.
        private bool _dbAvailable;
        private readonly List<CyberTask> _memoryTasks = new List<CyberTask>();
        private int _memoryIdCounter = 1;

        // Conversation state.
        private bool _awaitingName = true;
        private int _lastTaskId = -1;

        // Quiz UI state.
        private bool _quizAwaitingAnswer;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LogoText.Text = AsciiArt.Logo;

            // Carry the Part 1 voice greeting into the GUI.
            _voice.PlayGreeting();

            // Connect to MySQL and make sure the table exists.
            _dbAvailable = _db.EnsureSchema(out string dbError);
            _taskService = new TaskService(_db);
            DbStatusText.Text = _dbAvailable
                ? "Connected to MySQL database (cybersecurity_bot)."
                : "MySQL not connected, tasks are stored in memory for this session. (" + dbError + ")";

            _engine = new ChatbotEngine("there");

            AddBotBubble("Hello! Welcome to the Cybersecurity Awareness Bot. I am here to help you stay safe online.");
            AddBotBubble("Before we begin, what is your name?");

            RefreshTaskList();
            RefreshActivityLog();
        }

        // =====================================================================
        // CHAT
        // =====================================================================
        private void ChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HandleUserMessage();
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            HandleUserMessage();
        }

        private void HandleUserMessage()
        {
            string text = ChatInput.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            AddUserBubble(text);
            ChatInput.Clear();

            // First message captures the user's name.
            if (_awaitingName)
            {
                string name = text.Trim();
                name = char.ToUpperInvariant(name[0]) + name.Substring(1);
                _engine.UserName = name;
                _awaitingName = false;
                GreetingText.Text = "Chatting with " + name + ". Ask about passwords, phishing, scams or privacy.";
                AddBotBubble("Nice to meet you, " + name + "! You can ask me a cybersecurity question, " +
                             "or try commands like \"start quiz\", \"add a task to enable 2FA\", or \"show activity log\".");
                return;
            }

            // Hand the message to the engine and act on the result.
            ChatResult result = _engine.Process(text);
            AddBotBubble(result.Reply);
            ExecuteAction(result);
        }

        private void ExecuteAction(ChatResult result)
        {
            switch (result.Action)
            {
                case BotAction.AddTask:
                    AddTaskFromChat(result);
                    break;

                case BotAction.SetReminderForLastTask:
                    SetReminderFromChat(result);
                    break;

                case BotAction.ShowTasks:
                    ShowTasksInChat();
                    break;

                case BotAction.StartQuiz:
                    ((TabControl)((Grid)Content).Children[1]).SelectedIndex = 2;
                    StartQuiz_Click(null, null);
                    break;

                case BotAction.ShowActivityLog:
                    ShowLogInChat();
                    break;
            }
        }

        private void AddTaskFromChat(ChatResult result)
        {
            var task = new CyberTask
            {
                Title = result.TaskTitle,
                Description = result.TaskDescription,
                ReminderDate = result.ReminderDate
            };
            _lastTaskId = StoreTask(task);
            _activityLog.Add("Task added: \"" + task.Title + "\"" +
                (task.ReminderDate.HasValue ? " (reminder " + task.ReminderDate.Value.ToString("dd MMM yyyy") + ")" : " (no reminder)"));
            RefreshTaskList();
            RefreshActivityLog();

            if (result.ReminderDate.HasValue)
            {
                AddBotBubble("I also set a reminder for " + result.ReminderDate.Value.ToString("dd MMM yyyy") + ".");
            }
        }

        private void SetReminderFromChat(ChatResult result)
        {
            if (_lastTaskId < 0)
            {
                AddBotBubble("I do not have a recent task to attach that reminder to. Try adding a task first.");
                return;
            }
            DateTime when = result.ReminderDate ?? DateTime.Now.AddDays(7);
            SetReminder(_lastTaskId, when);
            _activityLog.Add("Reminder set for " + when.ToString("dd MMM yyyy") + " on the most recent task.");
            RefreshTaskList();
            RefreshActivityLog();
        }

        private void ShowTasksInChat()
        {
            List<CyberTask> tasks = GetTasks();
            if (tasks.Count == 0)
            {
                AddBotBubble("You have no tasks yet. Say \"add a task to enable two-factor authentication\" to start.");
                return;
            }
            int i = 1;
            foreach (CyberTask t in tasks)
            {
                AddBotBubble(i + ". " + t.Summary());
                i++;
            }
        }

        private void ShowLogInChat()
        {
            List<ActivityLogEntry> entries = _activityLog.GetRecent();
            if (entries.Count == 0)
            {
                AddBotBubble("No actions have been recorded yet.");
                return;
            }
            int i = 1;
            foreach (ActivityLogEntry entry in entries)
            {
                AddBotBubble(i + ". " + entry.ToString());
                i++;
            }
        }

        // =====================================================================
        // CHAT BUBBLES
        // =====================================================================
        private void AddUserBubble(string text)
        {
            AddBubble(text, HorizontalAlignment.Right, Color.FromRgb(0x1E, 0x88, 0xE5), Colors.White);
        }

        private void AddBotBubble(string text)
        {
            AddBubble(text, HorizontalAlignment.Left, Color.FromRgb(0x1B, 0x2A, 0x39), Color.FromRgb(0xE6, 0xF1, 0xFA));
        }

        private void AddBubble(string text, HorizontalAlignment align, Color background, Color foreground)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(background),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(0, 4, 0, 4),
                HorizontalAlignment = align,
                MaxWidth = 620
            };
            border.Child = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(foreground),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };
            ChatPanel.Children.Add(border);
            ChatScroll.ScrollToEnd();
        }

        // =====================================================================
        // TASK STORAGE (database first, memory fallback)
        // =====================================================================
        private int StoreTask(CyberTask task)
        {
            task.CreatedAt = DateTime.Now;
            if (_dbAvailable)
            {
                try
                {
                    return _taskService.Add(task);
                }
                catch (Exception ex)
                {
                    DbStatusText.Text = "Database error, switched to memory storage: " + ex.Message;
                    _dbAvailable = false;
                }
            }
            task.Id = _memoryIdCounter++;
            _memoryTasks.Add(task);
            return task.Id;
        }

        private List<CyberTask> GetTasks()
        {
            if (_dbAvailable)
            {
                try
                {
                    return _taskService.GetAll();
                }
                catch
                {
                    _dbAvailable = false;
                }
            }
            return _memoryTasks.ToList();
        }

        private void SetReminder(int id, DateTime when)
        {
            if (_dbAvailable)
            {
                try { _taskService.SetReminder(id, when); return; }
                catch { _dbAvailable = false; }
            }
            CyberTask t = _memoryTasks.FirstOrDefault(x => x.Id == id);
            if (t != null) t.ReminderDate = when;
        }

        private void CompleteTaskInStore(int id)
        {
            if (_dbAvailable)
            {
                try { _taskService.MarkCompleted(id); return; }
                catch { _dbAvailable = false; }
            }
            CyberTask t = _memoryTasks.FirstOrDefault(x => x.Id == id);
            if (t != null) t.IsCompleted = true;
        }

        private void DeleteTaskInStore(int id)
        {
            if (_dbAvailable)
            {
                try { _taskService.Delete(id); return; }
                catch { _dbAvailable = false; }
            }
            _memoryTasks.RemoveAll(x => x.Id == id);
        }

        // =====================================================================
        // TASKS TAB
        // =====================================================================
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.", "Missing title",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var task = new CyberTask
            {
                Title = title,
                Description = TaskDescBox.Text.Trim(),
                ReminderDate = TaskReminderPicker.SelectedDate
            };
            _lastTaskId = StoreTask(task);
            _activityLog.Add("Task added: \"" + task.Title + "\"" +
                (task.ReminderDate.HasValue ? " (reminder " + task.ReminderDate.Value.ToString("dd MMM yyyy") + ")" : " (no reminder)"));

            TaskTitleBox.Clear();
            TaskDescBox.Clear();
            TaskReminderPicker.SelectedDate = null;

            RefreshTaskList();
            RefreshActivityLog();
            AddBotBubble("Task added: \"" + task.Title + "\".");
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            CyberTask selected = TaskListView.SelectedItem as CyberTask;
            if (selected == null)
            {
                MessageBox.Show("Select a task first.", "No task selected",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            CompleteTaskInStore(selected.Id);
            _activityLog.Add("Task marked completed: \"" + selected.Title + "\"");
            RefreshTaskList();
            RefreshActivityLog();
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            CyberTask selected = TaskListView.SelectedItem as CyberTask;
            if (selected == null)
            {
                MessageBox.Show("Select a task first.", "No task selected",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DeleteTaskInStore(selected.Id);
            _activityLog.Add("Task deleted: \"" + selected.Title + "\"");
            RefreshTaskList();
            RefreshActivityLog();
        }

        private void RefreshTasks_Click(object sender, RoutedEventArgs e)
        {
            RefreshTaskList();
        }

        private void RefreshTaskList()
        {
            TaskListView.ItemsSource = null;
            TaskListView.ItemsSource = GetTasks();
        }

        // =====================================================================
        // QUIZ TAB
        // =====================================================================
        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            _quiz.Reset();
            _activityLog.Add("Quiz started.");
            RefreshActivityLog();
            QuizFeedbackText.Text = string.Empty;
            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {
            QuizOptionsPanel.Children.Clear();

            if (_quiz.IsFinished)
            {
                QuizQuestionText.Text = "Quiz complete!";
                QuizProgressText.Text = string.Empty;
                QuizFeedbackText.Text = "Final score: " + _quiz.Score + " out of " + _quiz.Total +
                    ".  " + _quiz.ResultMessage();
                QuizScoreText.Text = "Score: " + _quiz.Score + "/" + _quiz.Total;
                QuizNextButton.IsEnabled = false;
                QuizStartButton.Content = "Play Again";
                _quizAwaitingAnswer = false;
                _activityLog.Add("Quiz completed. Score " + _quiz.Score + "/" + _quiz.Total + ".");
                RefreshActivityLog();
                AddBotBubble("You scored " + _quiz.Score + "/" + _quiz.Total + " on the quiz. " + _quiz.ResultMessage());
                return;
            }

            QuizQuestion q = _quiz.Current();
            QuizProgressText.Text = "Question " + _quiz.CurrentNumber + " of " + _quiz.Total;
            QuizScoreText.Text = "Score: " + _quiz.Score + "/" + _quiz.Total;
            QuizQuestionText.Text = q.Text;
            QuizStartButton.Content = "Restart Quiz";

            for (int i = 0; i < q.Options.Count; i++)
            {
                var rb = new RadioButton
                {
                    Content = q.Options[i],
                    Tag = i,
                    GroupName = "quiz",
                    Foreground = Brushes.White,
                    FontSize = 15,
                    Margin = new Thickness(0, 6, 0, 6)
                };
                QuizOptionsPanel.Children.Add(rb);
            }

            _quizAwaitingAnswer = true;
            QuizNextButton.IsEnabled = true;
            QuizFeedbackText.Text = string.Empty;
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (!_quizAwaitingAnswer)
            {
                ShowCurrentQuestion();
                return;
            }

            int chosen = -1;
            foreach (UIElement child in QuizOptionsPanel.Children)
            {
                if (child is RadioButton rb && rb.IsChecked == true)
                {
                    chosen = (int)rb.Tag;
                    break;
                }
            }

            if (chosen < 0)
            {
                MessageBox.Show("Please choose an answer.", "No answer selected",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _quiz.Answer(chosen, out string feedback);
            QuizFeedbackText.Text = feedback;
            QuizScoreText.Text = "Score: " + _quiz.Score + "/" + _quiz.Total;
            _quizAwaitingAnswer = false;

            // Disable options until the user moves on.
            foreach (UIElement child in QuizOptionsPanel.Children)
            {
                if (child is RadioButton rb)
                {
                    rb.IsEnabled = false;
                }
            }

            // Brief pause is not needed; user clicks Next again to continue.
            QuizNextButton.Content = _quiz.IsFinished ? "See Result" : "Continue";
        }

        // =====================================================================
        // ACTIVITY LOG TAB
        // =====================================================================
        private void RefreshLog_Click(object sender, RoutedEventArgs e)
        {
            RefreshActivityLog();
        }

        private void RefreshActivityLog()
        {
            ActivityLogList.ItemsSource = null;
            ActivityLogList.ItemsSource = _activityLog.GetRecent()
                .Select((entry, index) => (index + 1) + ".  " + entry.ToString())
                .ToList();
        }
    }
}
