using System.Collections.Generic;

namespace MoreTrees.Models
{
    /// <summary>Represents a tree content.json file.</summary>
    public class TreeData
    {
        /// <summary>The item the tree drops when using a tap on it.</summary>
        public TappingProduct TappingProduct { get; set; }

        /// <summary>The item the tree drops when it gets cut down.</summary>
        public string WoodProduct { get; set; }

        /// <summary>The item to plant to grow it.</summary>
        public string Seed { get; set; }

        /// <summary>The tree will only get loaded if one of the listed mods is present.</summary>
        public List<string> IncludeIfModIsPresent { get; set; }

        /// <summary>The tree will only get loaded if none of the listed mods are present.</summary>
        public List<string> ExcludeIfModIsPresent { get; set; }

        /// <summary>Whether this tree required the user to have the extended mode of MoreTrees.</summary>
        public bool RequiresExtendedMode { get; set; }

        /// <summary>The item the tree drops when using the 'Barn Remover' tool on it.</summary>
        public string BarkProduct { get; set; }

        /// <summary>The items the tree can drop whenever it's shaked.</summary>
        public List<ShakeProduct> ShakingProduct { get; set; }
    }
}
