using System.Collections.Generic;

namespace CybersecurityChatbot.Wpf.Models
{
    /// <summary>
    /// A single quiz question. Supports both multiple-choice and true/false
    /// formats (a true/false question simply has the two options "True"/"False").
    /// </summary>
    public class QuizQuestion
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }

        // Zero-based index of the correct option in Options.
        public int CorrectIndex { get; set; }

        // Short explanation shown after the user answers.
        public string Explanation { get; set; }

        public QuizQuestion(string text, List<string> options, int correctIndex, string explanation)
        {
            Text = text;
            Options = options;
            CorrectIndex = correctIndex;
            Explanation = explanation;
        }

        public bool IsCorrect(int chosenIndex)
        {
            return chosenIndex == CorrectIndex;
        }

        public string CorrectAnswerText()
        {
            if (Options != null && CorrectIndex >= 0 && CorrectIndex < Options.Count)
            {
                return Options[CorrectIndex];
            }
            return string.Empty;
        }
    }
}
