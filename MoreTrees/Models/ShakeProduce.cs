namespace MoreTrees.Models
{
    /// <summary>Represents a product the tree can drop whenever it's shaked.</summary>
    public class ShakeProduct
    {
        /// <summary>The number of days inbetween the product dropping.</summary>
        public int DaysBetweenDropping { get; set; }

        /// <summary>The product that will drop.</summary>
        public string Product { get; set; }
    }
}
