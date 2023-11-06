using System;

namespace SonoCore.Attributes
{
    /// <summary>Indicates the property is editable through <see cref="Repository{T, TIdentifier}.Edit(T)"/>.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EditableAttribute : Attribute { }
}
