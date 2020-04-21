using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the StardewValley.FarmAnimal class.</summary>
    internal static class FarmAnimalPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The post fix for the constructor.</summary>
        /// <param name="type">The animal type being constructed.</param>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        internal static void ConstructorPostFix(string type, FarmAnimal __instance)
        {
            // check if the type is custom
            var animal = ModEntry.Animals.Where(animal => animal.Name == type).FirstOrDefault();
            if (animal != null)
            {
                var subType = animal.SubTypes[Game1.random.Next(animal.SubTypes.Count())];
                __instance.type.Value = subType.Name;
            }

            __instance.reloadData();
        }

        /// <summary>The prefix for the ReloadData method.</summary>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool ReloadDataPrefix(FarmAnimal __instance)
        {
            string data;
            Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals").TryGetValue(__instance.type.Value, out data);
            if (data == null)
                return false;

            string[] strArray = data.Split('/');
            __instance.daysToLay.Value = Convert.ToByte(strArray[0]);
            __instance.ageWhenMature.Value = Convert.ToByte(strArray[1]);
            __instance.defaultProduceIndex.Value = Convert.ToInt32(strArray[2]);
            __instance.deluxeProduceIndex.Value = Convert.ToInt32(strArray[3]);
            __instance.sound.Value = strArray[4].Equals("none") ? (string)null : strArray[4];
            __instance.frontBackBoundingBox.Value = new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(strArray[5]), Convert.ToInt32(strArray[6]), Convert.ToInt32(strArray[7]), Convert.ToInt32(strArray[8]));
            __instance.sidewaysBoundingBox.Value = new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(strArray[9]), Convert.ToInt32(strArray[10]), Convert.ToInt32(strArray[11]), Convert.ToInt32(strArray[12]));
            __instance.harvestType.Value = Convert.ToByte(strArray[13]);
            __instance.showDifferentTextureWhenReadyForHarvest.Value = Convert.ToBoolean(strArray[14]);
            __instance.buildingTypeILiveIn.Value = strArray[15];
            __instance.Sprite = GetAnimatedSprite(Convert.ToInt32(strArray[16]), Convert.ToInt32(strArray[17]), __instance);
            __instance.frontBackSourceRect.Value = new Microsoft.Xna.Framework.Rectangle(0, 0, Convert.ToInt32(strArray[16]), Convert.ToInt32(strArray[17]));
            __instance.sidewaysSourceRect.Value = new Microsoft.Xna.Framework.Rectangle(0, 0, Convert.ToInt32(strArray[18]), Convert.ToInt32(strArray[19]));
            __instance.fullnessDrain.Value = Convert.ToByte(strArray[20]);
            __instance.happinessDrain.Value = Convert.ToByte(strArray[21]);
            __instance.toolUsedForHarvest.Value = strArray[22].Length > 0 ? strArray[22] : "";
            __instance.meatIndex.Value = Convert.ToInt32(strArray[23]);
            __instance.price.Value = Convert.ToInt32(strArray[24]);
            
            if (!__instance.isCoopDweller())
                __instance.Sprite.textureUsesFlippedRightForLeft = true;

            return false;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Create an <see cref="AnimatedSprite"/> for the passed <see cref="FarmAnimal"/>.</summary>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        private static AnimatedSprite GetAnimatedSprite(int spriteWidth, int spriteHeight, FarmAnimal __instance)
        {
            // get sprite name
            string textureName = __instance.type.Value;
            if (__instance.isBaby())
                textureName = "Baby" + (__instance.type.Value.Equals("Duck") ? "White Chicken" : __instance.type.Value);
            else if (__instance.showDifferentTextureWhenReadyForHarvest && __instance.currentProduce <= 0)
                textureName = "Sheared" + __instance.type.Value;

            Texture2D animalSpriteSheet = null;
            // try get sprite sheet from base files
            if (File.Exists(Path.Combine(Constants.ExecutionPath, "Content", "Animals", $"{textureName}.xnb")))
            {
                animalSpriteSheet = ModEntry.ModHelper.Content.Load<Texture2D>($"Animals\\{textureName}", ContentSource.GameContent);
            }
            else
            {
                // convert Sheared to Harvestable and add a space after Baby as that's the prefix for the custom harvestable / baby sprite sheet
                textureName = textureName.Replace("Baby", "Baby ").Replace("Sheared", "Harvestable ");

                // get sprite sheet from custom sprites
                foreach (var animal in ModEntry.Animals)
                {
                    var animalSprites = animal.SubTypes
                        .Where(subType => subType.Name == __instance.type.Value)
                        .Select(subType => subType.Sprites)
                        .FirstOrDefault();

                    if (animalSprites == null)
                        continue;

                    Season season = Season.Spring;
                    switch (Game1.currentSeason)
                    {
                        case "spring":
                            season = Season.Spring;
                            break;
                        case "summer":
                            season = Season.Summer;
                            break;
                        case "fall":
                            season = Season.Fall;
                            break;
                        case "winter":
                            season = Season.Winter;
                            break;
                    }

                    animalSpriteSheet = animalSprites.GetSpriteSheet(
                        isBaby: __instance.isBaby(),
                        isHarvestable: __instance.showDifferentTextureWhenReadyForHarvest && __instance.currentProduce <= 0,
                        season: season
                    );

                    break;
                }
            }

            // manually create animated sprite so the custom sprite sheets can be used
            var animatedSprite = new AnimatedSprite();
            typeof(AnimatedSprite).GetField("spriteTexture", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(animatedSprite, animalSpriteSheet);

            animatedSprite.SpriteWidth = spriteWidth;
            animatedSprite.SpriteHeight = spriteHeight;
            animatedSprite.CurrentFrame = 0;
            return animatedSprite;
        }
    }
}
