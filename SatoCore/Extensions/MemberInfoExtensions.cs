using System;
using System.Linq;
using System.Reflection;

namespace SatoCore.Extensions
{
    /// <summary>Extension methods for the <see cref="MemberInfo"/> class.</summary>
    public static class MemberInfoExtensions
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Retrieves the full name of the member.</summary>
        /// <param name="memberInfo">The member instance to get the full name of.</param>
        /// <returns>The full name of the member.</returns>
        public static string GetFullName(this MemberInfo memberInfo) => $"{memberInfo.DeclaringType.FullName}.{memberInfo.Name}";

        /// <summary>Gets the first attribute of a specified type on the member.</summary>
        /// <typeparam name="T">The attribute type to get.</typeparam>
        /// <param name="memberInfo">The member instance to get the attribute from.</param>
        /// <returns>The first occurance of an attribute of type <typeparamref name="T"/>, if one exists; otherwise, <see langword="null"/>.</returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo) where T : Attribute => (T)memberInfo?.GetCustomAttributes(typeof(T), false).FirstOrDefault();
    }
}
