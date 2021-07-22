namespace SatoCore
{
    /// <summary>The base of a model that can be used in a <see cref="Repository{T, TIdentifier}"/>.</summary>
    public abstract class ModelBase
    {
        /*********
        ** Accessors
        *********/
        /// <summary>How the model data should be interpreted.</summary>
        public Action Action { get; set; }
    }
}
