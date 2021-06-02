using MasterFisher.Models.Converted;
using MasterFisher.Models.Parsed;
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
    }
}
