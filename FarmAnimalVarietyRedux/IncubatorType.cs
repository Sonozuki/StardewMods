namespace FarmAnimalVarietyRedux
{
    /// <summary>The types of incubator a recipe can use.</summary>
    public enum IncubatorType
    {
        /// <summary>The regular incubator in the coop.</summary>
        Regular = 1,

        /// <summary>The ostrich incubator in the barn.</summary>
        Ostrich = 2,

        /// <summary>Either incubator.</summary>
        Both = Regular | Ostrich
    }
}
