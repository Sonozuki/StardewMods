using System.Collections.Generic;

namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents FAVR's 'content.json' file.</summary>
    public class FavrContent
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the animal.</summary>
        public string Name { get; set; }

        /// <summary>The description of the animal.</summary>
        public string Description { get; set; }

        /// <summary>The sub types of the animal.</summary>
        public List<FavrAnimalSubType> Types { get; set; }

        /// <summary>The number of days it takes the animal to produce product.</summary>
        public int DaysToProduce { get; set; }

        /// <summary>The number of days it takes the animal to become an adult.</summary>
        public int DaysTillMature { get; set; }

        /// <summary>The id of the sound the animal will make.</summary>
        public string SoundId { get; set; }

        /// <summary>The width of the animal sprite when it's looking toward / away from the camera.</summary>
        public int FrontAndBackSpriteWidth { get; set; }

        /// <summary>The height of the animal sprite when it's looking toward / away from the camera.</summary>
        public int FrontAndBackSpriteHeight { get; set; }

        /// <summary>The width of the animal sprite when it's looking to the side.</summary>
        public int SideSpriteWidth { get; set; }

        /// <summary>The height of the animal sprite when it's looking to the side.</summary>
        public int SideSpriteHeight { get; set; }

        /// <summary>The amount the animal's hunger bar will drain each night.</summary>
        public byte FullnessDrain { get; set; }

        /// <summary>The amount the animal's happiness bar will drain each night.</summary>
        public byte HappinessDrain { get; set; }

        /// <summary>The amount the animal costs.</summary>
        public int BuyPrice { get; set; }

        /// <summary>The name(s) of the building(s) the animal can be housed in.</summary>
        public List<string> Buildings { get; set; }

        /// <summary>The walk speed of the animal.</summary>
        public int WalkSpeed { get; set; } = 2;


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the animal.</param>
        /// <param name="description">The description of the animal.</param>
        /// <param name="types">The sub types of the animal.</param>
        /// <param name="daysToProduce">The number of days it takes the animal to produce product.</param>
        /// <param name="daysTillMature">The number of days it takes the animal to become an adult.</param>
        /// <param name="soundId">The id of the sound the animal will make.</param>
        /// <param name="harvestType">The method required to harvest product from the animal.</param>
        /// <param name="harvestToolName">The name of the tool required to harvest (If the harvest method requires a tool).</param>
        /// <param name="frontAndBackSpriteWidth">The width of the animal sprite when it's looking toward / away from the camera.</param>
        /// <param name="frontAndBackSpriteHeight">The height of the animal sprite when it's looking toward / away from the camera.</param>
        /// <param name="sideSpriteWidth">The width of the animal sprite when it's looking to the side.</param>
        /// <param name="sideSpriteHeight">The height of the animal sprite when it's looking to the side.</param>
        /// <param name="fullnessDrain">The amount the animal's hunger bar will drain each night.</param>
        /// <param name="happinessDrain">The amount the animal's happiness bar will drain each night.</param>
        /// <param name="buyPrice">The amount the animal costs.</param>
        /// <param name="buildings">The name(s) of the building(s) the animal can be housed in.</param>
        public FavrContent(string name, string description, List<FavrAnimalSubType> types, int daysToProduce, int daysTillMature, string soundId, int frontAndBackSpriteWidth, int frontAndBackSpriteHeight,
            int sideSpriteWidth, int sideSpriteHeight, byte fullnessDrain, byte happinessDrain, int buyPrice, List<string> buildings)
        {
            Name = name;
            Description = description;
            Types = types;
            DaysToProduce = daysToProduce;
            DaysTillMature = daysTillMature;
            SoundId = soundId;
            FrontAndBackSpriteWidth = frontAndBackSpriteWidth;
            FrontAndBackSpriteHeight = frontAndBackSpriteHeight;
            SideSpriteWidth = sideSpriteWidth;
            SideSpriteHeight = sideSpriteHeight;
            FullnessDrain = fullnessDrain;
            HappinessDrain = happinessDrain;
            BuyPrice = buyPrice;
            Buildings = buildings;
        }
    }
}
