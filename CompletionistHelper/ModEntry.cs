using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace CompletionistHelper
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            this.Helper.Events.Input.ButtonPressed += Events_ButtonPressed;
        }

        void Events_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Check if the button pressed is the menu button in config
            if (e.Button == SButton.J)
            {
                // Toggle the completionist menu
                if (Game1.activeClickableMenu is CompletionistMenu)
                {
                    // Play the default sound when closing a menu
                    Game1.playSound("bigDeSelect");
                    Game1.activeClickableMenu = null;
                }
                else
                {
                    // Open the CompletionistMenu
                    OpenHelper();
                }
            }
        }

        void OpenHelper()
        {
            this.Monitor.Log("Attempting to open HelperMenu", LogLevel.Trace);

            CompletionistMenu completionistMenu = new CompletionistMenu();

            if (Game1.activeClickableMenu == null)
            {
                Game1.activeClickableMenu = completionistMenu;
                this.Monitor.Log("HelperMenu opened successfully", LogLevel.Trace);
            }
            else
            {
                this.Monitor.Log("HelperMenu failed to open: Menu already open", LogLevel.Trace);
            }
        }
    }
}
