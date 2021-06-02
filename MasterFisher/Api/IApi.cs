using MasterFisher.Models.Converted;
using MasterFisher.Models.Parsed;

namespace MasterFisher
{
    /// <summary>Provides basic fish apis.</summary>
    public interface IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Adds a fish category.</summary>
        /// <param name="category">The category to add.</param>
        public void AddFishCategory(ParsedFishCategory category);

        /// <summary>Edits a fish category.</summary>
        /// <param name="category">The category to edit (with the new values.)</param>
        public void EditFishCategory(ParsedFishCategory category);

        /// <summary>Deletes a fish category.</summary>
        /// <param name="categoryName">The name of the category to delete.</param>
        public void DeleteFishCategory(string categoryName);

        /// <summary>Retrieves a fish catorgy by name.</summary>
        /// <param name="categoryName">The name of the category to retrieve.</param>
        /// <returns>A fish category with the name of <paramref name="categoryName"/>, if one exists; otherwise, <see langword="null"/>.</returns>
        public FishCategory GetFishCategory(string categoryName);
    }
}
