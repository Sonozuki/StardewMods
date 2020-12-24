using BetterMixedSeedsConfigUpdater.Models.V2;

namespace BetterMixedSeedsConfigUpdater.Models.V3
{
    /// <summary>Metadata of a crop.</summary>
    public class Crop_V3
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the crop.</summary>
        public string Name { get; set; }

        /// <summary>Whether the crop should get added the seed list.</summary>
        public bool Enabled { get; set; }

        /// <summary>The chance the crop can drop from mixed seeds.</summary>
        public float DropChance { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="oldCrop">The <see cref="BetterMixedSeedsConfigUpdater.Models.V2.Crop_V2"/> to recreate.</param>
        public Crop_V3(Crop_V2 oldCrop)
        {
            Name = oldCrop.Name;
            Enabled = oldCrop.Enabled;
            DropChance = oldCrop.Chance;
        }
    }
}
