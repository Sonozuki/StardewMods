using System.Collections.Generic;

namespace BfavToFavrModConverter.Bfav
{
    /// <summary>Represents an in animal in BFAV's 'content.json' file.</summary>
    public class BfavCategory
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The method of adding the animal (Add, Update).</summary>
        public string Action { get; set; }

        /// <summary>The name of the animal.</summary>
        public string Category { get; set; }

        /// <summary>The subtypes of the animal.</summary>
        public List<BfavAnimalType> Types { get; set; }

        /// <summary>The buildings the animal can live in.</summary>
        public List<string> Buildings { get; set; }

        /// <summary>The animal shop data of the animal.</summary>
        public BfavAnimalShop AnimalShop { get; set; }
    }
}
