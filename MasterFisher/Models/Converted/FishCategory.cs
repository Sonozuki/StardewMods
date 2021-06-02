namespace MasterFisher.Models.Converted
{
    /// <summary>Represents a category a fish can be in.</summary>
    public class FishCategory
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the category.</summary>
        public string Name { get; set; }

        /// <summary>The maximum number of fish allowed in a pond in the category.</summary>
        public int FishPondCap { get; set; }

        /// <summary>The path to the sprite to use in the fishing mini game when a fish in this category is hooked.</summary>
        public string MinigameSprite { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="name">The name of the category.</param>
        /// <param name="fishPondCap">The maximum number of fish allowed in a pond in the category.</param>
        /// <param name="minigameSprite">The path to the sprite to use in the fishing mini game when a fish in this category is hooked.</param>
        public FishCategory(string name, int fishPondCap, string minigameSprite)
        {
            Name = name;
            FishPondCap = fishPondCap;
            MinigameSprite = minigameSprite;
        }
    }
}
