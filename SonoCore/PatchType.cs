namespace SonoCore
{
    /// <summary>The types of a Harmony patch.</summary>
    public enum PatchType
    {
        /// <summary>A patch that gets applied to the beginning of a method.</summary>
        Prefix,

        /// <summary>A patch that changes the IL directly.</summary>
        Transpiler,

        /// <summary>A patch that gets applied to the end of a method.</summary>
        Postfix
    }
}
