namespace SonoCore.Attributes;

/// <summary>Indicates the property is the unique identifier of the model.</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class IdentifierAttribute : Attribute { }
