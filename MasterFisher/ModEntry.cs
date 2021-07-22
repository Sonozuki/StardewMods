using MasterFisher.Models;
using SatoCore;
using SatoCore.Extensions;
using StardewModdingAPI;
using System.Collections.Generic;

namespace MasterFisher
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : ModBase
    {
        /*********
        ** Fields
        *********/
        /// <summary>The fish categories that have been parsed from content packs.</summary>
        /// <remarks>This is used to temporarily store all categories before populating the repository, so edits and deletions can work correctly.</remarks>
        private readonly List<FishCategory> CategoriesBeingLoaded = new List<FishCategory>();

        /// <summary>The location areas that have been parsed from content packs.</summary>
        /// <remarks>This is used to temporarily store all location areas before populating the repository, so edits and deletions can work correctly.</remarks>
        private readonly List<LocationArea> LocationAreasBeingLoaded = new List<LocationArea>();

        /// <summary>The bait that have been parsed from content packs.</summary>
        /// <remarks>This is used to temporarily store all bait before populating the repository, so edits and deletions can work correctly.</remarks>
        private readonly List<Bait> BaitBeingLoaded = new List<Bait>();


        /*********
        ** Accessors
        *********/
        /// <summary>The loaded fish categories.</summary>
        public Repository<FishCategory, string> Categories { get; private set; }

        /// <summary>The loaded location areas.</summary>
        public Repository<LocationArea, string> LocationAreas { get; private set; }

        /// <summary>The loaded bait.</summary>
        public Repository<Bait, string> Bait { get; private set; }

        /// <summary>The singleton instance of <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Protected Methods
        *********/
        /// <inheritdoc/>
        protected override void Entry()
        {
            Instance = this;

            Categories = new Repository<FishCategory, string>(this.Monitor);
            LocationAreas = new Repository<LocationArea, string>(this.Monitor);
            Bait = new Repository<Bait, string>(this.Monitor);

            this.Helper.Events.GameLoop.SaveLoaded += (sender, e) => LoadContentPacks();

            this.Helper.ConsoleCommands.Add("mf_summary", "Logs the current state of all fish information.\n\nUsage: mf_summary", (command, args) => CommandManager.LogSummary());
        }

        /// <inheritdoc/>
        protected override void InitialiseContentPackLoading()
        {
            CategoriesBeingLoaded.Clear();
            LocationAreasBeingLoaded.Clear();
            BaitBeingLoaded.Clear();
        }

        /// <inheritdoc/>
        protected override void LoadContentPack(IContentPack contentPack)
        {
            // categories
            if (contentPack.TryLoadAsset<List<FishCategory>>("categories.json", out var fishCategories))
                CategoriesBeingLoaded.AddRange(fishCategories);

            // location areas
            if (contentPack.TryLoadAsset<List<LocationArea>>("locations.json", out var locations))
                LocationAreasBeingLoaded.AddRange(locations);

            // bait
            if (contentPack.TryLoadAsset<List<Bait>>("bait.json", out var bait))
                BaitBeingLoaded.AddRange(bait);
        }

        /// <inheritdoc/>
        protected override void FinaliseContentPackLoading()
        {
            Categories.Process(CategoriesBeingLoaded);
            LocationAreas.Process(LocationAreasBeingLoaded);
            Bait.Process(BaitBeingLoaded);
        }
    }
}
