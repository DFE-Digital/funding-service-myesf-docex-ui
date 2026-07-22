namespace Pds.DocumentExchange.Web.Validations
{
    /// <summary>
    /// The UKPRN validation.
    /// </summary>
    public static class UkprnValidation
    {
        private const int UKPRN_VALID_LENGTH = 8;

        /// <summary>
        /// Determines if the UKPRN is valid.
        /// </summary>
        /// <param name="ukprn">The UKPRN.</param>
        /// <returns>True if the UKPRN is valid. False otherwise.</returns>
        public static bool IsValidUkprn(this string ukprn)
        {
            if (string.IsNullOrWhiteSpace(ukprn))
            {
                return false;
            }

            return ukprn.Length == UKPRN_VALID_LENGTH
                && int.TryParse(ukprn, out _);
        }
    }
}