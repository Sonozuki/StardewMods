using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="FarmAnimal"/> class.</summary>
    internal static class FarmAnimalPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The transpile for the constructor so it doesn't call ReloadData() before the PostFix below can sort out custom animal types.</summary>
        /// <remarks>Transpile is used instead of prefix as the prefix was called before the field's default values were set.</remarks>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        internal static IEnumerable<CodeInstruction> ConstructorTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var codeInstructions = instructions.ToList();
            var skipNextInstruction = false;
            var patchCompleted = false;

            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (skipNextInstruction)
                {
                    skipNextInstruction = false;
                    continue;
                }

                var instruction = codeInstructions[i];

                if (!patchCompleted)
                {
                    var nextInstruction = codeInstructions[i + 1];

                    if (nextInstruction.operand is MethodInfo)
                    {
                        // ensure to remove both instructions so stack doesn't get corrupt
                        if (instruction.opcode == OpCodes.Ldarg_0 && nextInstruction.opcode == OpCodes.Callvirt && (nextInstruction.operand as MethodInfo).Name == "reloadData")
                        {
                            skipNextInstruction = true;
                            patchCompleted = true;
                            continue;
                        }
                    }
                }

                yield return instruction;
            }
        }

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
            // load content packs here instead of the OnSaveLoaded/OnLoadStageChanged event. this is because json assets need to be loaded first (for api object for products) (JA is loaded OnLoadStageChanged LoadStage.LoadParsed). 
            // whenever I made a OnLoadStageChanged event it would run before JAs causing api objects to not exist
            // this code is ran before the next load stage event is fired. when this code is first ran JA should be fully initialised though.
            if (!ModEntry.Instance.ContentPacksLoaded)
            {
                ModEntry.Instance.LoadContentPacks();
                ModEntry.Instance.ContentPacksLoaded = true;
            }

            string data;
            Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals").TryGetValue(__instance.type.Value, out data);
            if (data == null)
            {
                ModEntry.ModMonitor.Log($"Couldn't find farm animal datastring for animal: {__instance.type.Value}", LogLevel.Error);
                return false;
            }

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

            string animalType = __instance.type;
            if (__instance.age < __instance.ageWhenMature)
                animalType = "Baby" + (__instance.type.Value.Equals("Duck") ? "White Chicken" : __instance.type.Value);
            else if (__instance.showDifferentTextureWhenReadyForHarvest && __instance.currentProduce <= 0)
                animalType = "Sheared" + __instance.type.Value;

            __instance.Sprite = new AnimatedSprite($"Animals\\{animalType}", 0, Convert.ToInt32(strArray[16]), Convert.ToInt32(strArray[17]));
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
    }
}
