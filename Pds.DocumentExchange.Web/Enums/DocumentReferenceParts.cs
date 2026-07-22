namespace Pds.DocumentExchange.Web.Enums
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
        FileName,

        /// <summary>
        /// The batch identifier part.
        /// </summary>
        BatchIdentifier,

        /// <summary>
        /// The parent batch identifier part.
        /// </summary>
        ParentBatchIdentifier
    }
}