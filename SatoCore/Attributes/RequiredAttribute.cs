using System;

namespace SatoCore.Attributes
{
    /// <summary>Indicates a member isn't allowed to be <see langword="null"/> (or whitespace if the member is a <see langword="string"/>.)</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RequiredAttribute : Attribute { }
}
