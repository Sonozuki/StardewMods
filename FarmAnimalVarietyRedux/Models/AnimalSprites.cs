using Microsoft.Xna.Framework.Graphics;

namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>A collection of sprites corresponding to each animal type. This includes sprite sheets for adult, harvest, and baby animal spritesheets per season.</summary>
    public class AnimalSprites
    {
        /*********
        ** Accessors
        *********/
        /****
        ** Non Seasonal
        ****/
        /// <summary>The non seasonal adult sprite sheet for the animal.</summary>
        public Texture2D AdultSpriteSheet { get; set; }

        /// <summary>The non seasonal sprite sheet for the animal when it's ready for harvest.</summary>
        public Texture2D HarvestableSpriteSheet { get; set; }

        /// <summary>The non seasonal baby sprite sheet for the animal.</summary>
        public Texture2D BabySpriteSheet { get; set; }

        /****
        ** Spring
        ****/
        /// <summary>The spring adult sprite sheet for the animal.</summary>
        public Texture2D SpringAdultSpriteSheet { get; set; }

        /// <summary>The spring ready to harvest sprite sheet for the animal.</summary>
        public Texture2D SpringHarvestableSpriteSheet { get; set; }

        /// <summary>The spring baby sprite sheet for the animal.</summary>
        public Texture2D SpringBabySpriteSheet { get; set; }

        /****
        ** Summer
        ****/
        /// <summary>The summer adult sprite sheet of the animal.</summary>
        public Texture2D SummerAdultSpriteSheet { get; set; }

        /// <summary>The summer ready to harvest sprite sheet for the animal.</summary>
        public Texture2D SummerHarvestableSpriteSheet { get; set; }

        /// <summary>The summer baby sprite sheet of the animal.</summary>
        public Texture2D SummerBabySpriteSheet { get; set; }

        /****
        ** Fall
        ****/
        /// <summary>The fall adult sprite sheet of the animal.</summary>
        public Texture2D FallAdultSpriteSheet { get; set; }

        /// <summary>The fall ready to harvest sprite sheet for the animal.</summary>
        public Texture2D FallHarvestableSpriteSheet { get; set; }

        /// <summary>The fall baby sprite sheet of the animal.</summary>
        public Texture2D FallBabySpriteSheet { get; set; }

        /****
        ** Winter
        ****/
        /// <summary>The winter adult sprite sheet of the animal.</summary>
        public Texture2D WinterAdultSpriteSheet { get; set; }

        /// <summary>The winter ready to harvest sprite sheet for the animal.</summary>
        public Texture2D WinterHarvestableSpriteSheet { get; set; }

        /// <summary>The winter baby sprite sheet of the animal.</summary>
        public Texture2D WinterBabySpriteSheet { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="adultSpriteSheet">The non seasonal adult sprite sheet for the animal.</param>
        /// <param name="babySpriteSheet">The non seasonal sprite sheet for the animal when it's ready for harvest.</param>
        /// <param name="harvestableSpriteSheet">The non seasonal baby sprite sheet for the animal.</param>
        /// <param name="springAdultSpriteSheet">The spring adult sprite sheet for the animal.</param>
        /// <param name="springHarvestableSpriteSheet">The spring ready to harvest sprite sheet for the animal.</param>
        /// <param name="springBabySpriteSheet">The spring baby sprite sheet for the animal.</param>
        /// <param name="summerAdultSpriteSheet">The summer adult sprite sheet of the animal.</param>
        /// <param name="summerHarvestableSpriteSheet">The summer ready to harvest sprite sheet for the animal.</param>
        /// <param name="summerBabySpriteSheet">The summer baby sprite sheet of the animal.</param>
        /// <param name="fallAdultSpriteSheet">The fall adult sprite sheet of the animal.</param>
        /// <param name="fallHarvestableSpriteSheet">The fall ready to harvest sprite sheet for the animal.</param>
        /// <param name="fallBabySpriteSheet">The fall baby sprite sheet of the animal.</param>
        /// <param name="winterAdultSpriteSheet">The winter adult sprite sheet of the animal.</param>
        /// <param name="winterHarvestableSpriteSheet">The winter ready to harvest sprite sheet for the animal.</param>
        /// <param name="winterBabySpriteSheet">The winter baby sprite sheet of the animal.</param>
        public AnimalSprites(Texture2D adultSpriteSheet, Texture2D babySpriteSheet, Texture2D harvestableSpriteSheet = null, Texture2D springAdultSpriteSheet = null, Texture2D springHarvestableSpriteSheet = null,
            Texture2D springBabySpriteSheet = null, Texture2D summerAdultSpriteSheet = null, Texture2D summerHarvestableSpriteSheet = null, Texture2D summerBabySpriteSheet = null, 
            Texture2D fallAdultSpriteSheet = null, Texture2D fallHarvestableSpriteSheet = null, Texture2D fallBabySpriteSheet = null, Texture2D winterAdultSpriteSheet = null, Texture2D winterHarvestableSpriteSheet = null,
            Texture2D winterBabySpriteSheet = null)
        {
            AdultSpriteSheet = adultSpriteSheet;
            BabySpriteSheet = babySpriteSheet;
            HarvestableSpriteSheet = harvestableSpriteSheet;
            SpringAdultSpriteSheet = springAdultSpriteSheet;
            SpringHarvestableSpriteSheet = springHarvestableSpriteSheet;
            SpringBabySpriteSheet = springBabySpriteSheet;
            SummerAdultSpriteSheet = summerAdultSpriteSheet;
            SummerHarvestableSpriteSheet = summerHarvestableSpriteSheet;
            SummerBabySpriteSheet = summerBabySpriteSheet;
            FallAdultSpriteSheet = fallAdultSpriteSheet;
            FallHarvestableSpriteSheet = fallHarvestableSpriteSheet;
            FallBabySpriteSheet = fallBabySpriteSheet;
            WinterAdultSpriteSheet = winterAdultSpriteSheet;
            WinterHarvestableSpriteSheet = winterHarvestableSpriteSheet;
            WinterBabySpriteSheet = winterBabySpriteSheet;
        }

        /// <summary>Get whether the sprites are valid, meaning there is atleast a sprite sheet for the baby and adult for each season.</summary>
        /// <returns>Whether the sprites are valid.</returns>
        public bool IsValid()
        {
            // non seasonal
            if (AdultSpriteSheet != null && BabySpriteSheet != null)
                return true;
            
            // spring
            if ((SpringAdultSpriteSheet != null || AdultSpriteSheet != null) && (SpringBabySpriteSheet != null || BabySpriteSheet != null))
                return true;

            // summer
            if ((SummerAdultSpriteSheet != null || AdultSpriteSheet != null) && (SummerBabySpriteSheet != null || BabySpriteSheet != null))
                return true;

            // fall
            if ((FallAdultSpriteSheet != null || AdultSpriteSheet != null) && (FallBabySpriteSheet != null || BabySpriteSheet != null))
                return true;

            // winter
            if ((WinterAdultSpriteSheet != null || AdultSpriteSheet != null) && (WinterBabySpriteSheet != null || BabySpriteSheet != null))
                return true;

            return false;
        }

        /// <summary>Get a valid sprite sheet.</summary>
        /// <param name="isBaby">Whether the sprite sheet should be the baby version.</param>
        /// <param name="isHarvested">Whether the sprite sheet should be the harvestable version.</param>
        /// <param name="season">The season the sprite sheet should be in.</param>
        /// <returns>The sprite sheet </returns>
        public Texture2D GetSpriteSheet(bool isBaby, bool isHarvestable, Season season)
        {
            return (isBaby, isHarvestable, season) switch
            {
                (false, false, Season.Spring) => SpringAdultSpriteSheet ?? AdultSpriteSheet,
                (false, false, Season.Summer) => SummerAdultSpriteSheet ?? AdultSpriteSheet,
                (false, false, Season.Fall) => FallAdultSpriteSheet ?? AdultSpriteSheet,
                (false, false, Season.Winter) => WinterAdultSpriteSheet ?? AdultSpriteSheet,
                (false, true, Season.Spring) => SpringHarvestableSpriteSheet ?? HarvestableSpriteSheet ?? SpringAdultSpriteSheet ?? AdultSpriteSheet,
                (false, true, Season.Summer) => SummerHarvestableSpriteSheet ?? HarvestableSpriteSheet ?? SummerAdultSpriteSheet ?? AdultSpriteSheet,
                (false, true, Season.Fall) => FallHarvestableSpriteSheet ?? HarvestableSpriteSheet ?? FallAdultSpriteSheet ?? AdultSpriteSheet,
                (false, true, Season.Winter) => WinterHarvestableSpriteSheet ?? HarvestableSpriteSheet ?? WinterAdultSpriteSheet ?? AdultSpriteSheet,
                (true, false, Season.Spring) => SpringBabySpriteSheet ?? BabySpriteSheet,
                (true, false, Season.Summer) => SummerBabySpriteSheet ?? BabySpriteSheet,
                (true, false, Season.Fall) => FallBabySpriteSheet ?? BabySpriteSheet,
                (true, false, Season.Winter) => WinterBabySpriteSheet ?? BabySpriteSheet,
                (true, _, _) => BabySpriteSheet
            };
        }

        /// <summary>Get whether the animal has valid harvestable sprite sheets.</summary>
        /// <returns>Whether the animal has valid harvestable sprite sheets.</returns>
        public bool HasHarvestableSpriteSheets()
        {
            if (HarvestableSpriteSheet != null)
                return true;

            if (SpringHarvestableSpriteSheet != null && SummerHarvestableSpriteSheet != null && FallHarvestableSpriteSheet != null && WinterHarvestableSpriteSheet != null)
                return true;

            return false;
        }
    }
}
