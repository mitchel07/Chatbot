#  CyberGuard - Cybersecurity Awareness Chatbot

##   Project Overview

**CyberGuard** is a WPF-based Cybersecurity Awareness Chatbot designed to educate users about common cybersecurity threats and promote safer online behaviour.

The application combines an interactive chatbot with task management, a cybersecurity quiz, sentiment detection, memory, and activity tracking.

###   Main Features

*   Interactive cybersecurity chatbot
*   NLP-based intent detection
*   Sentiment detection
*   Conversation memory
*   Cybersecurity task assistant
*   MySQL database integration
*   12-question cybersecurity quiz
*   Quiz scoring and personalised results
*   Activity log
*   Voice greeting
*   Randomised chatbot responses

---

##   Project Structure

```text
CyberChatbotPOE/
│
├── Models/
│   ├── TaskItem.cs
│   └── QuizQuestion.cs
│
├── Data/
│   ├── DatabaseHelper.cs
│   ├── NlpEngine.cs
│   ├── ChatbotEngine.cs
│   └── QuizBank.cs
│
├── Views/
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
│
├── Resources/
│   └── greeting.wav
│
├── database_setup.sql
└── README.md
```

---

#   Setup Instructions

## 1. Prerequisites

Before running CyberGuard, make sure you have:

* **Visual Studio 2022** Community or higher
* **.NET 6 or .NET 8**
* **MySQL Server 8.x**
* **MySQL Workbench** (optional, but recommended)

---

## 2. Database Setup

Open **MySQL Workbench** and run the `database_setup.sql` script.

Alternatively, the script can be executed directly through the MySQL command line.

The database is used to store and manage cybersecurity tasks and activity log information.

---

## 3. Configure the Database Connection

Open:

```text
Data/DatabaseHelper.cs
```

Locate the connection string:

```csharp
private const string ConnectionString =
    "Server=localhost;Database=CyberChatbotDB;Uid=root;Pwd=YOUR_PASSWORD;";
```

Replace `YOUR_PASSWORD` with your local MySQL password.

>   Do not commit your real database password to GitHub.

---

## 4. Install MySQL NuGet Package

In Visual Studio, open **Package Manager Console** and run:

```text
Install-Package MySql.Data
```

Alternatively, install `MySql.Data` through the NuGet Package Manager.

---

## 5. Add the Voice Greeting

CyberGuard includes a voice greeting when the application starts.

To configure it:

1. Record a WAV audio file containing your greeting.
2. Name the file:

```text
greeting.wav
```

3. Place it inside:

```text
Resources/
```

4. In **Solution Explorer**, right-click the file and select **Properties**.
5. Set **Build Action** to `Content`.
6. Set **Copy to Output Directory** to `Copy if newer`.



## 6. Run the Application

Once everything is configured:

1. Open the project in Visual Studio.
2. Build the solution.
3. Make sure MySQL Server is running.
4. Press **F5** to launch CyberGuard.



#   How to Use CyberGuard

##   Chat Tab

The Chat tab allows users to interact with CyberGuard using natural-language-style commands.

| Example Input                        | CyberGuard Response                                      |
| ------------------------------------ | -------------------------------------------------------- |
| `What is phishing?`                  | Provides a phishing explanation and safety tip           |
| `Add task Enable 2FA`                | Saves a task to MySQL                                    |
| `Add task Review settings in 3 days` | Creates a task with a reminder                           |
| `Show my tasks`                      | Displays saved tasks                                     |
| `I am worried about scams`           | Detects sentiment and provides support                   |
| `Quiz`                               | Directs the user to the Quiz tab                         |
| `Show activity log`                  | Displays recent chatbot actions                          |
| `Tell me more`                       | Provides additional information about the previous topic |

The chatbot supports **20+ intents** and recognises different ways of asking similar questions.



#    Tasks Tab

The Tasks tab allows users to manage cybersecurity-related tasks.

Users can:

* Create a task
* Add an optional description
* Set an optional reminder date
* Mark a task as completed
* Delete a task
* View saved tasks

Tasks are stored using the **MySQL database**.

---

#   Quiz Tab

The Quiz tab contains a **12-question cybersecurity quiz** covering topics such as online safety and common cyber threats.

The quiz includes:

* Multiple-choice questions
* True/false questions
* One question displayed at a time
* Immediate answer feedback
* Explanations after each answer
* Score tracking
* Final results
* Personalised feedback based on the user's score

Click **Start Quiz** to begin.

---

#   Activity Log

CyberGuard automatically records important chatbot activities.

The Activity Log can be accessed directly through the application or by using the chat command:

```text
Show activity log
```

Users can also select **Clear Log** to reset the current activity history.

---

#   Features Implemented

| Feature                                  | Implementation    |
| ---------------------------------------- | ----------------- |
| MySQL task storage                       | Part 3 - Task 1   |
| Add, view, complete and delete tasks     | Part 3 - Task 1   |
| Task reminder dates                      | Part 3 - Task 1   |
| Chatbot task commands                    | Part 3 - Task 1   |
| 12-question cybersecurity quiz           | Part 3 - Task 2   |
| Multiple-choice and true/false questions | Part 3 - Task 2   |
| Immediate feedback and explanations      | Part 3 - Task 2   |
| Score tracking                           | Part 3 - Task 2   |
| Final quiz results                       | Part 3 - Task 2   |
| NLP intent detection                     | Part 3 - Task 3   |
| 20+ chatbot intents                      | Part 3 - Task 3   |
| Flexible phrasing recognition            | Part 3 - Task 3   |
| Activity logging                         | Part 3 - Task 4   |
| Database activity log                    | Part 3 - Task 4   |
| Chat-based log retrieval                 | Part 3 - Task 4   |
| Sentiment detection                      | Part 2 carry-over |
| Conversation memory                      | Part 2 carry-over |
| Random response variation                | Part 2 carry-over |
| Voice greeting                           | Part 1 carry-over |

---

#   Technical Concepts

CyberGuard demonstrates practical knowledge of:

* C#
* WPF
* Object-Oriented Programming
* Classes and objects
* Encapsulation
* Event handling
* GUI development
* MySQL
* SQL
* Database connectivity
* CRUD operations
* Regular expressions
* NLP-style intent detection
* Sentiment detection
* Exception handling
* Collections
* JSON/data handling
* Application state management

---

#   Cybersecurity Topics

The chatbot provides awareness information about common cybersecurity threats, including:

* Phishing
* Scams
* Password security
* Two-factor authentication
* Malware
* Ransomware
* Social engineering
* Safe browsing
* Online security practices

The goal is to help users recognise common threats and make safer decisions online.

---

#  References

Pieterse, H. (2021). *The Cyber Threat Landscape in South Africa: A 10-Year Review*. The African Journal of Information and Communication, 28(28).

DOI: https://doi.org/10.23962/10539/32213

Available from:
https://www.scielo.org.za/scielo.php?pid=S2077-72132021000200003&script=sci_arttext

**Accessed:** 16 February 2026.

---
