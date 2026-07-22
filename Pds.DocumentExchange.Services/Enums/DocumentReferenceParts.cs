namespace Pds.DocumentExchange.Services.Enums
{
    /// <summary>
    /// The document reference parts of a string containing the information separated by
    /// a special character.
    /// </summary>
    public enum DocumentReferenceParts
    {
        /// <summary>
        /// The file name part.
        /// </summary>
        FileName = 0,

        /// <summary>
        /// The batch identifier part.
        /// </summary>
        BatchIdentifier = 1,

        /// <summary>
        /// The parent batch identifier part.
        /// </summary>
        ParentBatchIdentifier = 2
    }
}