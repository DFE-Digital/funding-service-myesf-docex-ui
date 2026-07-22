namespace Pds.DocumentExchange.Web.Areas.Admin.Interfaces
{
    /// <summary>
    /// Validation Service interface.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Check string contains no special characters.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>true or false.</returns>
        bool HasNoSpecialCharacters(string input);

        /// <summary>
        /// Check string is not empty and in range.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="maxLength">Max length of string.</param>
        /// <returns>true or false.</returns>
        bool NotEmptyAndInRange(string input, int maxLength);

        /// <summary>
        /// Check int is in range.
        /// </summary>
        /// <param name="input">The input int.</param>
        /// <param name="from">Min value allowed.</param>
        /// <param name="to">Max value allowed.</param>
        /// <returns>true or false.</returns>
        bool IsInRange(int input, int from, int to);
    }
}