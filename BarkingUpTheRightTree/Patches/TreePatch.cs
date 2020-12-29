using BarkingUpTheRightTree.Tools;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BarkingUpTheRightTree.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="StardewValley.TerrainFeatures.Tree"/> class.</summary>
    internal static class TreePatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="StardewValley.TerrainFeatures.Tree.loadTexture()"/> method.</summary>
        /// <param name="__instance">The <see cref="StardewValley.TerrainFeatures.Tree"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (this depends on if the tree is custom).</returns>
        /// <remarks>This is used to intercept the tree texture loading to add the custom textures.</remarks>
        internal static bool LoadTexturePrefix(Tree __instance, ref Texture2D __result)
        {
            // if the tree type is a default one, let the original method handle it
            if (__instance.treeType < 20)
            {
                __result = null;
                return true;
            }

            if (!ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out var texture, out _, out _, out _, out _, out _, out _, out _, out _))
            {
                ModEntry.Instance.Monitor.Log($"A tree with the id: {__instance.treeType} couldn't be found.", LogLevel.Error);
                __result = null;
                return false;
            }

            __result = texture;
            return false;
        }

        /// <summary>The prefix for the <see cref="StardewValley.TerrainFeatures.Tree.shake(Microsoft.Xna.Framework.Vector3, bool, StardewValley.GameLocation)"/> method.</summary>
        /// <param name="tileLocation">The location of the tree being shaken.</param>
        /// <param name="doEvenIfStillShaking">Whether the shake action can be started if the tree is still shaking.</param>
        /// <param name="location">The location of the tree.</param>
        /// <param name="__instance">The current <see cref="StardewValley.TerrainFeatures.Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/>, meaning the original method will get ran.</returns>
        /// <remarks>This is used to drop custom debris when a custom tree is shaken.</remarks>
        internal static bool ShakePrefix(Vector2 tileLocation, bool doEvenIfStillShaking, GameLocation location, Tree __instance)
        {
            // ensure tree being shaken is a custom one
            if (__instance.treeType < 20)
                return true;

            // get private member
            var maxShake = (float)typeof(Tree).GetField("maxShake", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // ensure tree is valid for dropping shake products
            if ((maxShake == 0 || doEvenIfStillShaking) && __instance.growthStage >= 5 && !__instance.stump.Value && (Game1.IsMultiplayer || Game1.player.ForagingLevel >= 1))
            {
                // get seed and shake produce debris
                if (!ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out _, out _, out var seed, out var shakingProducts, out _, out _, out _))
                    return false;

                // handle dropping seed
                if (__instance.hasSeed)
                    Game1.createObjectDebris(seed, (int)tileLocation.X, (int)tileLocation.Y - 3, location);

                // handle dropping custom shake produce
                var daysTillNextShakeProducts = JsonConvert.DeserializeObject<int[]>(__instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillNextShakeProducts"]);
                for (int i = 0; i < daysTillNextShakeProducts.Length; i++)
                {
                    if (daysTillNextShakeProducts[i] > 0)
                        continue;

                    var seasons = shakingProducts[i].Seasons.Select(season => season?.ToLower()).ToArray();
                    if (!shakingProducts[i].Seasons.Contains(Game1.currentSeason.ToLower()))
                        continue;

                    Game1.createObjectDebris(shakingProducts[i].Product, (int)tileLocation.X, (int)tileLocation.Y - 3, ((int)tileLocation.Y + 1) * 64, location: location);
                    daysTillNextShakeProducts[i] = shakingProducts[i].DaysBetweenProduce;
                }
                __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillNextShakeProducts"] = JsonConvert.SerializeObject(daysTillNextShakeProducts);
            }

            return true;
        }

        /// <summary>The transpiler for the <see cref="StardewValley.TerrainFeatures.Tree.performToolAction(StardewValley.Tool, int, Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to stop the tree from dropping wood when hit with an axe with the <see cref="StardewValley.ShavingEnchantment"/>.<br/>This is to make it drop the custom wood debris in <see cref="BarkingUpTheRightTree.Patches.TreePatch.TickUpdatePrefix(Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation, StardewValley.TerrainFeatures.Tree)"/> patch, this was done as a <see cref="StardewValley.TerrainFeatures.Tree"/> instance can't be retrieved in a transpile but can in a prefix (and as such can't get the wood id for the tree being hit with the axe).</remarks>
        internal static IEnumerable<CodeInstruction> PerformToolActionTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var patchApplied = false;
            for (int i = 0; i < instructions.Count(); i++)
            {
                var instruction = instructions.ElementAt(i);

                // skip checking the instruction if the patch has already been applied
                if (patchApplied)
                {
                    yield return instruction;
                    continue;
                }

                // get the next two instructions (to check if it should be patched)
                var nextInstruction = instructions.ElementAt(i + 1);
                var nextNextInstruction = instructions.ElementAt(i + 2);
                if (instruction.opcode == OpCodes.Ldloc_0
                    && nextInstruction.opcode == OpCodes.Ldc_R4 && Convert.ToSingle(nextInstruction.operand) == 5
                    && nextNextInstruction.opcode == OpCodes.Div)
                {
                    // this patch will change the code: Game1.random.NextDouble() <= (double)(damage / 5f)
                    // into:                            Game1.random.NextDouble() <= (double)(1f - 5f)
                    // this will always return false which is how the functionality will be disabled (so it can be reimplemented using the custom wood type in PerformToolActionPrefix)

                    // in here, 'damage' is instruction, '5f' is nextInstruction, and the divide is 'nextNextInstruction'

                    patchApplied = true;
                    instruction.opcode = OpCodes.Ldc_R4;
                    instruction.operand = 1;
                    nextNextInstruction.opcode = OpCodes.Sub;

                    yield return instruction;
                    yield return nextInstruction;
                    yield return nextNextInstruction;

                    i += 2; // increment as the next 2 instructions have been handled
                    continue;
                }

                yield return instruction;
            }
        }

        /// <summary>The prefix for the <see cref="StardewValley.TerrainFeatures.Tree.performToolAction(StardewValley.Tool, int, Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation)"/> method.</summary>
        /// <param name="t">The tool being used.</param>
        /// <param name="tileLocation">The tile action of the tree.</param>
        /// <param name="location">The location the player is currently in.</param>
        /// <returns><see langword="true"/>, meaning the original method will get ran.</returns>
        /// <remarks>This is used to add the <see cref="BarkingUpTheRightTree.Tools.BarkRemover"/> functionality and to drop the custom wood when using an axe with the <see cref="StardewValley.ShavingEnchantment"/>.</remarks>
        internal static bool PerformToolActionPrefix(Tool t, Vector2 tileLocation, GameLocation location, Tree __instance)
        {
            if (t is Axe)
            {
                // ensure the axe has the shaving enchantment
                if (!t.hasEnchantmentOfType<ShavingEnchantment>())
                    return true;

                // get the damage of the axe (this is used when calculating the chance to see if wood should be dropped)
                var damage = 1f;
                switch (t.UpgradeLevel)
                {
                    case 1: damage = 1.25f; break;
                    case 2: damage = 1.67f; break;
                    case 3: damage = 2.5f; break;
                    case 4: damage = 5; break;
                }

                // determine if a piece of wood should be dropped
                if (Game1.random.NextDouble() <= (damage / 5f))
                {
                    // get the id of the wood to drop
                    var woodId = 388;
                    if (ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out var customWoodId, out _, out _, out _, out _, out _, out _))
                        woodId = customWoodId;

                    // spawn the debris (just copy over the game code so it functions the same)
                    var debris = new Debris(woodId, new Vector2(tileLocation.X * 64f + 32f, (tileLocation.Y - 0.5f) * 64f + 32f), new Vector2(Game1.player.getStandingX(), Game1.player.getStandingY()));
                    debris.Chunks[0].xVelocity.Value += Game1.random.Next(-10, 11) / 10f;
                    debris.chunkFinalYLevel = (int)(tileLocation.Y * 64f + 64f);
                    location.debris.Add(debris);
                }
            }
            else if (t is BarkRemover)
            {
                location.playSound("axchop", NetAudio.SoundContext.Default);
                typeof(Tree).GetMethod("shake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { tileLocation, true, location });

                // ensure tree is custom
                if (__instance.treeType < 20)
                    return true;

                // ensure tree is grown, alive (not a strump), and has bark
                if (__instance.growthStage < 5 || __instance.stump || !ModEntry.Instance.Api.GetBarkState(location.Name, tileLocation))
                    return true;

                // make sure tree has a bark product (meaning it can be debarked in the first place)
                if (!ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out _, out _, out _, out _, out _, out _, out var barkProduct) || barkProduct.Product == -1)
                    return true;

                // mark tree as barkless
                ModEntry.Instance.Api.SetBarkState(location.Name, tileLocation, false);

                // drop bark objects
                var debris = new Debris(barkProduct.Product, barkProduct.Amount, t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0.0f), t.getLastFarmerToUse().Position);
                location.debris.Add(debris);
            }

            return true;
        }

        /// <summary>The transpiler for the <see cref="StardewValley.TerrainFeatures.Tree.tickUpdate(Microsoft.Xna.Framework.GameTime, Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to stop the tree from dropping wood and sap, when cut down (not as a stump).<br/>This is to make it drop the custom wood debris in <see cref="BarkingUpTheRightTree.Patches.TreePatch.TickUpdatePrefix(Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation, StardewValley.TerrainFeatures.Tree)"/> patch, this was done as a <see cref="StardewValley.TerrainFeatures.Tree"/> instance can't be retrieved in a transpile but can in a prefix (and as such can't get the wood id for the tree being cut down).<br/>The debris type is set to 21, this is because <see cref="System.Reflection.Emit.OpCodes.Ldc_I4_S"/> only accepts an <see langword="sbyte"/> and <see cref="sbyte.MaxValue"/> isn't big enough to be outside of the game object ids range. As such 21 was used as it's an unused id and isn't an 'aliased' id (check the <see langword="switch"/> in <see cref="StardewValley.Debris(int, int, Microsoft.Xna.Framework.Vector2, Microsoft.Xna.Framework.Vector2, float)"/> constructor for the 'aliased' types).</remarks>
        internal static IEnumerable<CodeInstruction> TickUpdateTranspile(IEnumerable<CodeInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count(); i++)
            {
                var instruction = instructions.ElementAt(i);

                // if the instruction is the last one, skip checking them for groups
                if (i >= instructions.Count() - 1)
                {
                    yield return instruction;
                    continue;
                }

                // check if the instruction is for the wood id
                {
                    var nextInstruction = instructions.ElementAt(i + 1);
                    if (instruction.opcode == OpCodes.Ldarg_3
                        && nextInstruction.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(nextInstruction.operand) == 12)
                    {
                        // edit the instruction to be the correct id and return them
                        nextInstruction.operand = 21;
                        yield return instruction;
                        yield return nextInstruction;

                        i++; // increment as the next instruction has been handled
                        continue;
                    }
                }

                // check if the instruction is for the sap id
                {
                    if (instruction.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(instruction.operand) == 92)
                    {
                        // edit the instruction to be the correct id and return them
                        instruction.operand = 21;
                        yield return instruction;
                        continue;
                    }
                }

                yield return instruction;
            }
        }

        /// <summary>The prefix for the <see cref="StardewValley.TerrainFeatures.Tree.tickUpdate(Microsoft.Xna.Framework.GameTime, Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation)"/> method.</summary>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="location">The location of the tree being patched.</param>
        /// <param name="__instance">The <see cref="StardewValley.TerrainFeatures.Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/>, meaning the original method will get ran.</returns>
        /// <remarks>This is used to spawn the custom wood debris when cutting down a tree (that isn't a stump).</remarks>
        internal static bool TickUpdatePrefix(Vector2 tileLocation, GameLocation location, Tree __instance)
        {
            // run the same code as the game does to determine if the tree has fully fallen
            var falling = ((NetBool)typeof(Tree).GetField("falling", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)).Value;
            var shakeRotation = (float)typeof(Tree).GetField("shakeRotation", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var maxShake = (float)typeof(Tree).GetField("maxShake", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            var shakeRotationModifier = maxShake * maxShake;
            var newShakeRotation = shakeRotation + (__instance.shakeLeft
                ? -shakeRotationModifier
                : shakeRotationModifier);

            if (!(falling && Math.Abs(newShakeRotation) > Math.PI / 2.0))
                return true;

            // tree has just fully fallen down, spawn wood and sap debris
            if (__instance.treeType == Tree.mushroomTree) // ensure not to spawn wood for mushroom trees
                return true;

            var treeFound = ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out var wood, out var dropsSap, out var seed, out _, out _, out _, out _);
            var woodId = Debris.woodDebris;
            if (__instance.treeType >= 20 && treeFound)
                woodId = wood;

            var lastPlayerToHit = ((NetLong)typeof(Tree).GetField("lastPlayerToHit", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)).Value;
            var extraWoodCalculator = typeof(Tree).GetMethod("extraWoodCalculator", BindingFlags.NonPublic | BindingFlags.Instance);

            // drop wood
            if (woodId != -1)
                Game1.createRadialDebris(location, woodId, (int)tileLocation.X + (__instance.shakeLeft ? (-4) : 4), (int)tileLocation.Y, (int)((Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * (12 + (int)extraWoodCalculator.Invoke(__instance, new object[] { tileLocation }))), resource: true);

            // drop sap if the tree is either default or has it enabled in it's data
            if (__instance.treeType < 20 || dropsSap)
                Game1.createRadialDebris(location, 92, (int)tileLocation.X + (__instance.shakeLeft ? (-4) : 4), (int)tileLocation.Y, 5, resource: true);

            // drop tree seed
            if (Game1.getFarmer(lastPlayerToHit).getEffectiveSkillLevel(Farmer.foragingSkill) >= 1
                && Game1.random.NextDouble() < 0.75
                && treeFound)
                Game1.createRadialDebris(location, seed, (int)tileLocation.X + (__instance.shakeLeft ? (-4) : 4), (int)tileLocation.Y, 5, resource: true);

            return true;
        }

        /// <summary>The transpiler for the <see cref="StardewValley.TerrainFeatures.Tree.performTreeFall(StardewValley.Tool, int, Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to stop the tree from dropping wood and sap, when cut down (as a stump).<br/>This is to make it drop the custom wood debris in <see cref="BarkingUpTheRightTree.Patches.TreePatch.TickUpdatePrefix(Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation, StardewValley.TerrainFeatures.Tree)"/>, this was done as a <see cref="StardewValley.TerrainFeatures.Tree"/> instance can't be retrieved in a transpile but can in a prefix (and as such can't get the wood id for the tree being cut down).<br/>The debris type is set to 21, this is because <see cref="System.Reflection.Emit.OpCodes.Ldc_I4_S"/> only accepts an <see langword="sbyte"/> and <see cref="sbyte.MaxValue"/> isn't big enough to be outside of the game object ids range. As such 21 was used as it's an unused id and isn't an 'aliased' id (check the <see langword="switch"/> in <see cref="StardewValley.Debris(int, int, Microsoft.Xna.Framework.Vector2, Microsoft.Xna.Framework.Vector2, float)"/> constructor for the 'aliased' types).</remarks>
        internal static IEnumerable<CodeInstruction> PerformTreeFallTranspile(IEnumerable<CodeInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count(); i++)
            {
                var instruction = instructions.ElementAt(i);

                // if the instruction is one of the last two, skip checking them for groups
                if (i >= instructions.Count() - 2)
                {
                    yield return instruction;
                    continue;
                }

                // check if the instruction is for the wood id
                {
                    var nextInstruction = instructions.ElementAt(i + 1);
                    if (instruction.opcode == OpCodes.Ldarg_S && Convert.ToInt32(instruction.operand) == 4
                        && nextInstruction.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(nextInstruction.operand) == 12)
                    {
                        // edit the instruction to be the correct id and return them
                        nextInstruction.operand = 21;
                        yield return instruction;
                        yield return nextInstruction;

                        i++; // increment as the next instruction has been handled
                        continue;
                    }
                }

                // check if the instruction is for the sap id
                {
                    // the nextNextInstruction had to be checked as the instruction before and after the ldc.i4.s instruction weren't able to checked properly (as they used labels)
                    var nextNextInstruction = instructions.ElementAt(i + 2);
                    if (instruction.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(instruction.operand) == 92
                        && nextNextInstruction.opcode == OpCodes.Ldc_I4 && Convert.ToInt32(nextNextInstruction.operand) == 709)
                    {
                        // edit the instruction to be the correct id and return them
                        instruction.operand = 21;
                        yield return instruction;
                        continue;
                    }
                }

                yield return instruction;
            }
        }

        /// <summary>The prefix for the <see cref="StardewValley.TerrainFeatures.Tree.performTreeFall(StardewValley.Tool, int, Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation)"/> method.</summary>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="location">The location of the tree being patched.</param>
        /// <param name="__instance">The <see cref="StardewValley.TerrainFeatures.Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/>, meaning the original method will get ran.</returns>
        /// <remarks>This is used to spawn the custom wood debris when cutting down a tree (that's a stump).</remarks>
        internal static bool PerformTreeFallPrefix(Vector2 tileLocation, GameLocation location, Tree __instance)
        {
            // run the same code as the game does to determine if the tree has fully fallen
            if (!__instance.stump)
                return true;

            // stump has been cut down, spawn wood & sap debris
            var treeFound = ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out var wood, out var dropsSap, out _, out _, out _, out _, out _);
            var woodId = Debris.woodDebris;
            if (__instance.treeType >= 20 && treeFound)
                woodId = wood;

            var lastPlayerToHit = ((NetLong)typeof(Tree).GetField("lastPlayerToHit", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)).Value;

            // drop wood
            if (woodId != -1)
                Game1.createRadialDebris(location, woodId, (int)tileLocation.X, (int)tileLocation.Y, (int)((Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * 4.0), resource: true);
            if (__instance.treeType < 20 || (treeFound && dropsSap)) // drop sap if the tree is either default or has it enabled in it's data
                Game1.createRadialDebris(location, 92, (int)tileLocation.X, (int)tileLocation.Y, 1, resource: true);

            return true;
        }

        /// <summary>The prefix for the <see cref="StardewValley.TerrainFeatures.Tree.performSeedDestroy(StardewValley.Tool, Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation)"/> method.</summary>
        /// <param name="t">The tool that was used to destroy the seed.</param>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="location">The location of the tree being patched.</param>
        /// <param name="__instance">The <see cref="StardewValley.TerrainFeatures.Tree"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method so the custom tree seeds will drop the item when they are destroyed.</remarks>
        internal static bool PerformSeedDestroyPrefix(Tool t, Vector2 tileLocation, GameLocation location, Tree __instance)
        {
            var multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White));

            // ensure the player that destroyed the seed has at least level 1 foraging
            var lastPlayerToHit = (long)(NetLong)typeof(Tree).GetField("lastPlayerToHit", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var player = lastPlayerToHit != 0
                ? Game1.getFarmer(lastPlayerToHit)
                : Game1.player;
            if (player.getEffectiveSkillLevel(Farmer.foragingSkill) < 1)
                return false;

            // get the seed id
            var seedId = -1;
            if (__instance.treeType <= 3)
                seedId = 308 + __instance.treeType;
            else if (__instance.treeType == Tree.mahoganyTree)
                seedId = 292;
            else if (ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out _, out _, out var seed, out _, out _, out _, out _))
                seedId = seed;
            if (seedId == -1)
                return false;

            // spawn seed
            Game1.createMultipleObjectDebris(seedId, (int)tileLocation.X, (int)tileLocation.Y, 1, (t == null) ? Game1.player.uniqueMultiplayerID : t.getLastFarmerToUse().uniqueMultiplayerID, location);
            return false;
        }

        /// <summary>The prefix for the <see cref="StardewValley.TerrainFeatures.Tree.UpdateTapperProduct(StardewValley.Object, StardewValley.Object)"/> method.</summary>
        /// <param name="tapper_instance">The tapper object on the tree.</param>
        /// <param name="__instance">The <see cref="StardewValley.TerrainFeatures.Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (this depends on if the tree is custom).</returns>
        /// <remarks>This is used to add the custom tapper products.</remarks>
        internal static bool UpdateTapperProductPrefix(StardewValley.Object tapper_instance, Tree __instance)
        {
            // ensure tree is custom
            if (__instance.treeType < 20)
                return true;

            // get tree by data
            if (!ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out var tappedProduct, out _, out _, out _, out _, out _, out _, out _))
                return false;

            var timeMultiplier = 1f;
            if (tapper_instance != null && tapper_instance.ParentSheetIndex == 264) // half the time for a heavy tapper
                timeMultiplier = .5f;

            tapper_instance.heldObject.Value = new StardewValley.Object(tappedProduct.Product, tappedProduct.Amount);
            tapper_instance.minutesUntilReady.Value = (int)(tappedProduct.DaysBetweenProduce * 1600 * timeMultiplier);

            return false;
        }

        /// <summary>The prefix for the <see cref="StardewValley.TerrainFeatures.Tree.draw(Microsoft.Xna.Framework.Graphics.SpriteBatch, Microsoft.Xna.Framework.Vector2)"/> method.</summary>
        /// <param name="spriteBatch">The <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> to draw the tree to.</param>
        /// <param name="tileLocation">The current location of the tree.</param>
        /// <param name="__instance">THe current <see cref="StardewValley.TerrainFeatures.Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (this depends on if the tree is custom).</returns>
        /// <remarks>This is used to draw trees with the different tree sprite sheet layouts.</remarks>
        internal static bool DrawPrefix(SpriteBatch spriteBatch, Vector2 tileLocation, Tree __instance)
        {
            // ensure tree trying to be drawn is custom
            if (__instance.treeType < 20)
                return true;

            // get private members
            var shakeRotation = (float)typeof(Tree).GetField("shakeRotation", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var falling = (NetBool)typeof(Tree).GetField("falling", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var alpha = (float)typeof(Tree).GetField("alpha", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var shakeTimer = (float)typeof(Tree).GetField("shakeTimer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var leaves = (List<Leaf>)typeof(Tree).GetField("leaves", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // calculate offsets depending on the season and whether the tree has been debarked
            var stumpXOffset = 0;
            var seasonXOffset = 0;
            var treeTopYOffset = 0;

            // calculate debark offset
            var debarked = !ModEntry.Instance.Api.GetBarkState(__instance.currentLocation.Name, tileLocation);
            if (debarked)
            {
                stumpXOffset += 16;
                treeTopYOffset += 96;
            }

            // calculate season offset
            switch (Game1.currentSeason)
            {
                case "summer": seasonXOffset += 48; break;
                case "fall": seasonXOffset += 96; break;
                case "winter": seasonXOffset += 144; break;
            }

            // draw tree
            if (__instance.growthStage < 5) // tree is not fully grown
            {
                var sourceRectangle = Rectangle.Empty;

                // get the sourceRectangle for the current growth stage
                switch (__instance.growthStage)
                {
                    case 0: sourceRectangle = new Rectangle(32, 32, 16, 16); break;
                    case 1: sourceRectangle = new Rectangle(16, 32, 16, 16); break;
                    case 2: sourceRectangle = new Rectangle(0, 32, 16, 16); break;
                    default: sourceRectangle = new Rectangle(0, 0, 16, 32); break;
                }

                // draw tree
                spriteBatch.Draw(
                    texture: __instance.texture.Value,
                    position: Game1.GlobalToLocal(Game1.viewport, new Vector2((tileLocation.X * 64 + 32), (tileLocation.Y * 64 - (sourceRectangle.Height * 4 - 64) + (__instance.growthStage >= 3 ? 128 : 64)))),
                    sourceRectangle: sourceRectangle,
                    color: __instance.fertilized ? Color.HotPink : Color.White,
                    rotation: shakeRotation,
                    origin: new Vector2(8, __instance.growthStage >= 3 ? 32 : 16),
                    scale: 4,
                    effects: __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    layerDepth: __instance.growthStage == 0 ? 0.0001f : __instance.getBoundingBox(tileLocation).Bottom / 10000f
                );
            }
            else // draw fully grown tree
            {
                if (!__instance.stump || falling) // check if the tree is still alive, if so draw it's shadow
                {
                    // draw tree shadow
                    spriteBatch.Draw(
                        texture: Game1.mouseCursors,
                        position: Game1.GlobalToLocal(Game1.viewport, new Vector2((tileLocation.X * 64 - 51), (tileLocation.Y * 64 - 16))),
                        sourceRectangle: Tree.shadowSourceRect,
                        color: Color.White * (1.570796f - Math.Abs(shakeRotation)), // oddly specific number is to emulate the game (game also is this specific)
                        rotation: 0,
                        origin: Vector2.Zero,
                        scale: 4,
                        effects: __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        layerDepth: 1E-06f
                    );

                    // draw tree top
                    spriteBatch.Draw(
                        texture: __instance.texture.Value,
                        position: Game1.GlobalToLocal(Game1.viewport, new Vector2((tileLocation.X * 64 + 32), (tileLocation.Y * 64 + 64))),
                        sourceRectangle: new Rectangle(0 + seasonXOffset, 64 + treeTopYOffset, 48, 96),
                        color: Color.White * alpha,
                        rotation: shakeRotation,
                        origin: new Vector2(24, 96),
                        scale: 4,
                        effects: __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        layerDepth: (__instance.getBoundingBox(tileLocation).Bottom + 2) / 10000f - tileLocation.X / 1000000f
                    );
                }

                // draw the stump
                if (__instance.health >= 1 || !falling && __instance.health > -99)
                {
                    spriteBatch.Draw(
                        texture: __instance.texture.Value,
                        position: Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(tileLocation.X * 64 + (shakeTimer > 0 ? Math.Sin(2 * Math.PI / shakeTimer) * 3 : 0)), (float)((double)tileLocation.Y * 64 - 64))),
                        sourceRectangle: new Rectangle(16 + seasonXOffset + stumpXOffset, 0, 16, 32),
                        color: Color.White * alpha,
                        rotation: 0,
                        origin: Vector2.Zero,
                        scale: 4,
                        effects: __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        layerDepth: __instance.getBoundingBox(tileLocation).Bottom / 10000f
                    );
                }
            }

            // draw falling leaves
            foreach (var leaf in leaves)
            {
                spriteBatch.Draw(
                    texture: __instance.texture.Value,
                    position: Game1.GlobalToLocal(Game1.viewport, leaf.position),
                    sourceRectangle: new Rectangle(leaf.type % 2 * 8, 48 + leaf.type / 2 * 8, 8, 8),
                    color: Color.White,
                    rotation: leaf.rotation,
                    origin: Vector2.Zero,
                    scale: 4,
                    effects: SpriteEffects.None,
                    layerDepth: (float)(__instance.getBoundingBox(tileLocation).Bottom / 10000f + 0.00999999977648258) // oddly specific number is to replicate the game (game also is this specific)
                );
            }

            return false;
        }
    }
}
