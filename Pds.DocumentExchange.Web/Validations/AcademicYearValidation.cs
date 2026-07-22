namespace Pds.DocumentExchange.Web.Validations
{
    /// <summary>
    /// The academic year validation.
    /// </summary>
    public static class AcademicYearValidation
    {
        private const int ACADEMIC_YEAR_VALID_LENGTH = 6;

        /// <summary>
        /// Determines if the academic year is valid.
        /// </summary>
        /// <param name="academicYear">The academic year.</param>
        /// <returns>True if the academic year is valid. False otherwise.</returns>
        public static bool IsValidAcademicYear(this string academicYear)
        {
            if (string.IsNullOrWhiteSpace(academicYear))
            {
                return false;
            }

            return academicYear.Length == ACADEMIC_YEAR_VALID_LENGTH
                && int.TryParse(academicYear, out _);
        }
    }
}