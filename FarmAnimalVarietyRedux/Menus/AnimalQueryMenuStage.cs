namespace FarmAnimalVarietyRedux.Menus
{
    /// <summary>The stages of the <see cref="CustomAnimalQueryMenu"/>.</summary>
    public enum AnimalQueryMenuStage
    {
        /// <summary>The main stage of the menu, where the animal infomation is shown.</summary>
        Main,

        /// <summary>A secondary stage of the menu, the player is choosing which building to put the animal in.</summary>
        Rehoming,

        /// <summary>A secondary stage of the menu, the player is confirming to sell the animal.</summary>
        ConfirmingSell
    }
}
