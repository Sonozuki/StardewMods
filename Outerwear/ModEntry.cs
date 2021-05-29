using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Outerwear.Models.Converted;
using Outerwear.Models.Parsed;
using Outerwear.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace Outerwear
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors
        *********/
        /// <summary>All the loaded outerwear data.</summary>
        public List<OuterwearData> OuterwearData { get; } = new List<OuterwearData>();

        /// <summary>Provides basic outerwear apis.</summary>
        public IApi Api { get; private set; }

        /// <summary>The sprite that is drawn in the outerwear slot when no outerwear item is in it.</summary>
        public Texture2D OuterwearSlotPlaceholder { get; private set; }

        /// <summary>The singleton instance of <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Api = new Api();

            var outerwearSlotPlaceholderPath = Path.Combine("assets", "OuterwearPlaceholder.png");
            if (!File.Exists(Path.Combine(this.Helper.DirectoryPath, outerwearSlotPlaceholderPath)))
            {
                this.Monitor.Log($"No asset could be found at: \"{outerwearSlotPlaceholderPath}\", please try reinstalling the mod.", LogLevel.Error);
                return;
            }
            OuterwearSlotPlaceholder = this.Helper.Content.Load<Texture2D>(outerwearSlotPlaceholderPath);

            ApplyHarmonyPatches();

            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            this.Helper.Events.GameLoop.UpdateTicked += (sender, e) => OuterwearEffectsApplier.Update();
        }

        /// <inheritdoc/>
        public override object GetApi() => Api;


        /*********
        ** Private Methods
        *********/
        /// <summary>Invoked when a save is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            OuterwearData.Clear();

            LoadContentPacks();
        }

        /// <summary>Applies the harmony patches for patching game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new harmony instance for patching game code
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.draw), new Type[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FarmerRendererPatch), nameof(FarmerRendererPatch.DrawPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(InventoryPage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.ConstructorPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.performHoverAction)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.PerformHoverActionPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.receiveLeftClick)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.ReceiveLeftClickPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.DrawPostFix)))
            );
        }

        /// <summary>Loads all the content packs.</summary>
        private void LoadContentPacks()
        {
            foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
            {
                this.Monitor.Log($"Loading {contentPack.Manifest.Name}", LogLevel.Info);

                // try loading content file
                if (!contentPack.HasFile("content.json"))
                {
                    this.Monitor.Log("\"content.json\" couldn't be found, skipping", LogLevel.Error);
                    continue;
                }

                var parsedOverwearDatas = contentPack.LoadAsset<List<ParsedOuterwearData>>("content.json");
                foreach (var parsedOverwearData in parsedOverwearDatas)
                {
                    // try loading asset file
                    if (!contentPack.HasFile(Path.Combine(parsedOverwearData.Asset)))
                    {
                        this.Monitor.Log($"Specified asset: \"{parsedOverwearData.Asset}\" couldn't be found, skipping", LogLevel.Error);
                        continue;
                    }

                    var equippedtexture = contentPack.LoadAsset<Texture2D>(parsedOverwearData.Asset);
                    OuterwearData.Add(new OuterwearData(Utilities.ResolveToken(parsedOverwearData.ObjectId), parsedOverwearData.Type, parsedOverwearData.Effects, equippedtexture));
                }
            }
        }
    }
}
