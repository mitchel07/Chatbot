using System.Collections.Generic;
using CybersecurityChatbot.Wpf.Models;

namespace CybersecurityChatbot.Wpf.Services
{
    /// <summary>
    /// Holds the quiz questions and tracks progress and score for one game.
    /// More than ten questions are provided, mixing multiple-choice and true/false.
    /// </summary>
    public class QuizService
    {
        private readonly List<QuizQuestion> _questions;
        private int _currentIndex;

        public int Score { get; private set; }
        public int CurrentNumber => _currentIndex + 1;
        public int Total => _questions.Count;
        public bool IsFinished => _currentIndex >= _questions.Count;

        public QuizService()
        {
            _questions = BuildQuestions();
            Reset();
        }

        public void Reset()
        {
            _currentIndex = 0;
            Score = 0;
        }

        public QuizQuestion Current()
        {
            return IsFinished ? null : _questions[_currentIndex];
        }

        /// <summary>
        /// Records the answer, returns whether it was correct, and advances.
        /// </summary>
        public bool Answer(int chosenIndex, out string feedback)
        {
            QuizQuestion q = Current();
            bool correct = q != null && q.IsCorrect(chosenIndex);
            if (correct)
            {
                Score++;
                feedback = "Correct! " + q.Explanation;
            }
            else
            {
                feedback = "Not quite. The correct answer is \"" + (q != null ? q.CorrectAnswerText() : string.Empty)
                           + "\". " + (q != null ? q.Explanation : string.Empty);
            }
            _currentIndex++;
            return correct;
        }

        /// <summary>
        /// Final message based on the score.
        /// </summary>
        public string ResultMessage()
        {
            double ratio = Total == 0 ? 0 : (double)Score / Total;
            if (ratio >= 0.8)
            {
                return "Great job! You are a cybersecurity pro!";
            }
            if (ratio >= 0.5)
            {
                return "Good effort! A little more practice and you will be very safe online.";
            }
            return "Keep learning to stay safe online. Review the tips and try again!";
        }

        private List<QuizQuestion> BuildQuestions()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion(
                    "What should you do if you receive an email asking for your password?",
                    new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    2,
                    "Reporting phishing emails helps prevent scams and protects others."),

                new QuizQuestion(
                    "True or False: Using the same password for every account is safe as long as it is long.",
                    new List<string> { "True", "False" },
                    1,
                    "Reusing passwords is risky. If one site is breached, all your accounts are exposed."),

                new QuizQuestion(
                    "Which of these is the strongest password?",
                    new List<string> { "password123", "Mitchel", "7r!verBlue-Sky29", "qwerty" },
                    2,
                    "Strong passwords are long and mix letters, numbers and symbols, with no personal details."),

                new QuizQuestion(
                    "What does two-factor authentication (2FA) add to your login?",
                    new List<string> { "A second password you reuse", "A second step such as a code on your phone", "A faster login", "Nothing useful" },
                    1,
                    "2FA adds a second verification step, so a stolen password alone is not enough."),

                new QuizQuestion(
                    "True or False: A padlock and https in the address bar guarantee a website is completely safe.",
                    new List<string> { "True", "False" },
                    1,
                    "Https means the connection is encrypted, but scam sites can also use https. Always check the address too."),

                new QuizQuestion(
                    "A message says you won a prize and asks for your bank details to claim it. What is it most likely?",
                    new List<string> { "A genuine prize", "A phishing or scam attempt", "A software update", "A friend's message" },
                    1,
                    "Unexpected prizes that ask for personal or bank details are a classic scam."),

                new QuizQuestion(
                    "What is 'social engineering' in cybersecurity?",
                    new List<string> { "Building social networks", "Manipulating people into revealing information", "A type of antivirus", "Coding a website" },
                    1,
                    "Social engineering tricks people, rather than computers, into giving up information."),

                new QuizQuestion(
                    "True or False: Public Wi-Fi is always safe for online banking.",
                    new List<string> { "True", "False" },
                    1,
                    "Public Wi-Fi can be insecure. Avoid sensitive tasks or use a trusted VPN."),

                new QuizQuestion(
                    "How often should you update your software and apps?",
                    new List<string> { "Never", "Only when it breaks", "Regularly, as updates are released", "Once a year" },
                    2,
                    "Updates fix security holes, so install them regularly."),

                new QuizQuestion(
                    "You get a link from an unknown sender. What is the safest action?",
                    new List<string> { "Click it quickly", "Forward it to friends", "Do not click and verify the sender first", "Enter your login to check" },
                    2,
                    "Do not click unexpected links. Verify the sender through a trusted channel first."),

                new QuizQuestion(
                    "True or False: You should share one-time verification codes (OTP) if someone calls and asks for them.",
                    new List<string> { "True", "False" },
                    1,
                    "Never share OTP codes. Genuine organisations will never ask you for them."),

                new QuizQuestion(
                    "What is the best way to store many strong, unique passwords?",
                    new List<string> { "Write them on a sticky note", "Use a reputable password manager", "Use your birthday for all", "Memorise one and reuse it" },
                    1,
                    "A password manager safely stores unique passwords so you do not have to reuse them.")
            };
        }
    }
}
