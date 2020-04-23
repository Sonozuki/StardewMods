using FarmAnimalVarietyRedux.Models;
using FarmAnimalVarietyRedux.Patches;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace FarmAnimalVarietyRedux
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Provides methods for interacting with the mod directory.</summary>
        public static IModHelper ModHelper { get; private set; }

        /// <summary>Provides methods for logging to the console.</summary>
        public static IMonitor ModMonitor { get; private set; }

        /// <summary>A list of all the animals.</summary>
        public static List<Animal> Animals { get; private set; } = new List<Animal>();

        /// <summary>The custom animal data for the custom farm animals.</summary>
        public Dictionary<string, string> DataStrings { get; private set; } = new Dictionary<string, string>();

        /// <summary>Whether the content packs have been loaded.</summary>
        public bool ContentPacksLoaded { get; set; }

        /// <summary>The singleton instance of the <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; set; }


        /*********
        ** Public Methods 
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = this.Helper;
            ModMonitor = this.Monitor;
            Instance = this;

            ApplyHarmonyPatches();
        }

        /// <summary>This will call when loading each asset, if the mail asset is being loaded, return true as we want to edit this.</summary>
        /// <typeparam name="T">The type of the assets being loaded.</typeparam>
        /// <param name="asset">The asset info being loaded.</param>
        /// <returns>True if the assets being loaded needs to be edited.</returns>
        public bool CanEdit<T>(IAssetInfo asset) => asset.AssetNameEquals("Data\\FarmAnimals");

        /// <summary>Edit the farm animals asset to add the the custom data strings.</summary>
        /// <typeparam name="T">The type of the assets being loaded.</typeparam>
        /// <param name="asset">The asset data being loaded.</param>
        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            foreach (var dataString in DataStrings)
                data.Add(dataString);
        }

        /// <summary>Load all the sprites for the new animals from the loaded content packs.</summary>
        public void LoadContentPacks()
        {
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                Monitor.Log($"Loading {contentPack.Manifest.Name}");

                // loop through each animal folder in the current content pack
                foreach (var animalFolder in Directory.GetDirectories(contentPack.DirectoryPath))
                {
                    var animalFolderSplit = animalFolder.Split(Path.DirectorySeparatorChar);
                    var animalName = animalFolderSplit[animalFolderSplit.Length - 1];

                    // ensure content.json file exists
                    if (!File.Exists(Path.Combine(animalFolder, "content.json")))
                    {
                        this.Monitor.Log($"Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} doesn't contain a content.json file.", LogLevel.Error);
                        continue;
                    }

                    // serialize and validate content.json file
                    AnimalData animalData = contentPack.LoadAsset<AnimalData>(Path.Combine(animalName, "content.json"));
                    bool isAnimalValid = animalData.IsValid(animalName);
                    if (!isAnimalValid)
                    {
                        this.Monitor.Log($"Content.json is not valid for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName}", LogLevel.Error);
                        continue;
                    }

                    // loop through each sub type and add them (such as colour varients etc)
                    List<AnimalSubType> animalSubTypes = new List<AnimalSubType>();
                    foreach (var type in animalData.Types)
                    {
                        // get sprites
                        var sprites = new AnimalSprites(
                            adultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"{type}.png"), contentPack),
                            babySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"Baby {type}.png"), contentPack),
                            harvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"Harvested {type}.png"), contentPack),
                            springAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"{type}.png"), contentPack),
                            springHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"Harvested {type}.png"), contentPack),
                            springBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"Baby {type}.png"), contentPack),
                            summerAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"{type}.png"), contentPack),
                            summerHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"Harvested {type}.png"), contentPack),
                            summerBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"Baby {type}.png"), contentPack),
                            fallAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"{type}.png"), contentPack),
                            fallHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"Harvested {type}.png"), contentPack),
                            fallBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"Baby {type}.png"), contentPack),
                            winterAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"{type}.png"), contentPack),
                            winterHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"Harvested {type}.png"), contentPack),
                            winterBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"Baby {type}.png"), contentPack)
                        );

                        // ensure sprites are valid
                        if (!sprites.IsValid())
                        {
                            this.Monitor.Log($"Sprites are not valid for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} >> Subtype: {type}", LogLevel.Error);
                            continue;
                        }

                        // get data
                        AnimalSubTypeData data;
                        if (File.Exists(Path.Combine(animalFolder, "assets", $"{type} content.json")))
                        {
                            data = contentPack.LoadAsset<AnimalSubTypeData>(Path.Combine(animalName, "assets", $"{type} content.json"));
                        }
                        else if (File.Exists(Path.Combine(animalFolder, "assets", "content.json")))
                        {
                            data = contentPack.LoadAsset<AnimalSubTypeData>(Path.Combine(animalName, "assets", "content.json"));
                        }
                        else
                        {
                            this.Monitor.Log($"Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} >> Type: {type} doesn't have a content.json file.", LogLevel.Error);
                            continue;
                        }

                        // resolve API tokens in data and validate it
                        data.ResolveTokens();
                        if (!data.IsValid(type))
                        {
                            this.Monitor.Log($"Data is not valid for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} >> Subtype: {type}", LogLevel.Error);
                            continue;
                        }

                        // create subtype and add to animal
                        var animalSubType = new AnimalSubType(
                            name: type,
                            data: data,
                            sprites: sprites
                        );

                        animalSubTypes.Add(animalSubType);
                    }

                    // ensure there were valid sub types
                    if (animalSubTypes.Count == 0)
                    {
                        this.Monitor.Log($"No valid sub types for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName}.\n Animal will not be added.", LogLevel.Error);
                        continue;
                    }

                    // create, validate, and add the animal
                    var animal = new Animal(
                        name: animalName,
                        data: animalData,
                        shopIcon: GetSpriteByPath(Path.Combine(animalName, $"shopdisplay.png"), contentPack),
                        subTypes: animalSubTypes
                    );

                    if (!animal.IsValid())
                    {
                        this.Monitor.Log($"Animal is not valid at Content pack: {contentPack.Manifest.Name} >> Animal: {animalName}", LogLevel.Error);
                        continue;
                    }

                    Animals.Add(animal);

                    // construct data string for game to use
                    foreach (var subType in animal.SubTypes)
                        DataStrings.Add(subType.Name, $"{animal.Data.DaysToProduce}/{animal.Data.DaysTillMature}/{subType.Data.ProductId}/{subType.Data.DeluxeProductId}/{animal.Data.SoundId}/0/0/0/0/0/0/0/0/{(int)animal.Data.HarvestType}/{subType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{animal.Data.FrontAndBackSpriteWidth}/{animal.Data.FrontAndBackSpriteHeight}/{animal.Data.SideSpriteWidth}/{animal.Data.SideSpriteHeight}/{animal.Data.FullnessDrain}/{animal.Data.HappinessDrain}/{animal.Data.HarvestToolName}/0/{animal.Data.BuyPrice}/{subType.Name}/");
                }
            }

            // print all added farm animals to trace
            foreach (var animalDataString in DataStrings)
                this.Monitor.Log($"{animalDataString.Key}: {animalDataString.Value}");

            // invalidate farm animal cache to add the new data strings to it
            this.Helper.Content.InvalidateCache("Data/FarmAnimals");
        }


        /*********
        ** Private Methods 
        *********/
        /// <summary>Apply the harmony patches for patching game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.FarmAnimal), new Type[] { typeof(string), typeof(long), typeof(long) }),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ConstructorTranspile)))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.FarmAnimal), new Type[] { typeof(string), typeof(long), typeof(long) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ConstructorPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.FarmAnimal), nameof(FarmAnimal.reloadData)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ReloadDataPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.Menus.PurchaseAnimalsMenu), new Type[] { typeof(List<StardewValley.Object>) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.ConstructorPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.getAnimalDescription)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.GetAnimalDescriptionPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.performHoverAction)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.PerformHoverActionPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveLeftClick)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.ReceiveLeftClickPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.draw), new Type[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.DrawPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Utility), nameof(Utility.getPurchaseAnimalStock)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(UtilityPatch), nameof(UtilityPatch.GetPurchaseAnimalStockPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.AnimatedSprite), "loadTexture"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(AnimatedSpritePatch), nameof(AnimatedSpritePatch.LoadTexturePrefix)))
            );
        }

        /// <summary>Get the sprite at the passed relative path.</summary>
        /// <param name="path">The relative (to the content pack) path for the sprite.</param>
        /// <param name="contentPack">The content pack that should contain the sprite.</param>
        /// <returns>The sprite at the passed path.</returns>
        private Texture2D GetSpriteByPath(string path, IContentPack contentPack)
        {
            if (File.Exists(Path.Combine(contentPack.DirectoryPath, path)))
            {
                return contentPack.LoadAsset<Texture2D>(path);
            }

            return null;
        }
    }
}
