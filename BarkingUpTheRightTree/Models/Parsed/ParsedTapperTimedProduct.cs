using BarkingUpTheRightTree.Models.Converted;

namespace BarkingUpTheRightTree.Models.Parsed
{
    /// <summary>Represents a product that a tree drops (through tapping) with a number of days between each production.</summary>
    /// <remarks>This is a version of <see cref="TapperTimedProduct"/> that inherits from <see cref="ParsedTimedProduct"/> because <see cref="ParsedTimedProduct.ProductId"/> is <see langword="string"/>.<br/>The reason this is done is so content packs can have tokens in place of the ids to call mod APIs to get the id (so JsonAsset items can be used for example).</remarks>
    public class ParsedTapperTimedProduct : ParsedTimedProduct
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The number of days inbetween the product dropping.</summary>
        public new float DaysBetweenProduce { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="daysBetweenProduce">The number of days inbetween the product dropping.</param>
        /// <param name="product">The product that will drop.</param>
        /// <param name="amount">The amount of product that will be produced.</param>
        public ParsedTapperTimedProduct(float daysBetweenProduce, string product, int amount)
            : base(0, product, amount)
        {
            DaysBetweenProduce = daysBetweenProduce;
        }
    }
}
