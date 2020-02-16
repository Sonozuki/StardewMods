using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoreGrass.Config;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MoreGrass
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The seasons enum.</summary>
        private enum Season { Spring, Summer, Fall, Winter }

        /// <summary>The list of all loaded spring grass sprites.</summary>
        public static List<Texture2D> SpringGrassSprites { get; private set; } = new List<Texture2D>();
        /// <summary>The list of all loaded summer grass sprites.</summary>
        public static List<Texture2D> SummerGrassSprites { get; private set; } = new List<Texture2D>();
        /// <summary>The list of all loaded fall grass sprites.</summary>
        public static List<Texture2D> FallGrassSprites { get; private set; } = new List<Texture2D>();
        /// <summary>The list of all loaded winter grass sprites.</summary>
        public static List<Texture2D> WinterGrassSprites { get; private set; } = new List<Texture2D>();

        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            ApplyHarmonyPatches();
            bool loadDefaultGrass = LoadContentPacks();
            if (loadDefaultGrass)
            {
                LoadDefaultGrass();
            }
            else
            {
                Monitor.Log("Skipping loading default sprites due to content pack config.");
            }

            // once all content packs are loaded, make sure there is atleast 1 sprite in each season pool
            ValidateSpritePools();

            Monitor.Log($"A total of {SpringGrassSprites.Count} spring sprites have been loaded.");
            Monitor.Log($"A total of {SummerGrassSprites.Count} summer sprites have been loaded.");
            Monitor.Log($"A total of {FallGrassSprites.Count} fall sprites have been loaded.");
            Monitor.Log($"A total of {WinterGrassSprites.Count} winter sprites have been loaded.");
        }

        /// <summary>Apply the harmony patches for replacing game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Grass), nameof(StardewValley.TerrainFeatures.Grass.seasonUpdate)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(SeasonUpdatePreFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Grass), nameof(StardewValley.TerrainFeatures.Grass.loadSprite)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(LoadSpritePostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Grass), nameof(StardewValley.TerrainFeatures.Grass.setUpRandom)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(SetupRandomPostFix)))
            );

            // winter grass compatibility patch
            if (Helper.ModRegistry.IsLoaded("cat.wintergrass"))
            {
                Monitor.Log("Patching WinterGrass for compatibility");

                // get directory of wintergrass.dll
                string directory = Directory.GetParent(Helper.DirectoryPath).FullName;
                string path = Path.Combine(directory, "WinterGrass", "wintergrass.dll");
                if (File.Exists(path))
                {
                    var dll = Assembly.LoadFile(path);
                    var winterGrassModEntry = dll.GetExportedTypes()[0];

                    harmony.Patch(
                        original: AccessTools.Method(winterGrassModEntry, "FixGrassColor"),
                        prefix: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(WinterGrassFixGrassColorPrefix)))
                    );
                }
                else
                {
                    Monitor.Log("Couldn't disable Winter Grass, this may cause texture bugs in winter. Winter Grass is not needed with them mod as this mod enables winter grass.", LogLevel.Warn);
                }
            }
        }

        /// <summary>Load all the content packs for this mod.</summary>
        /// <returns>Whether the default grass sprites should be loaded, based from all the content packs configurations.</returns>
        private bool LoadContentPacks()
        {
            bool loadDefaultGrass = true;

            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                Monitor.Log($"Loading {contentPack.Manifest.Name}");

                var contentPackConfig = contentPack.ReadJsonFile<ContentPackConfig>("config.json");
                if (contentPackConfig != null && !contentPackConfig.EnableDefaultGrass)
                {
                    loadDefaultGrass = false;
                }

                string springDirectory = Path.Combine(contentPack.DirectoryPath, "spring");
                if (Directory.Exists(springDirectory))
                {
                    Monitor.Log("Loading spring files");
                    LoadFilesFromDirectory(springDirectory, contentPack, Season.Spring);
                }

                string summerDirectory = Path.Combine(contentPack.DirectoryPath, "summer");
                if (Directory.Exists(summerDirectory))
                {
                    Monitor.Log("Loading summer files");
                    LoadFilesFromDirectory(summerDirectory, contentPack, Season.Summer);
                }

                string fallDirectory = Path.Combine(contentPack.DirectoryPath, "fall");
                if (Directory.Exists(fallDirectory))
                {
                    Monitor.Log("Loading fall files");
                    LoadFilesFromDirectory(fallDirectory, contentPack, Season.Fall);
                }

                string winterDirectory = Path.Combine(contentPack.DirectoryPath, "winter");
                if (Directory.Exists(winterDirectory))
                {
                    Monitor.Log("Loading winter files");
                    LoadFilesFromDirectory(winterDirectory, contentPack, Season.Winter);
                }
            }

            return loadDefaultGrass;
        }

        /// <summary>Load all .png files from specified directory into the correct sprite list.</summary>
        /// <param name="directory">The absolute directory containing the .png files.</param>
        /// <param name="contentPack">The content pack currently being loaded.</param>
        /// <param name="season">The season to load the images into.</param>
        private void LoadFilesFromDirectory(string directory, IContentPack contentPack, Season season)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (!file.EndsWith(".png"))
                {
                    Monitor.Log($"Invalid file in season folder: {file}");
                    return;
                }

                string relativeDirectory = GetRelativeDirectory(directory);
                string relativePath = Path.Combine(relativeDirectory, Path.GetFileName(file));
                Texture2D grass = contentPack.LoadAsset<Texture2D>(relativePath);
                if (grass == null)
                {
                    Monitor.Log($"Failed to get grass sprite. Path expected: {relativePath}");
                }
                else
                {
                    switch (season)
                    {
                        case Season.Spring:
                            {
                                SpringGrassSprites.Add(grass);
                                break;
                            }
                        case Season.Summer:
                            {
                                SummerGrassSprites.Add(grass);
                                break;
                            }
                        case Season.Fall:
                            {
                                FallGrassSprites.Add(grass);
                                break;
                            }
                        case Season.Winter:
                            {
                                WinterGrassSprites.Add(grass);
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>Get the relative (to the mods folder) directory for loading assets.</summary>
        /// <param name="absoluteDirectory">The absolute directory to the assets folder.</param>
        /// <returns>A relative (to the mods folder) directory.</returns>
        private string GetRelativeDirectory(string absoluteDirectory)
        {
            string[] splitDirectory = absoluteDirectory.Split(Path.DirectorySeparatorChar);
            return splitDirectory[splitDirectory.Length - 1];
        }

        /// <summary>Load the default game grass into the sprite lists.</summary>
        private void LoadDefaultGrass()
        {
            LoadDefaultGrass(Season.Spring);
            LoadDefaultGrass(Season.Summer);
            LoadDefaultGrass(Season.Fall);
            LoadDefaultGrass(Season.Winter);
        }

        /// <summary>Load the default game grass sprite for the given season into it's respective pool.</summary>
        /// <param name="season">The season of grass sprites to load.</param>
        private void LoadDefaultGrass(Season season)
        {
            Texture2D grassTexture = this.Helper.Content.Load<Texture2D>(Path.Combine("TerrainFeatures", "grass"), ContentSource.GameContent);

            switch (season)
            {
                case Season.Spring:
                    {
                        Rectangle[] springGrassBounds = new Rectangle[3] { new Rectangle(0, 0, 15, 20), new Rectangle(16, 0, 15, 20), new Rectangle(30, 0, 15, 20) };
                        LoadGrassSprites(grassTexture, springGrassBounds, Season.Spring);
                        break;
                    }
                case Season.Summer:
                    {
                        Rectangle[] summerGrassBounds = new Rectangle[3] { new Rectangle(0, 21, 15, 20), new Rectangle(16, 21, 15, 20), new Rectangle(30, 21, 15, 20) };
                        LoadGrassSprites(grassTexture, summerGrassBounds, Season.Summer);
                        break;
                    }
                case Season.Fall:
                    {
                        Rectangle[] fallGrassBounds = new Rectangle[3] { new Rectangle(0, 41, 15, 20), new Rectangle(16, 41, 15, 20), new Rectangle(30, 41, 15, 20) };
                        LoadGrassSprites(grassTexture, fallGrassBounds, Season.Fall);
                        break;
                    }
                case Season.Winter:
                    {
                        Rectangle[] winterGrassBounds = new Rectangle[3] { new Rectangle(0, 81, 15, 20), new Rectangle(16, 81, 15, 20), new Rectangle(30, 81, 15, 20) };
                        LoadGrassSprites(grassTexture, winterGrassBounds, Season.Winter);
                        break;
                    }
            }
        }

        /// <summary>Load individual sprites from a sprite sheet using the specificed rectangles.</summary>
        /// <param name="grassSpriteSheet">The sprite sheet containing the grass sprites.</param>
        /// <param name="grassBounds">The list of rectangles to get the sprites from the sheet.</param>
        /// <param name="season">The season to add the sprites to.</param>
        private void LoadGrassSprites(Texture2D grassSpriteSheet, Rectangle[] grassBounds, Season season)
        {
            foreach (var grassBound in grassBounds)
            {
                // create a new Texture2D using the grassBound
                Texture2D grassSprite = new Texture2D(Game1.graphics.GraphicsDevice, grassBound.Width, grassBound.Height);
                Color[] grassData = new Color[grassBound.Width * grassBound.Height];
                grassSpriteSheet.GetData(0, grassBound, grassData, 0, grassData.Length);
                grassSprite.SetData(grassData);

                switch (season)
                {
                    case Season.Spring:
                        {
                            SpringGrassSprites.Add(grassSprite);
                            break;
                        }
                    case Season.Summer:
                        {
                            SummerGrassSprites.Add(grassSprite);
                            break;
                        }
                    case Season.Fall:
                        {
                            FallGrassSprites.Add(grassSprite);
                            break;
                        }
                    case Season.Winter:
                        {
                            WinterGrassSprites.Add(grassSprite);
                            break;
                        }
                }
            }
        }

        /// <summary>Ensure there is atleast 1 sprite in each season pool, if not, load the default sprites for that season.</summary>
        private void ValidateSpritePools()
        {
            if (SpringGrassSprites.Count == 0)
            {
                Monitor.Log("Loading default spring sprites as no loaded sprites found.");
                LoadDefaultGrass(Season.Spring);
            }

            if (SummerGrassSprites.Count == 0)
            {
                Monitor.Log("Loading default summer sprites as no loaded sprites found.");
                LoadDefaultGrass(Season.Summer);
            }

            if (FallGrassSprites.Count == 0)
            {
                Monitor.Log("Loading default fall sprites as no loaded sprites found.");
                LoadDefaultGrass(Season.Fall);
            }

            if (WinterGrassSprites.Count == 0)
            {
                Monitor.Log("Loading default winter sprites as no loaded sprites found.");
                LoadDefaultGrass(Season.Winter);
            }
        }

        /// <summary>This is code that will replace some game code, this is ran whenever the season gets updated. Used for ensuring grass doesn't get killing in winter.</summary>
        /// <param name="__instance">The current grass instance that is being patched.</param>
        /// <param name="__result">Always return false, this means the grass won't get killed.</param>
        /// <returns></returns>
        private static bool SeasonUpdatePreFix(Grass __instance, ref bool __result)
        {
            // this will ensure the grass doesn't get killed in winter
            __result = false;
            __instance.loadSprite();

            return false;
        }

        /// <summary>This is code that will run after some game code, this is ran whenever the grass sprite gets loaded. Used for setting a custom sprite.</summary>
        /// <param name="__instance">The current grass instance that is being patched.</param>
        private static void LoadSpritePostFix(Grass __instance)
        {
            FieldInfo texture = typeof(Grass).GetField("texture", BindingFlags.NonPublic | BindingFlags.Instance);

            Texture2D grassTexture = null;
            switch (Game1.currentSeason)
            {
                case "spring":
                    {
                        grassTexture = ModEntry.SpringGrassSprites[Game1.random.Next(ModEntry.SpringGrassSprites.Count)];
                        break;
                    }
                case "summer":
                    {
                        grassTexture = ModEntry.SummerGrassSprites[Game1.random.Next(ModEntry.SummerGrassSprites.Count)];
                        break;
                    }
                case "fall":
                    {
                        grassTexture = ModEntry.FallGrassSprites[Game1.random.Next(ModEntry.FallGrassSprites.Count)];
                        break;
                    }
                case "winter":
                    {
                        grassTexture = ModEntry.WinterGrassSprites[Game1.random.Next(ModEntry.WinterGrassSprites.Count)];
                        break;
                    }
            }

            texture.SetValue(__instance, new Lazy<Texture2D>(() => grassTexture ));
            __instance.grassSourceOffset.Value = 0;
        }

        /// <summary>This is code that will run after some game code, this is ran on every update loop on the grass. Used for resetting 'whichWeed' which ensures the custom sprite is drawn correctly.</summary>
        /// <param name="__instance">The current grass instance that is being patched.</param>
        private static void SetupRandomPostFix(Grass __instance)
        {
            FieldInfo whichWeed = typeof(Grass).GetField("whichWeed", BindingFlags.NonPublic | BindingFlags.Instance);
            whichWeed.SetValue(__instance, new int[4] { 0, 0, 0, 0 });
        }

        /// <summary>This is code that is ran in the Winter Grass mod, this needs to be disabled so this mod can handle textures properly.</summary>
        /// <returns>False meaning the original method will never get ran.</returns>
        private static bool WinterGrassFixGrassColorPrefix()
        {
            return false;
        }
    }
}
