using Microsoft.Xna.Framework;
using MoreTrees.Models;

namespace MoreTrees
{
    /// <summary>Provides basic More Trees apis.</summary>
    public interface IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Get the tree type by tree name.</summary>
        /// <param name="name">The name of the tree.</param>
        /// <returns>The tree type.</returns>
        int GetTreeType(string name);

        /// <summary>Get a <see cref="CustomTree"/> by type.</summary>
        /// <param name="type">The type of the tree.</param>
        /// <returns>The <see cref="CustomTree"/>.</returns>
        CustomTree GetTreeByType(int type);

        /// <summary>Get whether the tree at a location has been debarked.</summary>
        /// <param name="tileLocation">The location of the tree to check.</param>
        /// <returns>Whether the tree at the given location has been debarked.</returns>
        bool IsTreeDebarked(Vector2 tileLocation);
    }
}
