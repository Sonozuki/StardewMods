using System.Collections.Generic;

namespace BetterMixedSeedsConfigUpdater.Models.V2
{
    /// <summary>A wrapper for a list of crops.</summary>
    public class Season_V2
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The list of crops that are present in this season.</summary>
        public List<Crop_V2> Crops { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="crops">The list of crops that will be added to the season.</param>
        public Season_V2(List<Crop_V2> crops)
        {
            Crops = crops;
        }
    }
}
