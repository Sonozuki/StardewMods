using System.Collections.Generic;
using System.Linq;

namespace MoreGrass
{
    /// <summary>Contains miscellaneous helper methods.</summary>
    public class Utilities
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Gets whether a list of locations contains a specified location.</summary>
        /// <param name="locations">The list of locations to check against.</param>
        /// <param name="currentLocationName">The name of the location to check if it's in <paramref name="locations"/>.</param>
        /// <returns><see langword="true"/>, if <paramref name="currentLocationName"/> is specified in <paramref name="locations"/>; otherwise, <see langword="false"/>.</returns>
        /// <remarks><paramref name="locations"/> can contain special syntax, these are: <c>"s:{value}"</c> to check if the <paramref name="currentLocationName"/> starts with <c>value</c>, <c>"c:{value}"</c> to check if <paramref name="currentLocationName"/> contains <c>value</c>, and <c>"e:{value}"</c> to check if <paramref name="currentLocationName"/> ends with <c>value</c>.</remarks>
        public static bool ContainsCurrentLocation(List<string> locations, string currentLocationName)
        {
            currentLocationName = currentLocationName.ToLower();

            foreach (var location in locations.Select(location => location.ToLower()))
            {
                if (location.StartsWith("s:"))
                {
                    if (currentLocationName.StartsWith(location.Substring(2)))
                        return true;
                }
                else if (location.StartsWith("c:"))
                {
                    if (currentLocationName.Contains(location.Substring(2)))
                        return true;
                }
                else if (location.StartsWith("e:"))
                {
                    if (currentLocationName.EndsWith(location.Substring(2)))
                        return true;
                }
                else
                {
                    if (currentLocationName == location)
                        return true;
                }
            }

            return false;
        }
    }
}
