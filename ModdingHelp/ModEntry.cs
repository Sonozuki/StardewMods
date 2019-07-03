using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdingHelp
{
    class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
        }

        private void Events_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ItemGrabMenu)
            {
                Game1.activeClickableMenu.emergencyShutDown();
            }
        }
    }
}
