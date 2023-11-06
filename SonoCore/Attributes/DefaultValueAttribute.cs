namespace SonoCore.Attributes;

/// <summary>The default value of the member.</summary>
/// <remarks>This should be used when the model has nullable members in order to support editing previous versions while still having a default value.</remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class DefaultValueAttribute : Attribute
{
    /*********
    ** Properties
    *********/
    /// <summary>The default value of the member.</summary>
    public object DefaultValue { get; }


    /*********
    ** Constructors
    *********/
    /// <summary>Constructs an instance.</summary>
    /// <param name="defaultValue">The default value of the member.</param>
    public DefaultValueAttribute(object defaultValue)
    {
        DefaultValue = defaultValue;
    }

    /// <summary>Constructs an instance.</summary>
    /// <param name="defaultValueType">The type of the default value to create.</param>
    /// <remarks>This is used to set the default value to the initial state of an object (an empty rectangle, for example).</remarks>
    public DefaultValueAttribute(Type defaultValueType)
    {
        if (defaultValueType != null)
            DefaultValue = Activator.CreateInstance(defaultValueType);
    }
}
