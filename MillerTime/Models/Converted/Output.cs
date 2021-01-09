namespace MillerTime.Models.Converted
{
    /// <summary>Represents the output of a recipe.</summary>
    public class Output
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the output.</summary>
        public int Id { get; }

        /// <summary>The number of objects to output.</summary>
        public int Amount { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="id">The id of the output.</param>
        /// <param name="amount">The number of objects to output.</param>
        public Output(int id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}
