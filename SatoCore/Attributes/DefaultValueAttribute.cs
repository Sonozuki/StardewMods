using System;

namespace SatoCore.Attributes
{
    /// <summary>The default value of the member.</summary>
    /// <remarks>This should be used when the model has nullable members in order to support editing previous versions while still having a default value.</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DefaultValueAttribute : Attribute
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The default value of the member.</summary>
        public object DefaultValue { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="defaultValue">The default value of the member.</param>
        public DefaultValueAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}
