﻿using FarmAnimalVarietyRedux.Models;
using FarmAnimalVarietyRedux.Models.Converted;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="AnimalHouse"/> class.</summary>
    internal class AnimalHousePatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="AnimalHouse.resetSharedState"/> method.</summary>
        /// <param name="__instance">The current <see cref="AnimalHouse"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to add the custom incubator recipes.</remarks>
        internal static bool ResetSharedStatePrefix(AnimalHouse __instance)
        {
            __instance.resetPositionsOfAllAnimals();

            foreach (var @object in __instance.Objects.Values)
            {
                // ensure object is an incubator
                if (!@object.bigCraftable || !@object.Name.Contains("Incubator") || @object.heldObject.Value == null || @object.minutesUntilReady > 0)
                    continue;

                // ensure there is space in the animal house to hatch the animal
                if (!__instance.isFull())
                {
                    var whatHatched = "??";
                    foreach (var incubatorRecipe in ModEntry.Instance.CustomIncubatorRecipes)
                        if (incubatorRecipe.InputId == @object.heldObject.Value.ParentSheetIndex)
                        {
                            var animalName = ModEntry.Instance.Api.GetAnimalByInternalName(incubatorRecipe.InternalAnimalName)?.Name
                                ?? ModEntry.Instance.Api.GetAnimalSubtypeByInternalName(incubatorRecipe.InternalAnimalName)?.Name;

                            if (animalName != null)
                                whatHatched = $"A new baby {animalName.ToLower()} hatched!";
                        }
                    if (whatHatched == "??")
                    {
                        // if there was no valid recipe, then just remove the object from the incubator (this is the case when the player puts an egg in the incubator then removes the mod that adds that recipe)
                        @object.heldObject.Value = null;
                        return false;
                    }

                    // run hatched event
                    __instance.currentEvent = new Event($"none/-1000 -1000/farmer 2 9 0/pause 250/message \"{whatHatched}\"/pause 500/animalNaming/pause 500/end");
                    break;
                }
                
                // tell player building is full if they haven't already been told
                if (!__instance.hasShownIncubatorBuildingFullMessage)
                {
                    __instance.hasShownIncubatorBuildingFullMessage = true;
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_HouseFull"));
                }
            }

            // call the base resetSharedState method
            // this approach isn't ideal but when using regular reflection and invoking the MethodInfo directly, it would call this patch (instead of the base method) resulting in a stack overflow
            // https://stackoverflow.com/questions/4357729/use-reflection-to-invoke-an-overridden-base-method/14415506#14415506
            var baseMethod = typeof(GameLocation).GetMethod("resetSharedState", BindingFlags.NonPublic | BindingFlags.Instance);
            var functionPointer = baseMethod.MethodHandle.GetFunctionPointer();
            var function = (System.Action)Activator.CreateInstance(typeof(System.Action), __instance, functionPointer);
            function.Invoke();
            return false;
        }

        /// <summary>The prefix for the <see cref="AnimalHouse.addNewHatchedAnimal"/> method.</summary>
        /// <param name="name">The name of the animal.</param>
        /// <param name="__instance">The current <see cref="AnimalHouse"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to add the custom incubator recipes.</remarks>
        internal static bool AddNewHatchedAnimalPrefix(string name, AnimalHouse __instance)
        {
            var foundIncubator = false;

            foreach (var @object in __instance.Objects.Values)
            {
                // ensure object is an incubator
                if (!@object.bigCraftable || !@object.Name.Contains("Incubator") || @object.heldObject.Value == null || @object.minutesUntilReady > 0 || __instance.isFull())
                    continue;

                var incubatorRecipes = new List<IncubatorRecipe>();
                if (@object.Name == "Incubator")
                    incubatorRecipes = ModEntry.Instance.CustomIncubatorRecipes.Where(incubatorRecipe => incubatorRecipe.IncubatorType.HasFlag(IncubatorType.Regular)).ToList();
                else if (@object.Name == "Ostrich Incubator")
                    incubatorRecipes = ModEntry.Instance.CustomIncubatorRecipes.Where(incubatorRecipe => incubatorRecipe.IncubatorType.HasFlag(IncubatorType.Ostrich)).ToList();

                // try to find the recipe that has the corresponding item
                var recipe = incubatorRecipes.FirstOrDefault(IncubatorRecipe => IncubatorRecipe.InputId == @object.heldObject.Value.ParentSheetIndex);
                if (recipe == null)
                {
                    ModEntry.Instance.Monitor.Log($"Couldn't find a recipe for the {@object.Name} that has an input id item of {@object.heldObject.Value.ParentSheetIndex}, emptying incubator");
                    @object.heldObject.Value = null;
                    @object.ParentSheetIndex = 101;
                    continue;
                }

                // ensure animal exists
                var isSubtype = false;
                var type = recipe.InternalAnimalName;
                var animal = ModEntry.Instance.Api.GetAnimalByInternalName(type);
                if (animal == null)
                {
                    isSubtype = true;
                    animal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(type);
                }
                if (animal == null)
                {
                    ModEntry.Instance.Monitor.Log($"Couldn't find an animal or animal type with the name of {type}, emptying incubator");
                    @object.heldObject.Value = null;
                    @object.ParentSheetIndex = 101;
                    continue;
                }

                // pick a random animal subtype that is valid and should be dropped from the recipe
                if (!isSubtype)
                {
                    var validSubtypes = animal.Subtypes.Where(subtype => subtype.IsIncubatable);
                    if (validSubtypes.Count() == 0) // ensure there was atleast one incububatable subtype, otherwise, just drop any subtype
                        validSubtypes = animal.Subtypes;

                    type = validSubtypes.ElementAt(Game1.random.Next(validSubtypes.Count())).InternalName;
                }

                var farmAnimal = CreateAnimal(name, type, __instance);
                
                // reset the incubator sprite
                @object.heldObject.Value = null;
                @object.ParentSheetIndex = 101;

                if (@object.Name == "Ostrich Incubator")
                    @object.ParentSheetIndex = 254;

                foundIncubator = true;
                break;
            }

            // if no incubator could be found, then create the animal based on the event instead (I *think* this means the animal wasn't created in an incubator, but was born by another animal (need to check to make sure))
            if (!foundIncubator && Game1.farmEvent != null && Game1.farmEvent is QuestionEvent questionEvent)
            {
                var farmAnimal = CreateAnimal(name, questionEvent.animal.type.Value, __instance);
                farmAnimal.parentId.Value = questionEvent.animal.myID;
                questionEvent.forceProceed = true;
            }

            Game1.exitActiveMenu();
            return false;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Creates an animal.</summary>
        /// <param name="name">The name of the animal.</param>
        /// <param name="type">The type of the animal.</param>
        /// <param name="__instance">The current <see cref="AnimalHouse"/> instance being patched.</param>
        /// <returns>The created <see cref="FarmAnimal"/>.</returns>
        private static FarmAnimal CreateAnimal(string name, string type, AnimalHouse __instance)
        {
            var multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            var animal = new FarmAnimal(type, multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
            animal.Name = name;
            animal.displayName = name;
            animal.home = __instance.getBuilding();
            animal.homeLocation.Value = new Vector2(animal.home.tileX, animal.home.tileY);
            animal.setRandomPosition(animal.home.indoors);

            var animalHome = animal.home.indoors.Value as AnimalHouse;
            animalHome.Animals.Add(animal.myID, animal);
            animalHome.animalsThatLiveHere.Add(animal.myID);

            return animal;
        }

    }
}
