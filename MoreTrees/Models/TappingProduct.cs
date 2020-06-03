namespace MoreTrees.Models
{
    /// <summary>Represents a product the tree will drop whenever it's tapped.</summary>
    public class TappingProduct
    {
        /// <summary>The number of days between each harvest.</summary>
        public int NumberOfDays { get; set; }

        /// <summary>The product that will drop.</summary>
        public string Product { get; set; }
    }
}
