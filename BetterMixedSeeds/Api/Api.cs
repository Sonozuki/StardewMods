namespace BetterMixedSeeds
{
    /// <summary>Provides basic crop apis.</summary>
    public class Api : IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public void ForceExcludeCrop(params string[] cropNames)
        {
            if (cropNames == null || cropNames.Length == 0)
                return;

            ModEntry.Instance.Monitor.Log($"A mod has forcibly excluded: {string.Join(", ", cropNames)}");
            ModEntry.Instance.CropsToExclude.AddRange(cropNames);
        }
    }
}
