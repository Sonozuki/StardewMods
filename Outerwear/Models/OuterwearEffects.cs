namespace Outerwear.Models
{
    /// <summary>Represents the effects the outerwear has when equipped.</summary>
    public class OuterwearEffects
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The farming skill added to the player.</summary>
        public int FarmingIncrease { get; set; }

        /// <summary>The mining skill added to the player.</summary>
        public int MiningIncrease { get; set; }

        /// <summary>The foraging skill added to the player.</summary>
        public int ForagingIncrease { get; set; }

        /// <summary>The fishing skill added to the player.</summary>
        public int FishingIncrease { get; set; }

        /// <summary>The max stamina added to the player.</summary>
        public int MaxStaminaIncrease { get; set; }

        /// <summary>The magnetic radius added to the player.</summary>
        public int MagneticRadiusIncrease { get; set; }

        /// <summary>The movement speed added to the player.</summary>
        public int SpeedIncrease { get; set; }

        /// <summary>The defence added to the player.</summary>
        public int DefenceIncrease { get; set; }

        /// <summary>The attack added to the player.</summary>
        public int AttackIncrease { get; set; }

        /// <summary>Whether the outerwear emits light.</summary>
        public bool EmitsLight { get; set; }

        /// <summary>The radius of the emitted light.</summary>
        public float LightRadius { get; set; } = 10;
    }
}
