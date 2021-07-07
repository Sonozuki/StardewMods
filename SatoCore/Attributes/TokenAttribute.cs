using System;

namespace SatoCore.Attributes
{
    /// <summary>Indicates the property is an api token.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TokenAttribute : Attribute
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the property to populate with the token value.</summary>
        public string OutputPropertyName { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructsa an instance.</summary>
        /// <param name="outputPropertyName">The name of the property to populate with the token value.</param>
        public TokenAttribute(string outputPropertyName)
        {
            OutputPropertyName = outputPropertyName;
        }
    }
}
