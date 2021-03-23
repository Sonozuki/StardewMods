namespace FarmAnimalVarietyRedux.Models.BfavSaveData
{
    /// <summary>Represents the Bfav 'TypeLog' class responsible for storing the type the animal is saved as and the custom type of it.</summary>
    public class BfavAnimalType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The custom type of the animal.</summary>
        public string Current { get; set; }

        /// <summary>The type the animal is saved as.</summary>
        public string Saved { get; set; }
    }
}
