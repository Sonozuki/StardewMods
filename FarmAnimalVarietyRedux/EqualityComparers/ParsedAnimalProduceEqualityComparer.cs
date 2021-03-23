using FarmAnimalVarietyRedux.Models.Parsed;
using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.EqualityComparers
{
    /// <summary>Defines how two <see cref="ParsedAnimalProduce"/>s should be compared.</summary>
    /// <remarks>This only uses the <see cref="ParsedAnimalProduce.DefaultProductId"/> and <see cref="ParsedAnimalProduce.UpgradedProductId"/> and disregards every other property such as amount, etc.</remarks>
    internal class ParsedAnimalProduceEqualityComparer : IEqualityComparer<ParsedAnimalProduce>
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public bool Equals(ParsedAnimalProduce x, ParsedAnimalProduce y) => x?.DefaultProductId == y?.DefaultProductId && x?.UpgradedProductId == y?.UpgradedProductId;

        /// <inheritdoc/>
        public int GetHashCode(ParsedAnimalProduce obj) => (obj?.DefaultProductId, obj?.UpgradedProductId).GetHashCode();
    }
}
