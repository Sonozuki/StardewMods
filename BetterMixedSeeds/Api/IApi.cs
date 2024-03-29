﻿using System;

namespace BetterMixedSeeds
{
    /// <summary>Provides basic crop apis.</summary>
    public interface IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Retrieves the names of all the crops that are currently forcibly excluded.</summary>
        /// <returns>The names of the crops that are currently forcibly excluded.</returns>
        public string[] GetExcludedCrops();

        /// <summary>Overrides the configuration file to foribly exclude crops.</summary>
        /// <param name="cropNames">The names of the crops to forcibly exclude.</param>
        /// <remarks>This should be used as a last resort by mod authors who have added hard to get, highly profitable crops which can't be implemented alongside BMS without destroying in-game economy.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if a save hasn't been loaded yet.</exception>
        public void ForceExcludeCrop(params string[] cropNames);

        /// <summary>Reincludes crops that have been forcibly excluded via <see cref="ForceExcludeCrop(string[])"/>.</summary>
        /// <param name="cropNames">The names of the crops to reinclude.</param>
        /// <remarks>The names of the crops that couldn't be reincluded (because they weren't excluded).</remarks>
        /// <exception cref="InvalidOperationException">Thrown if a save hasn't been loaded yet.</exception>
        public string[] ReincludeCrop(params string[] cropNames);
    }
}
