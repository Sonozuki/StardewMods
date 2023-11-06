namespace SonoCore;

/// <summary>The base of a model that can be used in a <see cref="Repository{T, TIdentifier}"/>.</summary>
public abstract class ModelBase
{
    /*********
    ** Accessors
    *********/
    /// <summary>How the model data should be interpreted.</summary>
    public Action Action { get; set; }


    /*********
    ** Public Methods
    *********/
    /// <summary>Retrieves the identifier of the item.</summary>
    /// <returns>The value of the identitifer, if one exists; otherwise, <see langword="null"/>.</returns>
    public object GetIdentifier() => this.GetType().GetIdentifierProperties().FirstOrDefault()?.GetValue(this);
}
