using BFAVToFAVRModConverter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BFAVToFAVRModConverter
{
    /// <summary>The application entry point.</summary>
    public class Program
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>The application entry point.</summary>
        /// <param name="args">The command line arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Logger.WriteLine("Invalid args supplied", ConsoleColor.Red);
                    Logger.WriteLine("Press any key to exit...", ConsoleColor.White);
                    Console.ReadKey();
                }

                // the folder that contains the bfav mod folders
                var sourceFolder = Path.GetFullPath(args[0]);
                // the folder that will contain the converted favr mod folders
                var destinationFolder = Path.GetFullPath(args[1]);

                if (!Directory.Exists(sourceFolder))
                    Directory.CreateDirectory(sourceFolder);
                if (!Directory.Exists(destinationFolder))
                    Directory.CreateDirectory(destinationFolder);

                var modDirectorties = Directory.GetDirectories(sourceFolder);
                foreach (var modDirectory in modDirectorties)
                {
                    var bfavModFolderName = GetDirectoryRootName(modDirectory);
                    var favrModFolderName = bfavModFolderName.Replace("[BFAV]", "[FAVR]");

                    Logger.WriteLine($"Converting {GetDirectoryRootName(modDirectory)} to {favrModFolderName}", ConsoleColor.White);

                    ConvertBFAVModFolder(modDirectory, Path.Combine(destinationFolder, favrModFolderName));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Converter failed and couldn't recover: {ex.Message}\n{ex.StackTrace}", ConsoleColor.Red);
            }

            Logger.WriteLine("Press any key to exit...", ConsoleColor.White);
            Console.ReadKey();
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Connvert the given BFAV mod folder into a FAVR mod folder.</summary>
        /// <param name="bfavFolderPath">The path to the BFAV mod folder to convert.</param>
        /// <param name="destinationFavrFolderPath">The path where the converted FAVR mod folder will go.</param>
        private static void ConvertBFAVModFolder(string bfavFolderPath, string destinationFavrFolderPath)
        {
            Logger.WriteLine("Converting manifest.json file", ConsoleColor.Gray);
            var bfavManifest = DeserializeJsonFile<ModManifest>(Path.Combine(bfavFolderPath, "manifest.json"));
            if (bfavManifest == default)
                return;
            var favrManifest = ConvertBfavManifest(bfavManifest);
            SerializeObjectToJson(favrManifest, Path.Combine(destinationFavrFolderPath, "manifest.json"));

            Logger.WriteLine("Converting content.json file", ConsoleColor.Gray);
            var bfavContent = DeserializeJsonFile<BfavContent>(Path.Combine(bfavFolderPath, "content.json"));
            if (bfavContent == default)
                return;
            if (bfavContent.Categories[0].Action != "Create")
            {
                Logger.WriteLine("Content.json isn't valid. FAVR doesn't support editing previous entries. Skipping", ConsoleColor.Red);
                return;
            }
            var favrContent = ConvertBfavContent(bfavContent);
            SerializeObjectToJson(favrContent, Path.Combine(destinationFavrFolderPath, favrContent.Name, "content.json"));

            Logger.WriteLine("Converting sprite sheets", ConsoleColor.Gray);
            MoveSpriteSheets(bfavFolderPath, destinationFavrFolderPath, bfavContent, favrContent);

            Logger.WriteLine("Converting animal subtype content.json files", ConsoleColor.Gray);
            CreateSubTypeContent(bfavContent, Path.Combine(destinationFavrFolderPath, favrContent.Name, "assets"));
        }

        /// <summary>Read an serialize the file at the given path.</summary>
        /// <typeparam name="T">The type to serialize the object to.</typeparam>
        /// <param name="path">The path containing the file to serialize.</param>
        /// <returns>The file serialized.</returns>
        private static T DeserializeJsonFile<T>(string path)
        {
            try
            {
                T serializedObject;

                using (var reader = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializedObject = (T)serializer.Deserialize(reader, typeof(T));
                }

                return serializedObject;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to deserialize object: {ex.Message}\n{ex.StackTrace}", ConsoleColor.Red);
                return default;
            }
        }

        /// <summary>Serilize the given object to the given path</summary>
        /// <param name="objectToSerialize">The object to serilize.</param>
        /// <param name="path">The path to serialize the object to.</param>
        private static void SerializeObjectToJson(object objectToSerialize, string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (StreamWriter streamWriter = new StreamWriter(path))
                using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.Formatting = Formatting.Indented;

                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jsonWriter, objectToSerialize);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to serialize object: {ex.Message}\n{ex.StackTrace}", ConsoleColor.Red);
            }
        }

        /// <summary>Convert the given <see cref="ModManifest"/> from the BFAV version to the FAVR version.</summary>
        /// <param name="bfavManifest">The <see cref="ModManifest"/> to convert.</param>
        /// <returns>The converted pased <see cref="ModManifest"/>.</returns>
        private static ModManifest ConvertBfavManifest(ModManifest bfavManifest)
        {
            return new ModManifest(
                name: bfavManifest.Name.Replace("[BFAV]", "[FAVR]"),
                author: bfavManifest.Author,
                version: bfavManifest.Version,
                description: bfavManifest.Description,
                uniqueId: bfavManifest.UniqueId,
                contentPackFor: new Dictionary<string, string> { { "UniqueId", "EpicBellyFlop45.FarmAnimalVarietyRedux" } }
            );
        }

        /// <summary>Convert the given <see cref="BfavContent"/> to <see cref="FavrContent"/>.</summary>
        /// <param name="bfavContent">The <see cref="BfavContent"/> to convert.</param>
        /// <returns>The converted passed <see cref="BfavContent"/>.</returns>
        private static FavrContent ConvertBfavContent(BfavContent bfavContent)
        {
            var dataString = bfavContent.Categories[0].Types[0].Data; // use types[0] as the data used here is common on all sub types
            var splitDataString = dataString.Split('/');

            var dataStringValid = ValidateBfavContentDataString(splitDataString, bfavContent.Categories[0].Types[0].Type);
            if (!dataStringValid)
                return null;

            return new FavrContent(
                name: bfavContent.Categories[0].AnimalShop.Name,
                description: bfavContent.Categories[0].AnimalShop.Description,
                types: bfavContent.Categories[0].Types.Select(type => type.Type).ToList(),
                daysToProduce: Convert.ToInt32(splitDataString[0]),
                daysTillMature: Convert.ToInt32(splitDataString[1]),
                soundId: splitDataString[4],
                harvestType: (HarvestType)Enum.Parse(typeof(HarvestType), splitDataString[13]),
                harvestToolName: splitDataString[22] == "null" ? null : splitDataString[22],
                frontAndBackSpriteWidth: Convert.ToInt32(splitDataString[16]),
                frontAndBackSpriteHeight: Convert.ToInt32(splitDataString[17]),
                sideSpriteWidth: Convert.ToInt32(splitDataString[18]),
                sideSpriteHeight: Convert.ToInt32(splitDataString[19]),
                fullnessDrain: Convert.ToByte(splitDataString[20]),
                happinessDrain: Convert.ToByte(splitDataString[21]),
                buyPrice: bfavContent.Categories[0].AnimalShop.Price,
                buildings: bfavContent.Categories[0].Buildings
            );
        }

        /// <summary>Validate a BFAV content.json data string.</summary>
        /// <param name="dataString">The data string to validate.</param>
        /// <param name="bfavAnimalName">The name of the animal the data string belongs to. This is only used for error logging.</param>
        /// <returns>Whether the passed datastring is valid.</returns>
        private static bool ValidateBfavContentDataString(string[] dataString, string bfavAnimalName)
        {
            var isValid = true;

            // days to produce
            if (!int.TryParse(dataString[0], out _))
            {
                isValid = false;
                Logger.WriteLine($"The 'DaysToProduce' data wasn't valid for ${bfavAnimalName}", ConsoleColor.Red);
            }

            // days till mature
            if (!int.TryParse(dataString[0], out _))
            {
                isValid = false;
                Logger.WriteLine($"The 'DaysTillMature' data wasn't valid for ${bfavAnimalName}", ConsoleColor.Red);
            }

            // front And Back Sprite Width
            if (!int.TryParse(dataString[0], out _))
            {
                isValid = false;
                Logger.WriteLine($"The 'FrontAndBackSpriteWidth' data wasn't valid for ${bfavAnimalName}", ConsoleColor.Red);
            }

            // front And Back Sprite Height
            if (!int.TryParse(dataString[0], out _))
            {
                isValid = false;
                Logger.WriteLine($"The 'FrontAndBackSpriteHeight' data wasn't valid for ${bfavAnimalName}", ConsoleColor.Red);
            }

            // side Sprite Width
            if (!int.TryParse(dataString[0], out _))
            {
                isValid = false;
                Logger.WriteLine($"The 'SideSpriteWidth' data wasn't valid for ${bfavAnimalName}", ConsoleColor.Red);
            }

            // side Sprite Height
            if (!int.TryParse(dataString[0], out _))
            {
                isValid = false;
                Logger.WriteLine($"The 'SideSpriteHeight' data wasn't valid for ${bfavAnimalName}", ConsoleColor.Red);
            }

            // fullness Drain
            if (!byte.TryParse(dataString[0], out _))
            {
                isValid = false;
                Logger.WriteLine($"The 'FullnessDrain' data wasn't valid for ${bfavAnimalName}", ConsoleColor.Red);
            }

            // happiness Drain
            if (!byte.TryParse(dataString[0], out _))
            {
                isValid = false;
                Logger.WriteLine($"The 'HappinessDrain' data wasn't valid for ${bfavAnimalName}", ConsoleColor.Red);
            }

            return isValid;
        }

        /// <summary>Move and rename all the spritesheets to the correct format.</summary>
        /// <param name="bfavFolderPath">The root folder path of the bfav mod being converted.</param>
        /// <param name="destinationFavrFolderPath">The destination folder path where the favr mod will be converted to.</param>
        /// <param name="bfavContent">The deserialized 'content.json' file for the current mod being converted.</param>
        /// <param name="favrContent">The converted 'content.json' file for the current bfav mod being converted.</param>
        private static void MoveSpriteSheets(string bfavFolderPath, string destinationFavrFolderPath, BfavContent bfavContent, FavrContent favrContent)
        {
            try
            {
                // create asset folder in destination folder
                var favrAssetsFolder = Path.Combine(destinationFavrFolderPath, favrContent.Name, "assets");
                if (!Directory.Exists(favrAssetsFolder))
                    Directory.CreateDirectory(favrAssetsFolder);

                // shop icon
                File.Copy(
                    sourceFileName: Path.Combine(bfavFolderPath, bfavContent.Categories[0].AnimalShop.Icon),
                    destFileName: Path.Combine(favrAssetsFolder, @"..\", "shopdisplay.png"),
                    overwrite: true
                );

                foreach (var subType in bfavContent.Categories[0].Types)
                {
                    // adult sprite sheet
                    var adultSpriteSheetPath = Path.Combine(bfavFolderPath, subType.Sprites.Adult);
                    
                    // baby sprite sheet
                    var babySpriteSheetPath = "";
                    if (!string.IsNullOrEmpty(subType.Sprites.Baby) && subType.Sprites.Baby.ToLower() != subType.Sprites.Adult.ToLower()) // ensure it's not the same as the adult, some mods use the same sprite sheet. this would create duplicates of the sprite sheet otherwise
                        babySpriteSheetPath = Path.Combine(bfavFolderPath, subType.Sprites.Baby);

                    // harvested sprite sheet
                    var harvestedSpriteSheetPath = "";
                    if (!string.IsNullOrEmpty(subType.Sprites.ReadyForHarvest))
                        harvestedSpriteSheetPath = Path.Combine(bfavFolderPath, subType.Sprites.ReadyForHarvest);

                    // copy over sprite sheets
                    if (!string.IsNullOrEmpty(adultSpriteSheetPath))
                    {
                        File.Copy(
                            sourceFileName: adultSpriteSheetPath,
                            destFileName: Path.Combine(favrAssetsFolder, $"{subType.Type}.png"),
                            overwrite: true
                        );
                    }

                    if (!string.IsNullOrEmpty(babySpriteSheetPath))
                    {
                        File.Copy(
                            sourceFileName: babySpriteSheetPath,
                            destFileName: Path.Combine(favrAssetsFolder, $"Baby {subType.Type}.png"),
                            overwrite: true
                        );
                    }

                    if (!string.IsNullOrEmpty(harvestedSpriteSheetPath))
                    {
                        File.Copy(
                            sourceFileName: harvestedSpriteSheetPath,
                            destFileName: Path.Combine(favrAssetsFolder, $"Harvested {subType.Type}.png"),
                            overwrite: true
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to move sprites: {ex.Message}\n{ex.StackTrace}", ConsoleColor.Red);
            }
        }

        /// <summary>Create the 'content.json' files for each animal sub type</summary>
        /// <param name="bfavContent">The deseralized 'content.json' file for the BFAV mod currently being converted.</param>
        /// <param name="favrAssetsPath">The path the FAVR assets folder where the 'content.json' files will go.</param>
        private static void CreateSubTypeContent(BfavContent bfavContent, string favrAssetsPath)
        {
            try
            {
                foreach (var animalType in bfavContent.Categories[0].Types)
                {
                    var splitDataString = animalType.Data.Split('/');

                    var productId = splitDataString[2];
                    var deluxeProductId = splitDataString[3];
                    // check if the (deluxe)product ids are valid ids, or it they should have api tokens added to them
                    if (!int.TryParse(productId, out _))
                        productId = $"spacechase0.JsonAssets:GetObjectId:{productId}";

                    if (!int.TryParse(deluxeProductId, out _))
                        deluxeProductId = $"spacechase0.JsonAssets:GetObjectId:{deluxeProductId}";

                    var subTypeContent = new FavrSubTypeContent(
                        productId: productId,
                        deluxeProductId: deluxeProductId
                    );

                    SerializeObjectToJson(subTypeContent, Path.Combine(favrAssetsPath, $"{animalType.Type} content.json"));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to create sub type content.json files: {ex.Message}\n{ex.StackTrace}", ConsoleColor.Red);
            }
        }

        /// <summary>Get the name of the directory root.</summary>
        /// <param name="path">The directory.</param>
        /// <returns>The name of the directory root.</returns>
        private static string GetDirectoryRootName(string path)
        {
            var splitDirectory = path.Split(Path.DirectorySeparatorChar);
            return splitDirectory[splitDirectory.Count() - 1];
        }
    }
}
