using StardewModdingAPI;

namespace SatoCore
{
    /// <summary>Represents the base of a mod that depends on SatoCore.</summary>
    public abstract class ModBase : Mod
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public sealed override void Entry(IModHelper helper) { }
    }
}
