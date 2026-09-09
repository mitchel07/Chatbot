# Cybersecurity Awareness Bot - Part 3 / POE (WPF GUI Application)

Author: Mitchel

The final, GUI-based version of the Cybersecurity Awareness Chatbot, built with
WPF (.NET 8). It carries forward every feature from Part 1 and Part 2 and adds
the Part 3 features: a task assistant backed by a MySQL database, a quiz
mini-game, an NLP-style command system, and an activity log.

> Note: This part is a GUI application. It does not run as a console program.

## Features

Carried over from Parts 1 and 2:
- Voice greeting (WAV) played when the window loads.
- ASCII art logo shown in the header.
- Friendly, name-personalised conversation.
- Keyword recognition (password, phishing, scam, privacy, safe browsing, 2FA).
- Random responses chosen from lists so tips stay varied.
- Conversation flow ("tell me more", "another tip") that stays on topic.
- Memory and recall of the user's name and favourite topic.
- Sentiment detection (worried, frustrated, curious) with supportive replies
  that still give a useful tip in the same turn.
- Graceful default response for unrecognised input.

New in Part 3:
- Task Assistant (Task 1): add cybersecurity tasks with title, description and an
  optional reminder, stored in a MySQL database. View, complete and delete tasks.
- Quiz Mini-Game (Task 2): more than ten questions mixing multiple-choice and
  true/false, shown one at a time, with immediate feedback, explanations and a
  final score with encouragement.
- NLP Simulation (Task 3): recognises intents worded in different ways, such as
  "add a task to enable 2FA" or "remind me to update my password tomorrow",
  using string matching and regular expressions.
- Activity Log (Task 4): records tasks added, reminders set, quiz activity and
  recognised commands, and shows the most recent actions on request
  ("show activity log" or "what have you done for me").

## Project structure

```
Part3-POE-WPF/
  CybersecurityChatbot.Wpf/
    App.xaml / App.xaml.cs
    MainWindow.xaml / MainWindow.xaml.cs    GUI and event wiring
    Models/
      CyberTask.cs
      QuizQuestion.cs
      ActivityLogEntry.cs
    Services/
      ChatbotEngine.cs        Keywords, random tips, sentiment, memory, NLP
      TaskService.cs          MySQL task CRUD
      DatabaseService.cs      MySQL connection + schema
      QuizService.cs          Quiz questions and scoring
      ActivityLogService.cs   In-memory action log
      VoiceService.cs         Plays the WAV greeting
      AsciiArt.cs             Logo for the header
      ChatResult.cs / BotAction.cs
    Assets/
      greeting.wav            Your recorded greeting (add this yourself)
  Database/
    schema.sql                MySQL database and table
  .github/workflows/dotnet.yml CI workflow (GitHub Actions)
```

## Prerequisites

- .NET 8 SDK (Windows, because WPF is Windows only).
- MySQL Server running locally (for the task assistant).
- The MySqlConnector NuGet package (restored automatically on build).

## Setup

1. Create the database by running `Database/schema.sql` in MySQL, for example:

   ```
   mysql -u root -p < Database/schema.sql
   ```

2. Update the connection string if your MySQL user or password differ. Edit
   `Services/DatabaseService.cs`:

   ```
   Server=localhost;Port=3306;Database=cybersecurity_bot;User ID=root;Password=password;
   ```

3. Add your recorded greeting at `CybersecurityChatbot.Wpf/Assets/greeting.wav`.

## How to run

From the `Part3-POE-WPF/CybersecurityChatbot.Wpf` folder:

```
dotnet run
```

If MySQL is not available the app still runs and stores tasks in memory for the
session, showing a status note in the header. The database is the primary store.

## Try these commands in the chat

- "Tell me about password safety"
- "Give me a phishing tip" then "another tip"
- "I am worried about online scams"
- "I am interested in privacy"
- "Add a task to enable two-factor authentication"
- "Remind me to update my password tomorrow"
- "Start quiz"
- "Show activity log" or "What have you done for me?"

## GitHub submission checklist

- Minimum of six meaningful commits.
- CI workflow under `.github/workflows/dotnet.yml`.
- At least three tagged releases for the POE.
