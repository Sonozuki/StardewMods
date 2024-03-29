﻿namespace MoreGrass.Patches;

/// <summary>Contains patches for patching code in the WinterGrass mod.</summary>
internal class WinterGrassPatch
{
    /*********
    ** Internal Methods
    *********/
    /// <summary>The prefix for the WinterGrass.Mod.FixGrassColor() method.</summary>
    /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
    /// <remarks>This is used to disable the WinterGrass mod so this mod can handle the textures properly.</remarks>
    internal static bool FixGrassColorPrefix() => false;
}
