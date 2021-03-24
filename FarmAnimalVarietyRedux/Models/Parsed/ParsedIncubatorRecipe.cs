using FarmAnimalVarietyRedux.Models.Converted;

namespace FarmAnimalVarietyRedux.Models.Parsed
{
    /// <summary>Represents an incubator recipe.</summary>
    /// <remarks>This is a version of <see cref="IncubatorRecipe"/> that has <see cref="IncubatorRecipe.InputId"/> as <see langword="string"/>.<br/>The reason this is done is so content packs can have tokens in place of the ids to call mod APIs to get the id (so JsonAsset items can be used for example).</remarks>
    public class ParsedIncubatorRecipe
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The type of incubator the recipe will apply to.</summary>
        public IncubatorType IncubatorType { get; set; } = IncubatorType.Regular;

        /// <summary>The id of the input item.</summary>
        public string InputId { get; set; } = "-1";

        /// <summary>The chance this recipe will get picked compared to others that have the same <see cref="InputId"/>.</summary>
        public float Chance { get; set; } = 1;

        /// <summary>The number of minutes it takes for the incubator to finish.</summary>
        public int MinutesTillDone { get; set; } = 9000;

        /// <summary>The internal name of the animal that will get created.</summary>
        public string InternalAnimalName { get; set; }
    }
}
