using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMod2
{
    class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            this.Helper.Events.GameLoop.DayStarted += Events_DayStarted;
        }
        
        private void Events_DayStarted(object sender, DayStartedEventArgs e)
        {
            int[,] locMonsters = new int[8, 2] { { 3, 37 }, { 4, 22 }, { 13, 20 }, { 17, 14 }, { 17, 27 }, { 22, 21 }, { 28, 21 }, { 31, 19 } };

            int locN = 0;
            IList<NPC> characters = Game1.getLocationFromName("Sewer").characters;

            for (int i = 0; i < locMonsters.Length / 2; i++)
            {
                int s1 = locMonsters[i, 0];
                int s2 = locMonsters[i, 1];
                Monitor.Log($"{Game1.getLocationFromName("Sewer").Name} Location {locN}: {{{s1},{s2}}}", LogLevel.Info);
                Vector2 posit = new Vector2(s1, s2);
                //Monitor.Log($"{posit.ToString()}");
                GreenSlime monster = new GreenSlime(posit * 64, 121);
                characters.Add((NPC)monster);
                Monitor.Log($"{monster.Name} added at {Game1.getLocationFromName("Sewer").Name} {{{s1},{s2}}}");
                locN++;
            }
        }
    }


}
