namespace SonoCore;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    /*********
    ** Accessors
    *********/
    /// <summary>The singleton instance of <see cref="ModEntry"/>.</summary>
    public static ModEntry Instance { get; private set; }


    /*********
    ** Public Methods
    *********/
    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        Instance = this;
    }
}
