using Pds.DocumentExchange.Web.Areas.Admin.Interfaces;
using System.Text.RegularExpressions;

namespace Pds.DocumentExchange.Web.Areas.Admin.Implementations
{
    /// <inheritdoc cref="IValidationService"/>
    public class ValidationService : IValidationService
    {
        private static Regex _regex = new Regex("^[a-zA-Z0-9 ]*$", RegexOptions.Compiled);

        /// <inheritdoc/>
        public bool HasNoSpecialCharacters(string input)
        {
            return !string.IsNullOrEmpty(input) ? _regex.IsMatch(input) : true;
        }

        /// <inheritdoc/>
        public bool NotEmptyAndInRange(string input, int maxLength)
        {
            return !string.IsNullOrEmpty(input) && input.Length <= maxLength;
        }

        /// <inheritdoc/>
        public bool IsInRange(int input, int from, int to)
        {
            return input >= from && input <= to;
        }
    }
}