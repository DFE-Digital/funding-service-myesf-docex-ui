namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// Class representing a reference to a file share document.
    /// </summary>
    public class FileShareDocumentReference
    {
        private const string Separator = "::";

        /// <summary>
        /// Tries to parse the given string as a <see cref="FileShareDocumentReference"/>.
        /// </summary>
        /// <param name="value">The value to try to parse.</param>
        /// <param name="documentReference">The parsed <see cref="FileShareDocumentReference"/>, if successful.</param>
        /// <returns>A boolean indicating whether or not the parse was successful.</returns>
        public static bool TryParse(string value, out FileShareDocumentReference documentReference)
        {
            documentReference = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var split = value.Split(Separator, System.StringSplitOptions.RemoveEmptyEntries);

            if (split.Length != 2)
            {
                return false;
            }

            documentReference = new FileShareDocumentReference
            {
                Team = split[0],
                FileName = split[1]
            };

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
            => $"{Team}{Separator}{FileName}";

        /// <summary>
        /// Gets or sets the team.
        /// </summary>
        public string Team { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }
    }
}