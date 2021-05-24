using FarmAnimalVarietyRedux.Menus;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="PurchaseAnimalsMenu"/> class.</summary>
    internal class PurchaseAnimalsMenuPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="PurchaseAnimalsMenu"/>() constructor.</summary>
        /// <param name="stock">The animals that were passed to the menu.</param>
        /// <remarks>This is used to keep track of the animals that should be displayed in the new <see cref="CustomPurchaseAnimalsMenu"/>.</remarks>
        internal static void ConstructorPrefix(List<Object> stock) => ModEntry.Instance.AnimalsInPurchaseMenu = stock;
    }
}
