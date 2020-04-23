namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents the 'content.json' file containing data about each animal sub type for FAVR.</summary>
    public class FavrSubTypeContent
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The item id of the product (API tokens are accepted).</summary>
        public string ProductId { get; set; }

        /// <summary>The item id of the deluxe product (API tokens are accepted).</summary>
        public string DeluxeProductId { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="productId">The item id of the product (API tokens are accepted).</param>
        /// <param name="deluxeProductId">The item id of the deluxe product (API tokens are accepted).</param>
        public FavrSubTypeContent(string productId, string deluxeProductId)
        {
            ProductId = productId;
            DeluxeProductId = deluxeProductId;
        }
    }
}
