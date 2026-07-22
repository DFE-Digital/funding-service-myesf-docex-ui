using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Pds.DocumentExchange.Web.Helpers
{
    /// <summary>
    /// Helper class containing common lightweight functions for computing content.
    /// </summary>
    public static class ContentHelper
    {
        private static readonly Regex FileNameBreakPointRegex = new Regex(@"[_\.][a-zA-Z0-9]+", RegexOptions.Compiled);

        /// <summary>
        /// Gets a message relating to a count of documents, which makes sense in English.
        /// </summary>
        /// <param name="count">The count of documents.</param>
        /// <param name="messageFormat">The message format string, with tokens as follows:
        /// <list type="bullet">
        /// <item>{is/are} - will be replaced by "is" if the count is 1, otherwise "are".</item>
        /// <item>{this/these} - will be replaced by "this" if the count is 1, otherwise "these".</item>
        /// <item>{count} - will be replaced by the count of documents.</item>
        /// <item>{document(s)} - will be replaced by "document" if the count is 1, otherwise "documents".</item>
        /// </list>
        /// </param>
        /// <returns>The document count message.</returns>
        public static string GetDocumentCountMessage(int count, string messageFormat)
        {
            var isOrAre = count == 1
                ? "is"
                : "are";

            var thisOrThese = count == 1
                ? "this"
                : "these";

            var documents = count == 1
                ? "document"
                : "documents";

            return messageFormat
                .Replace("{is/are}", isOrAre)
                .Replace("{this/these}", thisOrThese)
                .Replace("{count}", count.ToString())
                .Replace("{document(s)}", documents);
        }

        /// <summary>
        /// Gets a message relating to a version of document.
        /// </summary>
        /// <param name="isAllVersions">The bool value for identifying is all versions.</param>
        /// <param name="version">The version of document.</param>
        /// <param name="messageFormat">The message format string, with tokens as follows:
        /// <list type="bullet">
        /// <item>{version} - will be replaced by "all versions" if all, otherwise "version verson number".</item>
        /// </list>
        /// </param>
        /// <returns>The document version message.</returns>
        public static string GetDocumentVersionMessage(bool isAllVersions, string version, string messageFormat)
        {
            var versionReplaceText = isAllVersions
                ? "all versions"
                : "version " + version;

            return messageFormat.Replace("{versions}", versionReplaceText);
        }

        /// <summary>
        /// Inserts HTML word break opportunity (wbr) tags into a file name
        /// immediately before each occurring underscore or period character.
        /// </summary>
        /// <param name="fileName">The input file name.</param>
        /// <returns>The HTML string representing the file name, with breaks.</returns>
        public static string InsertBreaksIntoFileName(string fileName)
        {
            var matches = FileNameBreakPointRegex.Matches(fileName);

            // Using stack because we want to insert the breaks in reverse order
            // from the end of the string, so that the indexes won't change:
            var insertionPoints = new Stack<int>();
            foreach (Match match in matches)
            {
                insertionPoints.Push(match.Index);
            }

            while (insertionPoints.TryPop(out int insertionPoint))
            {
                fileName = fileName.Insert(insertionPoint, "<wbr>");
            }

            return fileName;
        }

        /// <summary>
        /// Gets the file extension note to display after a file name.
        /// E.g. for "document.pdf", the note will be "(PDF)".
        /// </summary>
        /// <param name="fileName">The input file name.</param>
        /// <returns>The file extension note.</returns>
        public static string GetFileExtensionNote(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(fileExtension))
            {
                return string.Empty;
            }

            return $"({fileExtension.TrimStart('.')})".ToUpper();
        }
    }
}