using System;
using System.Reflection;

namespace SatoCore.Extensions
{
    /// <summary>Extension methods for the <see cref="Type"/> class.</summary>
    public static class TypeExtensions
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Retrieves all the instance properties of the type.</summary>
        /// <param name="type">The type whose instance properties should be retrieved.</param>
        /// <returns>All the instance properties of <paramref name="type"/>.</returns>
        public static PropertyInfo[] GetInstanceProperties(this Type type) => type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
}
