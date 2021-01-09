namespace MillerTime.Models.Parsed
{
    /// <summary>Represents a mill recipe.</summary>
    /// <remarks>This is a version of <see cref="MillerTime.Models.Converted.Recipe"/> that uses <see cref="MillerTime.Models.Parsed.ParsedOutput"/> that will be used for parsing content packs.</remarks>
    public class ParsedRecipe
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the input item.</summary>
        public string InputId { get; set; } = "-1";

        /// <summary>The recipe output.</summary>
        public ParsedOutput Output { get; set; } = new ParsedOutput();


        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public override string ToString() => $"<InputId: {InputId}, Output: {Output}>";
    }
}
