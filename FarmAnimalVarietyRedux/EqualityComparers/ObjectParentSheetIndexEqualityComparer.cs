using StardewValley;
using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.EqualityComparers
{
    /// <summary>Defines how two <see cref="Object"/>s should be compared.</summary>
    /// <remarks>This only uses the <see cref="Item.ParentSheetIndex"/> and disregards every other property such as stack size, etc.</remarks>
    internal class ObjectParentSheetIndexEqualityComparer : IEqualityComparer<Object>
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public bool Equals(Object x, Object y) => x?.ParentSheetIndex == y?.ParentSheetIndex;

        /// <inheritdoc/>
        public int GetHashCode(Object obj) => (obj?.ParentSheetIndex).GetHashCode();
    }
}
