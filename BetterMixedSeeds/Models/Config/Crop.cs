﻿namespace BetterMixedSeeds.Models.Config
{
    /// <summary>Metadata of a crop.</summary>
    public class Crop
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
        /// <param name="name">The name of the crop.</param>
        /// <param name="enabled">Whether the crop should get added the seed list.</param>
        /// <param name="dropChance">The chance the crop can drop from mixed seeds.</param>
        public Crop(string name, bool enabled, float dropChance)
        {
            Name = name;
            Enabled = enabled;
            DropChance = dropChance;
        }
    }
}
