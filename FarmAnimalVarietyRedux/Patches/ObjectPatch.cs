using FarmAnimalVarietyRedux.Models;
using Harmony;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Object"/> class.</summary>
    internal class ObjectPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="Object.isForage(GameLocation)"/> method.</summary>
        /// <param name="location">The location of the object.</param>
        /// <param name="__instance">The <see cref="Object"/> instance being patched.</param>
        /// <param name="__result">The return value of the original method.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This is used so custom forage produce is recognised as such.</remarks>
        internal static bool IsForagePrefix(GameLocation location, Object __instance, ref bool __result)
        {
            if (__instance.Category == -79 || __instance.Category == -81 || __instance.Category == -80 || __instance.Category == -75 || location is Beach)
            {
                __result = true;
                return false;
            }

            // check all animals to see if they have the object as a forage produce
            foreach (var animal in ModEntry.Instance.CustomAnimals)
                foreach (var animalSubtype in animal.Subtypes)
                    if (animalSubtype.Produce.Any(produce => produce.HarvestType == HarvestType.Forage && (produce.DefaultProductId == __instance.ParentSheetIndex || produce.UpgradedProductId == __instance.ParentSheetIndex)))
                    {
                        __result = true;
                        return false;
                    }

            __result = false;
            return false;
        }

        /// <summary>The prefix for the <see cref="Object.isAnimalProduct()"/> method.</summary>
        /// <param name="__instance">The <see cref="Object"/> instance being patched.</param>
        /// <param name="__result">The return value of the original method.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This is used so custom produce is recognised as such.</remarks>
        internal static bool IsAnimalProductPrefix(Object __instance, ref bool __result)
        {
            if (__instance.Category == -18 || __instance.Category == -5 || __instance.Category == -6)
            {
                __result = true;
                return false;
            }

            // check all animals to see if they have the object as a produce
            foreach (var animal in ModEntry.Instance.CustomAnimals)
                foreach (var animalSubtype in animal.Subtypes)
                    if (animalSubtype.Produce.Any(produce => produce.DefaultProductId == __instance.ParentSheetIndex || produce.UpgradedProductId == __instance.ParentSheetIndex))
                    {
                        __result = true;
                        return false;
                    }

            __result = false;
            return false;
        }

        /// <summary>The post fix for the <see cref="Object.getOne()"/> method.</summary>
        /// <param name="__instance">The <see cref="Object"/> instance being patched.</param>
        /// <param name="__result">The return value of the original method.</param>
        /// <remarks>This is used to retain the stack size of the object if it was produced by an animal.</remarks>
        internal static void GetOnePostFix(Object __instance, ref Item __result)
        {
            // check if the object has the required flag
            if (!__instance.modData.ContainsKey($"{ModEntry.Instance.ModManifest.UniqueID}/producedItem"))
                return;

            // update the stack size of the returned item
            __result.Stack = __instance.Stack;
            __instance.modData.Remove($"{ModEntry.Instance.ModManifest.UniqueID}/producedItem");
        }

        /// <summary>The transpiler for the <see cref="Object.performObjectDropInAction(Item, bool, Farmer)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to disable the incubator logic so it can be handled separately in <see cref="PerformObjectDropInActionPostFix(Item, bool, Farmer, Object, ref bool)"/> using the custom recipes.</remarks>
        internal static IEnumerable<CodeInstruction> PerformObjectDropInActionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "Incubator"
                    || instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "Ostrich Incubator")
                    instruction.operand = "人生を得る"; // just change to a string that wouldn't ever be true, meaning the base game logic won't ever get ran

                yield return instruction;
            }
        }

        /// <summary>The post fix for the <see cref="Object.performObjectDropInAction(Item, bool, Farmer)"/> method.</summary>
        /// <param name="dropInItem">The item being dropped into the object.</param>
        /// <param name="probe">Whether the ability for the item being able to be dropped in is just being checked (meaning the actual drop in action shouldn't happen).</param>
        /// <param name="who">The player who is performing the action.</param>
        /// <param name="__instance">The current <see cref="Object"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <remarks>This is used to add the custom incubator recipes.</remarks>
        internal static void PerformObjectDropInActionPostFix(Item dropInItem, bool probe, Farmer who, Object __instance, ref bool __result)
        {
            // ensure the object attempted to be dropped into is an incubator
            if (__instance.Name != "Incubator" && __instance.Name != "Ostrich Incubator")
                return;

            // ensure incubator is empty and player is holding an item
            if (__instance.heldObject.Value != null || dropInItem == null)
                return;
            var dropInObject = dropInItem.getOne() as Object;

            // do logic for regular incubator
            if (__instance.Name == "Incubator")
            {
                // ensure the object is an incubator recipe
                var recipe = ModEntry.Instance.CustomIncubatorRecipes.FirstOrDefault(incubatorRecipe => incubatorRecipe.IncubatorType.HasFlag(IncubatorType.Regular) && incubatorRecipe.InputId == dropInObject.ParentSheetIndex);
                if (recipe == null)
                    return;

                // add the item to the incubator
                if (!probe)
                {
                    __instance.heldObject.Value = dropInObject;
                    who.currentLocation.playSound("coin");
                    __instance.MinutesUntilReady = recipe.MinutesTillDone;

                    // half production time if the player has the coop master profession
                    if (who.professions.Contains(Farmer.butcher)) // for some reason the CoopMaster profession constant is called Butcher
                        __instance.MinutesUntilReady /= 2;

                    // change the incubator sprite based on if the dropped in item is a chicken egg
                    if (dropInObject.ParentSheetIndex == 180 || dropInObject.ParentSheetIndex == 182 || dropInObject.ParentSheetIndex == 305)
                        __instance.ParentSheetIndex += 2;
                    else
                        __instance.ParentSheetIndex++;

                    // reset the building full message flag
                    if (who?.currentLocation != null && who.currentLocation is AnimalHouse animalHouse)
                        animalHouse.hasShownIncubatorBuildingFullMessage = false;
                }

                __result = true;
                return;
            }

            // do logic for ostrich incubator
            else if (__instance.Name == "Ostrich Incubator")
            {
                // ensure the object is an incubator recipe
                var recipe = ModEntry.Instance.CustomIncubatorRecipes.FirstOrDefault(incubatorRecipe => incubatorRecipe.IncubatorType.HasFlag(IncubatorType.Ostrich) && incubatorRecipe.InputId == dropInObject.ParentSheetIndex);
                if (recipe == null)
                    return;

                // add the item to the incubator
                if (!probe)
                {
                    __instance.heldObject.Value = dropInObject;
                    who.currentLocation.playSound("coin");
                    __instance.MinutesUntilReady = recipe.MinutesTillDone;

                    // half production time if the player has the coop master profession (as the game applies the effect the this incubator too, even though it doesn't go in the coop)
                    if (who.professions.Contains(Farmer.butcher)) // for some reason the CoopMaster profession constant is called Butcher
                        __instance.MinutesUntilReady /= 2;

                    // change the incubator sprite
                    __instance.ParentSheetIndex++;

                    // reset the building full message flag
                    if (who?.currentLocation != null && who.currentLocation is AnimalHouse animalHouse)
                        animalHouse.hasShownIncubatorBuildingFullMessage = false;
                }

                __result = true;
                return;
            }
        }
    }
}
