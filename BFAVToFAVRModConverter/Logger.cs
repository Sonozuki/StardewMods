using System;

namespace BfavToFavrModConverter
{
    /// <summary>A wrapper around <see cref="Console"/> for easier logging with colour.</summary>
    public static class Logger
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Writes the given text to the console output.</summary>
        /// <param name="text">The text to display.</param>
        /// <param name="colour">The colour to display the text in.</param>
        public static void WriteLine(string text, ConsoleColor colour = ConsoleColor.White)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
