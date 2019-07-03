using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class ModEntry : Mod
    {
        bool OldCurrent;

        public override void Entry(IModHelper helper)
        {
            this.Helper.Events.GameLoop.OneSecondUpdateTicked += Events_OneSecLoop;
            this.Helper.Events.GameLoop.SaveLoaded += Events_SaveLoaded;
        }

        private void Events_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // This sets the default, meaning if the greenhouse is already aquired, it will be true meaning it wouldn't run the code that should only be ran once every save
            OldCurrent = Game1.player.hasGreenhouse;
        }

        private void Events_OneSecLoop(object sender, OneSecondUpdateTickedEventArgs e)
        {
            // Make sure a save is loaded before tried to check if the player has a greenhouse
            if (!Context.IsWorldReady)
            {
                return;
            }

            bool NewCurrent = Game1.player.hasGreenhouse;

            if (OldCurrent != NewCurrent)
            {
                // Greenhouse now acquired code in here
            }

            // Make sure to set the old value to the current value ready for this loop to be ran again
            OldCurrent = NewCurrent;
        }
    }
}
