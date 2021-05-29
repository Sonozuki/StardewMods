using Outerwear.Models.Converted;

namespace Outerwear.Models.Parsed
{
    /// <summary>Represents an outerwear.</summary>
    /// <remarks>This is a version of <see cref="OuterwearData"/> that has <see cref="OuterwearData.ObjectId"/> as <see langword="string"/>.<br/>The reason this is done is so content packs can have tokens in place of the ids to call mod APIs to get the id (so JsonAsset items can be used for example).</remarks>
    public class ParsedOuterwearData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the outerwear object.</summary>
        public string ObjectId { get; set; }

        /// <summary>The type of outerwear.</summary>
        public OuterwearType Type { get; set; }

        /// <summary>The asset path of the outerwear.</summary>
        public string Asset { get; set; }

        /// <summary>The effects the outerwear has when equipped.</summary>
        public OuterwearEffects Effects { get; set; }
    }
}
