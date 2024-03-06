namespace MoreGrass;

/// <summary>Contains modData key constants.</summary>
public class ModDataConstants
{
    /*********
    ** Constants
    *********/
    /// <summary>The modData for storing the x offset (in the atlas) for grass.</summary>
    /// <remarks>This is the base for offsets, meaning 0 - 3 will be appended to the end (as grass can have up to 4 sprites per object).</remarks>
    public const string GrassOffsetXBase = $"Sonozuki.MoreGrass/GrassOffsetX";

    /// <summary>The modData for storing the y offset (in the atlas) for grass.</summary>
    /// <remarks>This is the base for offsets, meaning 0 - 3 will be appended to the end (as grass can have up to 4 sprites per object).</remarks>
    public const string GrassOffsetYBase = $"Sonozuki.MoreGrass/GrassOffsetY";
}
