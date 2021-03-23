namespace FarmAnimalVarietyRedux
{
    /// <summary>The ways that animal data can be interpreted.</summary>
    public enum Action
    {
        /// <summary>The animal data will be added.</summary>
        Add,

        /// <summary>The animal data will edit previous animal data.</summary>
        Edit,

        /// <summary>The animal data will delete previous animal data.</summary>
        Delete
    }
}
