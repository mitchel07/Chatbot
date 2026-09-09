using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CybersecurityChatbot.Wpf.Services
{
    /// <summary>
    /// The brain of the chatbot. It carries forward every Part 1 and Part 2
    /// feature and adds the Part 3 NLP routing:
    ///   - Keyword recognition (password, scam, privacy, phishing, and more).
    ///   - Random responses chosen from lists so replies stay varied.
    ///   - Conversation flow ("tell me more", "another tip") that stays on topic.
    ///   - Memory and recall (the user's name and favourite topic).
    ///   - Simple sentiment detection (worried, curious, frustrated).
    ///   - NLP simulation: detects intents such as add task, set reminder,
    ///     start quiz and show activity log using string and regex matching.
    /// </summary>
    public class ChatbotEngine
    {
        private readonly Random _random = new Random();

        // ----- Memory -----
        public string UserName { get; set; } = "there";
        public string FavouriteTopic { get; private set; }

        // The last cybersecurity topic discussed, so "tell me more" can continue it.
        private string _lastTopic;

        // ----- Keyword tip banks (lists -> random selection) -----
        private readonly Dictionary<string, List<string>> _tips = new Dictionary<string, List<string>>
        {
            ["password"] = new List<string>
            {
                "Use a long, unique passphrase for each account. Avoid personal details such as your name or birthday.",
                "A password manager lets you keep strong, unique passwords without memorising them all.",
                "Turn on two-factor authentication so a stolen password alone cannot unlock your account."
            },
            ["phishing"] = new List<string>
            {
                "Be cautious of emails asking for personal information. Scammers often pose as trusted organisations.",
                "Check the sender's address carefully and hover over links before clicking them.",
                "Never enter your password on a page you reached by clicking a link in an unexpected email."
            },
            ["scam"] = new List<string>
            {
                "If an offer sounds too good to be true, it usually is. Verify before you act.",
                "Never send money or share bank details based on an unexpected call or message.",
                "Scammers create urgency to rush you. Slow down and confirm through an official channel."
            },
            ["privacy"] = new List<string>
            {
                "Review the privacy settings on your social media and limit what you share publicly.",
                "Only give apps the permissions they genuinely need to work.",
                "Think before you post. Personal details can be used to guess passwords or answers."
            },
            ["safe browsing"] = new List<string>
            {
                "Look for https and the padlock, but remember that alone does not guarantee a site is honest.",
                "Keep your browser and operating system updated to close security holes.",
                "Avoid downloading files or software from sites you do not trust."
            },
            ["2fa"] = new List<string>
            {
                "Two-factor authentication adds a second step, such as a code on your phone, to your login.",
                "Enable 2FA on your email first, because it can reset all your other accounts."
            }
        };

        // ----- Simple Q and A (Part 1 carried forward) -----
        private readonly Dictionary<string, string> _basicResponses = new Dictionary<string, string>
        {
            ["how are you"] = "I am running smoothly and ready to help you stay safe online.",
            ["what is your purpose"] = "My purpose is to raise your cybersecurity awareness and help you avoid online threats.",
            ["purpose"] = "My purpose is to raise your cybersecurity awareness and help you avoid online threats.",
            ["what can i ask"] = "You can ask me about passwords, phishing, scams, privacy and safe browsing. You can also say 'start quiz', 'add a task' or 'show activity log'.",
            ["thank"] = "You are welcome! Staying safe online is a team effort.",
            ["hello"] = "Hello! How can I help you stay safe online today?",
            ["hi"] = "Hi! Ask me a cybersecurity question, or try the quiz and task features."
        };

        // ----- Sentiment cues -----
        private readonly Dictionary<string, string> _sentimentReplies = new Dictionary<string, string>
        {
            ["worried"] = "It is completely understandable to feel worried. Scammers can be very convincing. Let me share something that helps.",
            ["scared"] = "It is completely understandable to feel that way. You are taking the right step by learning. Here is a helpful tip.",
            ["anxious"] = "That is a very normal feeling. Knowing what to look for takes away a lot of the fear. Here is some guidance.",
            ["frustrated"] = "I understand this can be frustrating. Let me make it simpler for you.",
            ["confused"] = "No problem at all, let me explain it more clearly.",
            ["curious"] = "I love the curiosity! Here is something interesting to know."
        };

        public ChatbotEngine(string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                UserName = userName;
            }
        }

        /// <summary>
        /// Main entry point. Takes the user's raw message and returns a reply and
        /// any action the GUI should take.
        /// </summary>
        public ChatResult Process(string rawInput)
        {
            if (string.IsNullOrWhiteSpace(rawInput))
            {
                return new ChatResult("I did not catch that. Could you type your question again?");
            }

            string input = rawInput.Trim();
            string lower = input.ToLowerInvariant();

            // 1. NLP intent routing first, so commands work even when worded loosely.
            ChatResult intent = DetectIntent(input, lower);
            if (intent != null)
            {
                return intent;
            }

            // 2. Remember a favourite topic if the user states an interest.
            string interest = DetectInterest(lower);
            if (interest != null)
            {
                FavouriteTopic = interest;
                _lastTopic = interest;
                string tip = PickTip(interest);
                return new ChatResult("Great, I will remember that you are interested in " + interest +
                    ". It is an important part of staying safe online. " + tip);
            }

            // 3. Sentiment: if detected, acknowledge feelings AND still give help
            //    on the same message, so the user does not have to ask again.
            string sentimentPrefix = DetectSentiment(lower);

            // 4. Follow-up flow: "tell me more" / "another tip" continues last topic.
            if (IsFollowUp(lower) && _lastTopic != null)
            {
                string moreTip = PickTip(_lastTopic);
                return new ChatResult(Combine(sentimentPrefix, "Here is more on " + _lastTopic + ": " + moreTip));
            }

            // 5. Keyword recognition with random tip selection.
            string topic = DetectTopic(lower);
            if (topic != null)
            {
                _lastTopic = topic;
                string tip = PickTip(topic);
                string recall = MaybeRecallFavourite(topic);
                return new ChatResult(Combine(sentimentPrefix, recall + tip));
            }

            // 6. Basic predefined responses.
            foreach (KeyValuePair<string, string> pair in _basicResponses)
            {
                if (lower.Contains(pair.Key))
                {
                    return new ChatResult(Combine(sentimentPrefix, pair.Value));
                }
            }

            // 7. If we at least detected a sentiment, respond supportively.
            if (sentimentPrefix != null)
            {
                return new ChatResult(sentimentPrefix +
                    " Tell me which topic you would like help with: passwords, phishing, scams or privacy.");
            }

            // 8. Default fallback (kept rare on purpose).
            return new ChatResult("I am not sure I understand. Could you try rephrasing? You can ask about " +
                "passwords, phishing, scams or privacy, or say 'start quiz' or 'add a task'.");
        }

        // ---------------------------------------------------------------
        // NLP simulation: recognise commands even when worded differently.
        // ---------------------------------------------------------------
        private ChatResult DetectIntent(string original, string lower)
        {
            // Quiz
            if (Mentions(lower, "quiz", "game", "test my knowledge", "play"))
            {
                return new ChatResult("Starting the cybersecurity quiz. Good luck, " + UserName + "!")
                {
                    Action = BotAction.StartQuiz
                };
            }

            // Activity log
            if (Mentions(lower, "activity log", "show log", "what have you done", "recent actions", "history"))
            {
                return new ChatResult("Here is a summary of recent actions:")
                {
                    Action = BotAction.ShowActivityLog
                };
            }

            // View tasks
            if (Mentions(lower, "show task", "view task", "my task", "list task", "see my task"))
            {
                return new ChatResult("Here are your cybersecurity tasks:")
                {
                    Action = BotAction.ShowTasks
                };
            }

            // Set a reminder (for the most recent task) - check before add, because
            // "remind me" can follow an add request.
            if (Mentions(lower, "remind me", "set a reminder", "set reminder", "reminder"))
            {
                DateTime? when = ExtractReminderDate(lower);
                var result = new ChatResult(
                    when.HasValue
                        ? "Got it. I will remind you on " + when.Value.ToString("dd MMM yyyy") + "."
                        : "Got it. I have noted a reminder for your most recent task.")
                {
                    Action = BotAction.SetReminderForLastTask,
                    ReminderDate = when ?? DateTime.Now.AddDays(7)
                };
                return result;
            }

            // Add a task. Recognise "add task", "add a task", "create a task",
            // "i need to", "remind me to" (which implies a task), etc.
            if (Mentions(lower, "add task", "add a task", "create task", "create a task", "new task", "add a reminder to"))
            {
                string title = ExtractTaskTitle(original);
                DateTime? inlineReminder = ExtractReminderDate(lower);
                return new ChatResult("Task added: \"" + title + "\". Would you like to set a reminder for this task?")
                {
                    Action = BotAction.AddTask,
                    TaskTitle = title,
                    TaskDescription = "Cybersecurity task: " + title,
                    ReminderDate = inlineReminder
                };
            }

            return null;
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------
        private static bool Mentions(string text, params string[] phrases)
        {
            foreach (string p in phrases)
            {
                if (text.Contains(p))
                {
                    return true;
                }
            }
            return false;
        }

        private string DetectTopic(string lower)
        {
            // Match longer phrases first.
            if (lower.Contains("safe browsing") || lower.Contains("browsing") || lower.Contains("browser")) return "safe browsing";
            if (lower.Contains("password")) return "password";
            if (lower.Contains("phishing") || lower.Contains("phish")) return "phishing";
            if (lower.Contains("scam") || lower.Contains("fraud")) return "scam";
            if (lower.Contains("privacy") || lower.Contains("private")) return "privacy";
            if (lower.Contains("2fa") || lower.Contains("two-factor") || lower.Contains("two factor") || lower.Contains("authentication")) return "2fa";
            return null;
        }

        private string DetectInterest(string lower)
        {
            // Phrases like "I'm interested in privacy" or "my favourite topic is phishing".
            if (Regex.IsMatch(lower, @"interested in|favou?rite topic|i like|i want to learn"))
            {
                return DetectTopic(lower);
            }
            return null;
        }

        private string DetectSentiment(string lower)
        {
            foreach (KeyValuePair<string, string> pair in _sentimentReplies)
            {
                if (lower.Contains(pair.Key))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        private static bool IsFollowUp(string lower)
        {
            return Mentions(lower, "tell me more", "another tip", "more tips", "explain more",
                "another", "go on", "continue", "more");
        }

        private string PickTip(string topic)
        {
            if (topic != null && _tips.ContainsKey(topic))
            {
                List<string> list = _tips[topic];
                return list[_random.Next(list.Count)];
            }
            return "Stay alert, keep your software updated, and think before you click.";
        }

        private string MaybeRecallFavourite(string topic)
        {
            if (!string.IsNullOrEmpty(FavouriteTopic) && FavouriteTopic == topic)
            {
                return "As someone interested in " + FavouriteTopic + ", you will like this. ";
            }
            return string.Empty;
        }

        private static string Combine(string prefix, string body)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return body;
            }
            return prefix + " " + body;
        }

        /// <summary>
        /// Pulls a task title out of a sentence such as
        /// "Add a task to enable two-factor authentication" or
        /// "Remind me to update my password".
        /// </summary>
        private string ExtractTaskTitle(string original)
        {
            string text = original.Trim();

            // Look for the part after "to" first, which usually holds the action.
            Match m = Regex.Match(text, @"\bto\s+(.+)$", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                return CleanTitle(m.Groups[1].Value);
            }

            // Otherwise strip the leading command words.
            string cleaned = Regex.Replace(text,
                @"^\s*(please\s+)?(add|create|new)\s+(a\s+)?task\s*(-|:|to)?\s*",
                string.Empty, RegexOptions.IgnoreCase);

            return CleanTitle(string.IsNullOrWhiteSpace(cleaned) ? text : cleaned);
        }

        private static string CleanTitle(string title)
        {
            // Remove any trailing reminder phrase from the title text.
            title = Regex.Replace(title, @"\b(remind me|reminder).*$", string.Empty, RegexOptions.IgnoreCase).Trim();
            title = title.TrimEnd('.', ',', '!', '?').Trim();
            if (title.Length == 0)
            {
                return "Cybersecurity task";
            }
            return char.ToUpperInvariant(title[0]) + title.Substring(1);
        }

        /// <summary>
        /// Reads a reminder timeframe from text: "tomorrow", "in 3 days",
        /// "in 2 weeks", or a date like 2026-07-01. Returns null if none found.
        /// </summary>
        private DateTime? ExtractReminderDate(string lower)
        {
            if (lower.Contains("tomorrow"))
            {
                return DateTime.Now.AddDays(1);
            }
            if (lower.Contains("today"))
            {
                return DateTime.Now;
            }
            if (lower.Contains("next week"))
            {
                return DateTime.Now.AddDays(7);
            }

            Match days = Regex.Match(lower, @"in\s+(\d+)\s+day");
            if (days.Success)
            {
                return DateTime.Now.AddDays(int.Parse(days.Groups[1].Value));
            }

            Match weeks = Regex.Match(lower, @"in\s+(\d+)\s+week");
            if (weeks.Success)
            {
                return DateTime.Now.AddDays(7 * int.Parse(weeks.Groups[1].Value));
            }

            Match date = Regex.Match(lower, @"(\d{4}-\d{1,2}-\d{1,2})");
            if (date.Success && DateTime.TryParse(date.Groups[1].Value, out DateTime parsed))
            {
                return parsed;
            }

            return null;
        }
    }
}
