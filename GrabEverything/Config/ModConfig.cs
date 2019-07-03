using GrabEverything.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrabEverything
{
    class ModConfig
    {
        /*public Dictionary<string, ConfigItem> Config { get; set; } = new Dictionary<string, ConfigItem> {
            { "AutoWaterCrops", new ConfigItem(false, 5, false) },
            { "AutoHarvestCrops", new ConfigItem() },
            { "AutoDestroyDeadCrops", new ConfigItem() },
            { "AutoHarvestFlowers", new ConfigItem() },
            { "AutoDigArtifactSpots", new ConfigItem() },
            { "AutoRefillWateringCan", new ConfigItem() },
            { "AutoCollectCollectables", new ConfigItem() },
            { "AutoShakeFruitPlants", new ConfigItem() },
            { "AutoScavengeTrashBins", new ConfigItem() },
            { "AutoAnimalProdutsHarvest", new ConfigItem() },
            { "AutoHoeGround", new ConfigItem() },
            { "AutoCollectFromMachines", new ConfigItem() },
            { "AutoDestroyWeeds", new ConfigItem() },
            { "AutoCutTrees", new ConfigItem() },
            { "AutoDestroyRocks", new ConfigItem() },
            { "AutoHarvestLongGrass", new ConfigItem() }
        };*/

        public ConfigItem AutoWaterCrops { get; set; } = new ConfigItem(/*new Dictionary<string, bool> { { "Hoe", false }, { "Watering Can", false }, { "Axe", false }, { "Fishing Rod", false }, { "Pickaxe", false }, { "Scythe", false }, { "Sword", false }, { "Club", false }, { "Dagger", false }, { "Slingshot", false }, { "Milk Pail", false }, { "Shears", false } }, */true, 3, true );
        public ConfigItem AutoHarvestCrops { get; set; } = new ConfigItem(/*new Dictionary<string, bool> { { "Hoe", false }, { "Watering Can", false }, { "Axe", false }, { "Fishing Rod", false }, { "Pickaxe", false }, { "Scythe", false }, { "Sword", false }, { "Club", false }, { "Dagger", false }, { "Slingshot", false }, { "Milk Pail", false }, { "Shears", false } }, */true, 3, true );
        public ConfigItem AutoDestroyDeadCrops { get; set; } = new ConfigItem(/*new Dictionary<string, bool> { { "Hoe", false }, { "Watering Can", false }, { "Axe", false }, { "Fishing Rod", false }, { "Pickaxe", false }, { "Scythe", false }, { "Sword", false }, { "Club", false }, { "Dagger", false }, { "Slingshot", false }, { "Milk Pail", false }, { "Shears", false } }, */true, 3, true );
        public Dictionary<string, bool> Test { get; set; } = new Dictionary<string, bool> { { "Hoe", false }, { "Watering Can", false }, { "Axe", false }, { "Fishing Rod", false }, { "Pickaxe", false }, { "Scythe", false }, { "Sword", false }, { "Club", false }, { "Dagger", false }, { "Slingshot", false }, { "Milk Pail", false }, { "Shears", false } };

    /*
    public bool AutoWaterCrops { get; set; } = true;
    public int AutoWaterCropsRadius { get; set; } = 1;
    public bool RequireWateringCan { get; set; } = true;

    public bool AutoHarvestCrops { get; set; } = true;
    public int AutoHarvestCropsRadius { get; set; } = 1;
    public bool AutoHarvestCropsRequireScythe { get; set; }

    public bool AutoDestroyDeadCrops { get; set; } = true;
    public int AutoDestroyDeadCropsRadius { get; set; }
    public bool AutoDestroyDeadCropsRequirePickaxe { get; set; }

    public bool AutoHarvestFlowers { get; set; }
    public int AutoHarvestFlowersRadius { get; set; }
    public bool AutoHarvestFlowersRequireScythe { get; set; }

    public bool AutoDigArtifactSpots { get; set; } = false;
    public int AutoDigArtifactSpotsRadius { get; set; } = 1;
    public bool AutoDigArtifactSpotsRequireHoe { get; set; } = true;

    public bool AutoRefillWateringCan { get; set; } = true;
    public int AutoRefillWateringCanRadius { get; set; }
    public bool AutoRefillWateringCanRequireWateringCan { get; set; }

    public bool AutoCollectCollectables { get; set; } = false;
    public int AutoCollectCollectablesRadius { get; set; } = 1;
    public bool AutoCollectCollectablesRequireBareHands { get; set; }

    public bool AutoShakeFruitPlants { get; set; } = true;
    public int AutoShakeFruitPlantsRadius { get; set; } = 1;
    public bool AutoShakeFruitPlantsRequireBareHands { get; set; }
    */
    /*public bool AutoScavengeTrashBins { get; set; } = false;
    public int AutoScavengeTrashBinsRadius { get; set; } = 2;
    public bool AutoScavengeTrashBinsRequireBareHands { get; set; }

    public bool AutoAnimalHarvest { get; set; } = true;
    public int AutoAnimalHarvestsRadius { get; set; } = 1;
    public bool AutoAnimalHarvestRequireBareHands { get; set; }

    public bool AutoHoeGround { get; set; }
    public int AutoHoeGroundRadius { get; set; }
    public bool AutoHoeGroundRequireHoe { get; set; }

    public bool AutoCollectFromMachines { get; set; }
    public int AutoCollectFromMachinesRadius { get; set; }
    public bool AutoCollectFromMachinesRequireBareHands { get; set; }

    public bool AutoDestroyWeeds { get; set; }
    public int AutoDestroyWeedsRedius { get; set; }
    public bool AutoDestroyWeedsRequireScythe { get; set; }

    public bool AutoCutTrees { get; set; }
    public int AutoDestroyTreesRadius { get; set; }
    public bool AutoDestroyTreesRequireAxe { get; set; }

    public bool AutoDestroyRocks { get; set; }
    public int AutoDestroyRocksRadius { get; set; }
    public bool AutoDestroyRocksRequirePickaxe { get; set; }

    public bool AutoHarvestLongGrass { get; set; }
    public int AutoHarvestLongGrassRadius { get; set; }
    public bool AutoHarvestLongGrassRequireScythe { get; set; }
    */
}
}
