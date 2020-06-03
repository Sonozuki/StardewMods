using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MoreTrees
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            ApplyHarmonyPatches();

            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }


        /// <summary>Apply the harmony patches.</summary>
        private void ApplyHarmonyPatches()
        {
            // TODO: apply patches
        }

        /// <summary>Invoked when the player loads a save.</summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // TODO: load content packs
        }
    }
}
