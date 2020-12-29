**Barking Up the Right Tree** is a [Stardew Valley](http://stardewvalley.net/) framework mod that allows you to add custom trees using json.

![](pics/barkinguptherighttree.png)

## Creating a Content Pack
1. Create a new folder for the content pack. The convention is: **[BURT] mod name**.
2. Create a sub folder for each tree you plan to add (see final result below for reference).
3. Create a **tree.png** image and add to the respective tree folder. NOTE: the images must be called **tree.png**.
4. Create a content.json and add to the respective tree folder (see below for reference). 
5. Create a manifest.json (see below for reference).

#### Final Content Pack Layout
    [BURT] mod name
        manifest.json
        Hard Oak
            tree.png
            content.json
        Birch
            tree.png
            content.json
        Spruce
            tree.png
            content.json
        Maple
            tree.png
            content.json
        Rowan
            tree.png
            content.json

#### Tree.png example
// TODO: give example and pixel locations of tree sprite sheet

#### Content.json example
    {
        "Name": "tree name"
        "TappedProduct": {
            "DaysBetweenProduce": 4.0,
            "Product": "id",
            "Amount": 9
        },
        "WoodProduct": "id",
	    "DropsSap": false,
        "Seed": "id",
        "ShakingProducts": [
            {
                "DaysBetweenProduce": 1,
                "Product": "id",
                "Amount": 3,
                "Seasons": [ "spring", "summer", "fall" ]
            },
            {
                "DaysBetweenProduce": 3,
                "Product": "id",
                "Amount": 10
            }
        ],
        "IncludeIfModIsPresent": [ "uniqueModId", "uniqueModId" ],
        "ExcludeIfModIsPresent": [ "uniqueModId", "uniqueModId" ],
        "BarkProduct": {
            "DaysBetweenProduce": 4,
            "Product": "id",
            "Amount": 15
        }
    }

* **Name**: This is the name of the tree, this doesn't necessarily have to match the folder name but it's encouraged.
* **TappedProduct**: This is the product that the tree drops when using a [tapper](https://stardewvalleywiki.com/Tapper) on it.
* **TappedProduct.DaysBetweenProduce**: The number of days between each harvest.
* **TappedProduct.Product**: The product that gets harvested when using a tapper.
* **TappedProduct.Amount**: The number of items you'll get from each harvest.
* **WoodProduct**: This is the product that the tree drops when it gets cut down.
* **Seed**: This is the item to plant for the tree to grow.
* **ShakingProducts**: This is a list of products that drop when the tree is shaken.
* **ShakingProducts.DaysBetweenProduce**: The number of days between the product can be dropped again.
* **ShakingProducts.Product**: The product that will get dropped.
* **ShakingProducts.Amount**: The amount of the item that will get dropped.
* **ShakingProducts.Seasons**: The seasons the item will get dropped (leaving this out means it can be dropped in all seasons).
* **IncludeIfModIsPresent**: The tree will only get loaded if atleast one of the listed mods (by uniqueId) is present.
* **ExcludeIfModIsPresent**: The tree will only get loaded if none of the listed mods (by uniqueId) are present.
* **BarkProduct**: This is the product that the tree drops when using the **Bark Remover**.
* **BarkProduct.DaysBetweenProduce**: The number of days between each harvest.
* **BarkProduct.Product**: The product that gets harvested when using a **Bark Remover**.
* **BarkProduct.Amount**: The amount of items that gets harvested when using a **Bark Remover**.

**NOTE:** Ensure all ids are strings, this is because they also allow API tokens (The layout is: "UniqueModId:MethodName:Value"), and example of an API token is: **spacechase0.JsonAssets:GetObjectId:Maple Bark**, this will use an item from JA called **Maple Bark**.

#### Manifest.json example
    {
        "Name": "[BURT] mod name",
        "Author": "your name",
        "Version": "1.0.0",
        "Description": "description",
        "UniqueID": "your name.mod name",
        "MinimumApiVersion": "3.8.0",
        "UpdateKeys": [ update key ],
        "ContentPackFor": {
            "UniqueID": "Satozaki.BarkingUpTheRightTree"
        }
    }

## Add to tilemap
// TODO: explain tile data for trees

## Install
1. Install the latest version of [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400).
2. Install the latest version of [SpaceCore](https://www.nexusmods.com/stardewvalley/mods/1348).
3. Install the latest version of [this mod](https://www.nexusmods.com/stardewvalley/mods/).
4. Extract the .zip mod file into your StardewValley/Mods folder and run the game using SMAPI.

## Use
First, add any content packs to the **StardewValley/Mods** folder 
Then load into the game with SMAPI and play like normal.

## Compatibility
Barking Up the Right Tree is compatible with Stardew Valley 1.5+ on Windows/Mac/Linus, both single player and multiplayer. To view reported bugs visit both the issues on this repo and bug reports on [Nexus](https://www.nexusmods.com/stardewvalley/mods/?tab=bugs).

TODO: add mod ids to compat and install sections