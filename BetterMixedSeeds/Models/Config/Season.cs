using System.Collections.Generic;

namespace BetterMixedSeeds.Models.Config
{
    /// <summary>A wrapper for a list of crops.</summary>
    public class Season
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The list of crops that are present in this season.</summary>
        public List<Crop> Crops { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="crops">The list of crops that are present in this season.</param>
        public Season(List<Crop> crops)
        {
            Crops = crops;
        }
    }
}
