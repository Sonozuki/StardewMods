namespace MillerTime.Models.Converted
{
    /// <summary>Represents a mill recipe.</summary>
    public class Recipe
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the input item.</summary>
        public int InputId { get; }

        /// <summary>The recipe output.</summary>
        public Output Output { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="inputId">The id of the input item.</param>
        /// <param name="output">The recipe output.</param>
        public Recipe(int inputId, Output output)
        {
            InputId = inputId;
            Output = output;
        }
    }
}
