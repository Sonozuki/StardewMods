namespace BfavToFavrModConverter.Bfav
{
    /// <summary>Represents the sprite paths of an animal subtype in BFAV's 'content.json' file.</summary>
    public class BfavAnimalTypeSprites
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The path to the adult sprite sheet.</summary>
        public string Adult { get; set; }

        /// <summary>The path to the baby sprite sheet.</summary>
        public string Baby { get; set; }

        /// <summary>The path to the ready to harvest sprite sheet.</summary>
        public string ReadyForHarvest { get; set; }
    }
}
