namespace SonoCore;

/// <summary>The ways that data in a model can be interpreted.</summary>
public enum Action
{
    /// <summary>The data will be added.</summary>
    Add,

    /// <summary>The data will edit previous data.</summary>
    Edit,

    /// <summary>The data will delete previous data.</summary>
    Delete
}
