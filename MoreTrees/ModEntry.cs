using Harmony;
using Microsoft.Xna.Framework.Graphics;
using MoreTrees.Models;
using MoreTrees.Patches;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoreTrees
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The singletong instance of <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; set; }

        /// <summary>Provides basic More Trees apis.</summary>
        public IApi Api { get; set; }

        /// <summary>All the loaded trees.</summary>
        public List<CustomTree> LoadedTrees { get; set; } = new List<CustomTree>();

        /// <summary>Data about trees that gets saved.</summary>
        public List<SavePersistantTreeData> SavedTreeData { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Api = new Api();

            ApplyHarmonyPatches();

            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            this.Helper.Events.GameLoop.Saved += OnSaved;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Apply the harmony patches.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), "loadTexture"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.LoadTexturePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), "shake"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.ShakePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.performToolAction)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.PerformToolActionPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.UpdateTapperProduct)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.UpdateTapperProductPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.draw)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.DrawPrefix)))
            );
        }

        /// <summary>Invoked when the player loads a save.</summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // load all content packs
            foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
            {
                try
                {
                    LoadContentPack(contentPack);
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Failed to load content pack: {ex}");
                }
            }

            // load saved tree data
            SavedTreeData = GetSavePersistantTreeData();
        }

        /// <summary>Invoked when the player saves the game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            // decrement the number of days till next produce
            foreach (var savedTreeData in SavedTreeData)
            {
                savedTreeData.DaysTillNextBarkHarvest = Math.Max(0, savedTreeData.DaysTillNextBarkHarvest - 1);
                for (int i = 0; i < savedTreeData.DaysTillNextShakeProduct.Count; i++)
                    savedTreeData.DaysTillNextShakeProduct[i] = Math.Max(0, savedTreeData.DaysTillNextShakeProduct[i] - 1);
            }

            // add newly planted trees
            foreach (var terrainFeature in Game1.getFarm().terrainFeatures.Values)
            {
                var tree = terrainFeature as Tree;
                if (tree == null || tree.treeType <= 7) // don't add to the persistant save if it's a default tree
                    continue;

                if (!SavedTreeData.Where(treeData => treeData.TileLocation == terrainFeature.currentTileLocation).Any())
                {
                    // tree doesn't exist in custom saved data, add it
                    var customTree = Api.GetTreeByType(tree.treeType);

                    List<int> daysTillNextShakeProduce = new List<int>();
                    foreach (var shakeProduce in customTree.Data.ShakingProducts)
                        daysTillNextShakeProduce.Add(0);

                    SavedTreeData.Add(new SavePersistantTreeData(terrainFeature.currentTileLocation, 0, daysTillNextShakeProduce));
                }
            }

            // remove newly cut down trees
            for (int i = SavedTreeData.Count - 1; i >= 0; i--)
            {
                // check if the tree still exists, if not, delete it
                if (!Game1.getFarm().terrainFeatures.ContainsKey(SavedTreeData[i].TileLocation))
                    SavedTreeData.RemoveAt(i);
            }

            // save the custom save data
            SetSavePersistantTreeData();
        }

        /// <summary>Get the treeData.json file content.</summary>
        private List<SavePersistantTreeData> GetSavePersistantTreeData()
        {
            // get the content of the treeTypes.json file
            var treeDataPath = GetSavePersistantTreeDataPath();
            var treeData = File.ReadAllText(treeDataPath);
            List<SavePersistantTreeData> savedTreeData = null;

            // parse file data
            try
            {
                savedTreeData = JsonConvert.DeserializeObject<List<SavePersistantTreeData>>(treeData);
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"TreeData couldn't be deserialised. TreeData will be ignored, this will result in all trees having bark and drops being reset.\nPath: {treeDataPath}\nError: {ex}", LogLevel.Error);
            }

            return savedTreeData ?? new List<SavePersistantTreeData>();
        }

        /// <summary>Set the treeData.json file content.</summary>
        private void SetSavePersistantTreeData()
        {
            var treeDataPath = GetSavePersistantTreeDataPath();

            try
            {
                File.WriteAllText(treeDataPath, JsonConvert.SerializeObject(SavedTreeData));
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"TreeData couldn't be serialized.\nPath: {treeDataPath}\tError: {ex}", LogLevel.Error);
            }
        }

        /// <summary>Get the treeData.json path.</summary>
        /// <returns>The treeData.json path.</returns>
        private string GetSavePersistantTreeDataPath()
        {
            // get/create the directory containing the treeData File
            var treeTypesFileDirectory = Path.Combine(Constants.CurrentSavePath, "MoreTrees");
            if (!Directory.Exists(treeTypesFileDirectory))
                Directory.CreateDirectory(treeTypesFileDirectory);

            // get/create the treeData File
            var treeTypesFilePath = Path.Combine(treeTypesFileDirectory, "treeData.json");
            if (!File.Exists(treeTypesFilePath))
                File.Create(treeTypesFilePath).Close();

            return treeTypesFilePath;
        }
        
        /// <summary>Load the passed content pack.</summary>
        /// <param name="contentPack">The content pack to load.</param>
        private void LoadContentPack(IContentPack contentPack)
        {
            this.Monitor.Log($"Loading content pack: {contentPack.Manifest.Name}", LogLevel.Info);

            // load each tree
            var modDirectory = new DirectoryInfo(contentPack.DirectoryPath);
            foreach (var treePath in modDirectory.EnumerateDirectories())
            {
                // ensure tree.png exists
                var isValid = true;
                if (!File.Exists(Path.Combine(treePath.FullName, "tree.png")))
                {
                    this.Monitor.Log($"tree.png couldn't be found for {contentPack.Manifest.Name}.", LogLevel.Error);
                    isValid = false;
                }

                // ensure content.json exists
                if (!File.Exists(Path.Combine(treePath.FullName, "content.json")))
                {
                    this.Monitor.Log($"content.json couldn't be found for {contentPack.Manifest.Name}.", LogLevel.Error);
                    isValid = false;
                }

                if (!isValid)
                    continue;

                var treeTexture = contentPack.LoadAsset<Texture2D>(Path.Combine(treePath.Name, "tree.png"));
                var treeData = contentPack.ReadJsonFile<TreeData>(Path.Combine(treePath.Name, "content.json"));
                if (treeData == null)
                {
                    this.Monitor.Log($"Content.json couldn't be found for: {treePath.Name}.", LogLevel.Error);
                    continue;
                }

                treeData.ResolveTokens();
                if (!treeData.IsValid())
                {
                    this.Monitor.Log($"Validation for treeData for: {treePath.Name} failed, skipping.", LogLevel.Error);
                    continue;
                }

                // ensure the tree can be loaded (using IncludeIfModIsPresent)
                {
                    var loadTree = true;
                    if (treeData.IncludeIfModIsPresent != null && treeData.IncludeIfModIsPresent.Count > 0)
                    {
                        // set this to false so it can be set to true if a required mod is found
                        loadTree = false;
                        foreach (var requiredMod in treeData.IncludeIfModIsPresent)
                        {
                            if (!this.Helper.ModRegistry.IsLoaded(requiredMod))
                                continue;

                            loadTree = true;
                            break;
                        }
                    }
                    if (!loadTree)
                    {
                        this.Monitor.Log("Tree won't get loaded as no mods specified in 'IncludeIfModIsPresent' were present.", LogLevel.Info);
                        continue;
                    }
                }

                // ensure the tree can be loaded (using ExcludeIfModIsPresent)
                {
                    var loadTree = true;
                    if (treeData.ExcludeIfModIsPresent != null && treeData.ExcludeIfModIsPresent.Count > 0)
                    {
                        foreach (var unwantedMod in treeData.ExcludeIfModIsPresent)
                        {
                            if (!this.Helper.ModRegistry.IsLoaded(unwantedMod))
                                continue;

                            loadTree = false;
                            break;
                        }
                    }
                    if (!loadTree)
                    {
                        this.Monitor.Log("Tree won't get loaded as a mod specified in 'ExcludeIfModIsPresent' was present.", LogLevel.Info);
                        continue;
                    }
                }

                // ensure the tree hasn't been added by another mod
                if (LoadedTrees.Where(tree => tree.Name.ToLower() == treePath.Name.ToLower()).Any())
                {
                    this.Monitor.Log($"A tree by the name: {treePath.Name} has already been added.", LogLevel.Error);
                    continue;
                }

                // get the tree type, use the api as they're save persitant
                var treeType = Api.GetTreeType(treePath.Name);

                // add the tree to the loaded trees
                LoadedTrees.Add(new CustomTree(treeType, treePath.Name, treeData, treeTexture));
            }
        }
    }
}
