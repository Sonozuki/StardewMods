using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MasterFisher.Models.Parsed
{
    /// <summary>Represents an area in a location that can be configured.</summary>
    public class ParsedLocationArea
    {
        /*********
        ** Accessors
        *********/
        /// <summary>How the location data should be interpreted.</summary>
        public Action Action { get; set; } = Action.Add;

        /// <summary>The unique name of the location area being configured.</summary>
        public string UniqueName { get; set; }

        /// <summary>The name of the location being configured.</summary>
        public string LocationName { get; set; }

        /// <summary>The area in the location being configured.</summary>
        public Rectangle Area { get; set; } // TODO: set this to nullable, this wasn't deserialising correctly, SMAPI is getting an update for this

        /// <summary>The fish that can be caught in spring in the area.</summary>
        public List<string> SpringFish { get; set; }

        /// <summary>The fish that can be caught in summer in the area.</summary>
        public List<string> SummerFish { get; set; }

        /// <summary>The fish that can be caught in fall in the area.</summary>
        public List<string> FallFish { get; set; }

        /// <summary>The fish that can be caught in winter in the area.</summary>
        public List<string> WinterFish { get; set; }

        // TODO: add fishable objects property

        /// <summary>The chance of finding treasure when fishing in the area.</summary>
        public float? TreasureChance { get; set; }
    }
}
