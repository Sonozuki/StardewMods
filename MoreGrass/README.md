**More Grass** is a [Stardew Valley](http://stardewvalley.net/) framework mod that allows you to add custom grass sprites, season dependant using json.

![](pics/moregrass.png)

## Creating a Content Pack
1. Create a new folder for the content pack. The convention is: **[MG] mod name**.
2. Create a sub folder for each season you have grass sprites for (see final result below for reference) NOTE: name is case sensitive, use lowercase.
3. Add all the **.png** images to the respective season folders. NOTE: the images can have any name, there is no convention. These should be **15px x 20px**
4. Create a manifest.json, see below for refence

#### Final Content Pack Layout
    [MG] mod name
        manifest.json
        spring
            1.png
            2.png
        summer
            1.png
            2.png
        fall
            1.png
            2.png
        winter
            1.png
            2.png

#### Manifest.json example
    {
        "Name": "[MG] mod name",
        "Author": "your name",
        "Version": "1.0.0",
        "Description": "description",
        "UniqueID": "your name.mod name",
        "MinimumApiVersion": "3.0.0",
        "UpdateKeys": [ update key ],
        "ContentPackFor": {
            "UniqueID": "EpicBellyFlop45.MoreGrass"
        }
    }

## Install
1. Install the latest version of [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400).
2. Install the latest version of [this mod](https://www.nexusmods.com/stardewvalley/mods/5398).
3. Extract the .zip mod file into your StardewValley/Mods folder and run the game using SMAPI.

## Use
First, open the game using SMAPI like normal, this will generate a config.json file in the Mods/BetterMixedSeeds folder.
Then using the below section configure the mixed seeds to your liking.
Lastly load back into the game and use mixed seeds like normal.


## Compatibility
More Grass is compatible with Stardew Valley 1.4+ on Windows/Mac/Linus, both single player and multiplayer. To view reported bug visit both the issues on this repo and bug reports on [Nexus](https://www.nexusmods.com/stardewvalley/mods/5398?tab=bugs).
