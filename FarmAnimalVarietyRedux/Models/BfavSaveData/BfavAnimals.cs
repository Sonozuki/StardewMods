using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.Models.BfavSaveData
{
    /// <summary>Represents the Bfav 'FarmAnimals' class responsible for containing all the data that's saved, used to keep track of custom animals.</summary>
    public class BfavAnimals
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The Bfav saved animal data.</summary>
        public List<BfavAnimal> Animals { get; set; }
    }
}
