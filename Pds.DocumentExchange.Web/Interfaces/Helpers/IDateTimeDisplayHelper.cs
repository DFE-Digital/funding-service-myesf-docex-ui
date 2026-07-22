using System;

namespace Pds.DocumentExchange.Web.Interfaces.Helpers
{
    /// <summary>
    /// Interface for DateTime display helper methods.
    /// </summary>
    public interface IDateTimeDisplayHelper
    {
        /// <summary>
        /// A method for generating a time and date display string from a
        /// <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to display.</param>
        /// <returns>The time and date display string.</returns>
        /// <remarks>Example output: "6:15pm on 29 September 2020", or "Today 8:35pm".</remarks>
        string ToTimeAndDateDisplayString(DateTime dateTime);

        /// <summary>
        /// A method for generating a time and date display string from a
        /// <see cref="DateTime"/> object, for use in sentences.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to display.</param>
        /// <returns>The sentence time and date display string.</returns>
        /// <remarks>Example output: "at 6:15pm on 29 September 2020", or "at 8:35pm today".</remarks>
        string ToSentenceTimeAndDateDisplayString(DateTime dateTime);
    }
}