namespace FarmAnimalVarietyRedux.Models.BfavSaveData
{
    /// <summary>Represents the Bfav 'FarmAnimal' class responsible for containing all the data that's saved for a custom animal.</summary>
    public class BfavAnimal
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the custom animal.</summary>
        public long Id { get; set; }

        /// <summary>The custom type of the animal.</summary>
        public BfavAnimalType TypeLog { get; set; }
    }
}
