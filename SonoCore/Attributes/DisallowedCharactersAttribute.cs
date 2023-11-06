namespace SonoCore.Attributes;

/// <summary>Indicates the property isn't allowed to have specified characters.</summary>
/// <remarks>This can only be used on a property of type <see langword="string"/>.</remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class DisallowedCharactersAttribute : Attribute
{
    /*********
    ** Properties
    *********/
    /// <summary>The characters the property isn't allowed to contain.</summary>
    public char[] DisallowedCharacters;


    /*********
    ** Constructors
    *********/
    /// <summary>Constructs an instance.</summary>
    /// <param name="disallowedCharacters">The characters the property isn't allowed to contain.</param>
    public DisallowedCharactersAttribute(char[] disallowedCharacters)
    {
        DisallowedCharacters = disallowedCharacters ?? new char[0];
    }
}
