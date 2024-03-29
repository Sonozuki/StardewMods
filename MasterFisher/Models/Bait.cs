﻿using SatoCore;
using SatoCore.Attributes;
using System.Collections.Generic;

namespace MasterFisher.Models
{
    /// <summary>Represents a bait.</summary>
    public class Bait : ModelBase
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the object that the bait is.</summary>
        [Identifier]
        [Token(nameof(ResolvedObjectId))]
        public string ObjectId { get; set; }

        /// <summary>The context tags items that can be found when using the bait.</summary>
        [Editable]
        [DefaultValue(typeof(List<string>))]
        public List<string> ContextTagsAffected { get; set; }

        /// <summary>The fish that can be found when using the bait.</summary>
        [Editable]
        [DefaultValue(typeof(List<string>))]
        public List<string> FishAffected { get; set; }

        /// <summary>The bite rate when using the bait.</summary>
        [Editable]
        [DefaultValue(1)]
        public float? BiteRate { get; set; }

        /// <summary>The id of the object that the bait is.</summary>
        internal int ResolvedObjectId { get; set; }
    }
}
