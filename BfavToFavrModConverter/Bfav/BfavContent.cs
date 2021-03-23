using System.Collections.Generic;

namespace BfavToFavrModConverter.Bfav
{
    /// <summary>Represents BFAV's 'content.json' file.</summary>
    public class BfavContent
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The content wrapper for each animal.</summary>
        public List<BfavCategory> Categories { get; set; }
    }
}
