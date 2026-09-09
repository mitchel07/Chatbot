using System;
using System.IO;
using System.Media;

namespace CybersecurityChatbot.Wpf.Services
{
    /// <summary>
    /// Plays the recorded WAV welcome message when the GUI loads (Part 1
    /// requirement carried into the GUI). Failures are swallowed so a missing
    /// audio file never crashes the window.
    /// </summary>
    public class VoiceService
    {
        public string AudioFilePath { get; private set; }

        public VoiceService()
        {
            AudioFilePath = Path.Combine(AppContext.BaseDirectory, "Assets", "greeting.wav");
        }

        public void PlayGreeting()
        {
            try
            {
                if (!File.Exists(AudioFilePath))
                {
                    return;
                }
                var player = new SoundPlayer(AudioFilePath);
                // Play asynchronously so it does not freeze the UI thread.
                player.Play();
            }
            catch
            {
                // Ignore audio errors; the chat still works without sound.
            }
        }
    }
}
