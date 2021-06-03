using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MasterFisher.Models.Converted
{
    /// <summary>Represents an area in a location that can be configured.</summary>
    public class LocationArea
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The unique name of the location area being configured.</summary>
        public string UniqueName { get; set; }

        /// <summary>The name of the location being configured.</summary>
        public string LocationName { get; set; }

        /// <summary>The area in the location being configured.</summary>
        public Rectangle Area { get; set; }

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
        public float TreasureChance { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="uniqueName">The unique name for the area being configured.</param>
        /// <param name="locationName">The name of the location being configured.</param>
        /// <param name="area">The area in the location being configured.</param>
        /// <param name="springFish">The fish that can be caught in spring, in the area.</param>
        /// <param name="summerFish">The fish that can be caught in summer, in the area.</param>
        /// <param name="fallFish">The fish that can be caught in fall, in the area.</param>
        /// <param name="winterFish">The fish that can be caught in winter, in the area.</param>
        /// <param name="treasureChance">The chance of finding treasure when fishing.</param>
        public LocationArea(string uniqueName, string locationName, Rectangle area, List<string> springFish, List<string> summerFish, List<string> fallFish, List<string> winterFish, float treasureChance)
        {
            UniqueName = uniqueName;
            LocationName = locationName;
            Area = area;
            SpringFish = springFish ?? new List<string>();
            SummerFish = summerFish ?? new List<string>();
            FallFish = fallFish ?? new List<string>();
            WinterFish = winterFish ?? new List<string>();
            TreasureChance = treasureChance;
        }
    }
}
