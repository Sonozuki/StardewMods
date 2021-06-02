namespace MasterFisher.Models.Parsed
{
    /// <summary>Represents a category a fish can be in.</summary>
    public class ParsedFishCategory
    {
        /*********
        ** Accessors
        *********/
        /// <summary>How the category data should be interpreted.</summary>
        public Action Action { get; set; } = Action.Add;

        /// <summary>The name of the category.</summary>
        public string Name { get; set; }

        /// <summary>The maximum number of fish allowed in a pond in the category.</summary>
        public int? FishPondCap { get; set; }

        /// <summary>The path to the sprite to use in the fishing mini game when a fish in this category is hooked.</summary>
        public string MinigameSprite { get; set; }
    }
}
