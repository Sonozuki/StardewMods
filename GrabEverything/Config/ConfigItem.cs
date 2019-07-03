using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrabEverything.Config
{
    public class ConfigItem
    {
        // add bool for use tool for the affect, and another option for the key
        public bool Enabled { get; set; }
        public int Radius { get; set; }
        public bool RequiresTool { get; set; }
        public Dictionary<string, bool> RequiredTools { get; set; } = new Dictionary<string, bool> { { "Hoe", false }, { "Watering Can", false }, { "Axe", false }, { "Fishing Rod", false }, { "Pickaxe", false }, { "Scythe", false }, { "Sword", false }, { "Club", false }, { "Dagger", false }, { "Slingshot", false }, { "Milk Pail", false }, { "Shears", false } };

    // requiredT
    public ConfigItem(bool enabled = false, int radius = 3, bool requiresTool = true )
        {
            Enabled = enabled;
            Radius = radius;
            RequiresTool = requiresTool;
        }
    }
}
