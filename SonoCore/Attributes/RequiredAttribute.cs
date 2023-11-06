using System;

namespace SonoCore.Attributes
{
    /// <summary>Indicates the property isn't allowed to be <see langword="null"/> (or whitespace if the property is a <see langword="string"/>).</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RequiredAttribute : Attribute { }
}
