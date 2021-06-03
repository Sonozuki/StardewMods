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

        /// <summary>Adds a location area.</summary>
        /// <param name="locationArea">The location area to add.</param>
        public void AddLocationArea(ParsedLocationArea locationArea);

        /// <summary>Edits a location area.</summary>
        /// <param name="locationArea">The location area to edit (with the new values.)</param>
        public void EditLocationArea(ParsedLocationArea locationArea);

        /// <summary>Deletes a location area.</summary>
        /// <param name="uniqueName">The unique name of the location area to delete.</param>
        public void DeleteLocationArea(string uniqueName);

        /// <summary>Retrieves a location area by unique name.</summary>
        /// <param name="uniqueName">The unique name of the location area to retrieve.</param>
        /// <returns>A location area with the unique name of <paramref name="uniqueName"/>, if one exists; otherwise, <see langword="null"/>.</returns>
        public LocationArea GetLocationArea(string uniqueName);
    }
}
