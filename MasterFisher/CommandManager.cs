using StardewModdingAPI;

namespace MasterFisher
{
    /// <summary>Handles console commands.</summary>
    public static class CommandManager
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Logs the current fish data to the console.</summary>
        public static void LogSummary()
        {
            ModEntry.Instance.Monitor.Log("Categories:", LogLevel.Info);
            foreach (var category in ModEntry.Instance.Categories)
            {
                ModEntry.Instance.Monitor.Log($"    Name:           | {category.Name}", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    FishPondCap:    | {category.FishPondCap}", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    MinigameSprite: | {category.MinigameSprite}\n", LogLevel.Info);
            }
        }
    }
}
