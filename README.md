CyberGuard Cybersecurity Awareness Chatbot (Part 3 / POE)

Project Overview

CyberGuard is a WPF-based Cybersecurity Awareness Chatbot for South African citizens. It educates users about online threats through an interactive GUI that includes:

Chat with NLP simulation, sentiment detection, and memory
Task Assistant backed by MySQL for managing cybersecurity tasks
Quiz Mini-Game with 12 questions (multiple choice + true/false)
Activity Log tracking all chatbot actions
Project Structure
CyberChatbotPOE/
├── Models/
│   ├── TaskItem.cs
│   └── QuizQuestion.cs
├── Data/
│   ├── DatabaseHelper.cs
│   ├── NlpEngine.cs
│   ├── ChatbotEngine.cs
│   └── QuizBank.cs
├── Views/
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
├── Resources/
│   └── greeting.wav
├── database_setup.sql
└── README.md
Setup Instructions
1. Prerequisites
Visual Studio 2022 (Community or higher)
.NET 6 or .NET 8
MySQL Server 8.x installed and running
MySQL Workbench (optional but recommended)
2. Database Setup

Open MySQL Workbench and run the script database_setup.sql, or paste its contents directly into your MySQL terminal.

3. Configure Connection String

Open Data/DatabaseHelper.cs and update:

csharp
private const string ConnectionString =
    "Server=localhost;Database=CyberChatbotDB;Uid=root;Pwd=YOUR_PASSWORD;";

Replace YOUR_PASSWORD with your actual MySQL root password.

4. Install NuGet Package

In Visual Studio, open Package Manager Console and run:

Install-Package MySql.Data
5. Add Your Voice Greeting
Record a WAV file (e.g. "Hello! Welcome to CyberGuard...")
Place it in the Resources/ folder as greeting.wav
In Solution Explorer: right-click the file, Properties, set Build Action to Content and Copy to Output Directory to Copy if newer
6. Run the Project

Press F5 in Visual Studio.

How to Use
Chat Tab
You type	Bot does
What is phishing?	Gives a phishing tip
Add task Enable 2FA	Saves task to MySQL
Add task Review settings in 3 days	Saves task with reminder
Show my tasks	Lists all tasks
I am worried about scams	Detects sentiment, gives tips
Quiz	Directs you to quiz tab
Show activity log	Shows recent actions
Tell me more	Follows up on last topic
Tasks Tab

Fill in a title, optional description, and optional reminder date. Click Done to complete a task, Delete to remove it.

Quiz Tab

Click Start Quiz for 12 questions one at a time. You get immediate feedback and an explanation after each answer, plus a final score with a personalised message.

Activity Log Tab

All actions are logged automatically. Click Clear Log to reset.

Features Implemented
Feature	Task
MySQL task storage (add/view/complete/delete)	Task 1
Reminder date per task	Task 1
Chatbot integration for task commands	Task 1
12-question quiz (multiple choice and true/false)	Task 2
One question at a time with feedback	Task 2
Score tracking and result screen	Task 2
NLP intent detection (20+ intents)	Task 3
Flexible phrasing recognition	Task 3
Activity log (in-memory and DB)	Task 4
Show log command in chat	Task 4
Sentiment detection (worried/curious/frustrated)	Part 2 carry-over
Memory (name and favourite topic)	Part 2 carry-over
Random response variation	Part 2 carry-over
Voice greeting (WAV)	Part 1 carry-over
References

Pieterse, H. 2021. The Cyber Threat Landscape in South Africa: A 10-Year Review. The African Journal of Information and Communication, 28(28). doi: https://doi.org/10.23962/10539/32213. [Online]. Available at: https://www.scielo.org.za/scielo.php?pid=S2077-72132021000200003&script=sci_arttext [Accessed 16 February 2026].
