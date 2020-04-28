namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents an animal's sub type for FAVR.</summary>
    public class FavrAnimalSubType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the subtype.</summary>
        public string Name { get; set; }

        /// <summary>The item id of the product (API tokens are accepted).</summary>
        public string ProductId { get; set; }

        /// <summary>The item id of the deluxe product (API tokens are accepted).</summary>
        public string DeluxeProductId { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the subtype.</param>
        /// <param name="productId">The item id of the product (API tokens are accepted).</param>
        /// <param name="deluxeProductId">The item id of the deluxe product (API tokens are accepted).</param>
        public FavrAnimalSubType(string name, string productId, string deluxeProductId)
        {
            Name = name;
            ProductId = productId;
            DeluxeProductId = deluxeProductId;
        }
    }
}
