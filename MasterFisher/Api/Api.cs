using MasterFisher.Models.Converted;
using MasterFisher.Models.Parsed;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;
using System.Linq;

namespace MasterFisher
{
    /// <summary>Provides basic fish apis.</summary>
    public class Api : IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public void AddFishCategory(ParsedFishCategory category)
        {
            // validate
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrEmpty(category.Name))
            {
                ModEntry.Instance.Monitor.Log("Tried to add a fish category without specifying a name", LogLevel.Error);
                return;
            }

            if (GetFishCategory(category.Name) != null)
            {
                ModEntry.Instance.Monitor.Log($"A fish category with the name: {category.Name} already exists (trying to add)", LogLevel.Error);
                return;
            }

            // add category
            ModEntry.Instance.Categories.Add(new FishCategory(category.Name, category.FishPondCap ?? 10, category.MinigameSprite));
        }

        /// <inheritdoc/>
        public void EditFishCategory(ParsedFishCategory category)
        {
            // validate
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrEmpty(category.Name))
            {
                ModEntry.Instance.Monitor.Log("Tried to edit a fish category without specifying a name", LogLevel.Error);
                return;
            }

            var categoryToEdit = GetFishCategory(category.Name);
            if (categoryToEdit == null)
            {
                ModEntry.Instance.Monitor.Log($"A fish category with the name: {category.Name} doesn't exist (trying to edit)", LogLevel.Error);
                return;
            }

            // edit category
            categoryToEdit.FishPondCap = category.FishPondCap ?? categoryToEdit.FishPondCap;
            categoryToEdit.MinigameSprite = category.MinigameSprite ?? categoryToEdit.MinigameSprite;
        }

        /// <inheritdoc/>
        public void DeleteFishCategory(string categoryName)
        {
            // validate
            if (string.IsNullOrEmpty(categoryName))
            {
                ModEntry.Instance.Monitor.Log("Tried to delete a fish category without specifying a name", LogLevel.Error);
                return;
            }

            var categoryToDelete = GetFishCategory(categoryName);
            if (categoryToDelete == null)
            {
                ModEntry.Instance.Monitor.Log($"A fish category with the name: {categoryName} doesn't exist (trying to delete)", LogLevel.Error);
                return;
            }

            // delete category
            ModEntry.Instance.Categories.Remove(categoryToDelete);
        }

        /// <inheritdoc/>
        public FishCategory GetFishCategory(string categoryName) => ModEntry.Instance.Categories.FirstOrDefault(category => category.Name.ToLower() == categoryName.ToLower());

        /// <inheritdoc/>
        public void AddLocationArea(ParsedLocationArea locationArea)
        {
            // validate
            if (locationArea == null)
                throw new ArgumentNullException(nameof(locationArea));

            if (string.IsNullOrEmpty(locationArea.UniqueName))
            {
                ModEntry.Instance.Monitor.Log("Tried to add a location area without specifying a unique name", LogLevel.Error);
                return;
            }

            if (string.IsNullOrEmpty(locationArea.LocationName))
            {
                ModEntry.Instance.Monitor.Log("Tried to add a location area without specifying a location name", LogLevel.Error);
                return;
            }

            if (GetLocationArea(locationArea.UniqueName) != null)
            {
                ModEntry.Instance.Monitor.Log($"A location area with the unique name: {locationArea.UniqueName} already exists (trying to add)", LogLevel.Error);
                return;
            }

            // add location area // TODO: rectangle empty shouldn't be the default area size (unless theres a check for that in the area code?)
            ModEntry.Instance.LocationAreas.Add(new LocationArea(locationArea.UniqueName, locationArea.LocationName, locationArea.Area, locationArea.SpringFish, locationArea.SummerFish, locationArea.FallFish, locationArea.WinterFish, locationArea.TreasureChance ?? .15f));
        }

        /// <inheritdoc/>
        public void EditLocationArea(ParsedLocationArea locationArea)
        {
            // validate
            if (locationArea == null)
                throw new ArgumentNullException(nameof(locationArea));

            if (string.IsNullOrEmpty(locationArea.UniqueName))
            {
                ModEntry.Instance.Monitor.Log("Tried to edit a location area without specifying a unique name", LogLevel.Error);
                return;
            }

            var locationAreaToEdit = GetLocationArea(locationArea.UniqueName);
            if (locationAreaToEdit == null)
            {
                ModEntry.Instance.Monitor.Log($"A location area with the unique name: {locationArea.UniqueName} doesn't exist (trying to edit)", LogLevel.Error);
                return;
            }

            // edit location area
            locationAreaToEdit.LocationName = locationArea.LocationName ?? locationAreaToEdit.LocationName;
            locationAreaToEdit.Area = locationArea.Area;
            locationAreaToEdit.SpringFish = locationArea.SpringFish ?? locationAreaToEdit.SpringFish;
            locationAreaToEdit.SummerFish = locationArea.SummerFish ?? locationAreaToEdit.SummerFish;
            locationAreaToEdit.FallFish = locationArea.FallFish ?? locationAreaToEdit.FallFish;
            locationAreaToEdit.WinterFish = locationArea.WinterFish ?? locationAreaToEdit.WinterFish;
            locationAreaToEdit.TreasureChance = locationArea.TreasureChance ?? locationAreaToEdit.TreasureChance;
        }

        /// <inheritdoc/>
        public void DeleteLocationArea(string uniqueName)
        {
            // validate
            if (string.IsNullOrEmpty(uniqueName))
            {
                ModEntry.Instance.Monitor.Log("Tried to delete a location area without specifying a unique name", LogLevel.Error);
                return;
            }

            var locationAreaToDelete = GetLocationArea(uniqueName);
            if (locationAreaToDelete == null)
            {
                ModEntry.Instance.Monitor.Log($"A location area with the unique name: {uniqueName} doesn't exist (trying to delete)", LogLevel.Error);
                return;
            }

            // delete location area
            ModEntry.Instance.LocationAreas.Remove(locationAreaToDelete);
        }

        /// <inheritdoc/>
        public LocationArea GetLocationArea(string uniqueName) => ModEntry.Instance.LocationAreas.FirstOrDefault(locationArea => locationArea.UniqueName.ToLower() == uniqueName.ToLower());
    }
}
