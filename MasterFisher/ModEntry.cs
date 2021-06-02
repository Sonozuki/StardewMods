using MasterFisher.Models.Converted;
using MasterFisher.Models.Parsed;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MasterFisher
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Provides basic fish apis.</summary>
        public IApi Api { get; private set; }

        /// <summary>The loaded fish categories.</summary>
        public List<FishCategory> Categories { get; } = new List<FishCategory>();

        /// <summary>The singleton instance of <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Api = new Api();

            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            this.Helper.ConsoleCommands.Add("mf_summary", "Logs the current state of all fish information.\n\nUsage: mf_summary", (command, args) => CommandManager.LogSummary());
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Invoked when the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to load the content packs.</remarks>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            LoadContentPacks();
        }

        /// <summary>Loads all the content packs.</summary>
        private void LoadContentPacks()
        {
            var categories = new List<ParsedFishCategory>();

            foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
            {
                try
                {
                    this.Monitor.Log($"Loading {contentPack.Manifest.Name}", LogLevel.Info);

                    // categories
                    if (File.Exists(Path.Combine(contentPack.DirectoryPath, "categories.json")))
                        categories.AddRange(contentPack.LoadAsset<List<ParsedFishCategory>>("categories.json"));
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Unhandled exception occured when loading content pack: {contentPack.Manifest.Name}\n{ex}", LogLevel.Error);
                }
            }

            // categories
            foreach (var category in categories.Where(category => category.Action == Action.Add))
                Api.AddFishCategory(category);
            foreach (var category in categories.Where(category => category.Action == Action.Edit))
                Api.EditFishCategory(category);
            foreach (var category in categories.Where(category => category.Action == Action.Delete))
                Api.DeleteFishCategory(category.Name);
        }
    }
}
