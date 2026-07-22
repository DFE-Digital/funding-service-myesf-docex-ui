using Pds.Core.Utils.Interfaces;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using System;

namespace Pds.DocumentExchange.Web.Implementations.Helpers
{
    /// <inheritdoc/>
    public class DateTimeDisplayHelper : IDateTimeDisplayHelper
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeDisplayHelper"/> class.
        /// </summary>
        /// <param name="dateTimeProvider">The datetime provider.</param>
        public DateTimeDisplayHelper(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        /// <inheritdoc/>
        public string ToSentenceTimeAndDateDisplayString(DateTime dateTime)
        {
            var localDateTime = _dateTimeProvider.ConvertToUKTime(dateTime);

            return IsToday(localDateTime)
                ? $"at {FormatTimeComponent(localDateTime)} today"
                : $"at {FormatTimeComponent(localDateTime)} on {FormatDateComponent(localDateTime)}";
        }

        /// <inheritdoc/>
        public string ToTimeAndDateDisplayString(DateTime dateTime)
        {
            var localDateTime = _dateTimeProvider.ConvertToUKTime(dateTime);

            return IsToday(localDateTime)
                ? $"Today {FormatTimeComponent(localDateTime)}"
                : $"{FormatTimeComponent(localDateTime)} on {FormatDateComponent(localDateTime)}";
        }

        private static string FormatTimeComponent(DateTime localDateTime)
            => localDateTime.ToString("h:mmtt").ToLower();

        private static string FormatDateComponent(DateTime localDateTime)
            => $"{localDateTime:d MMMM yyyy}";

        private bool IsToday(DateTime localDateTime)
        {
            var today = _dateTimeProvider.Now().Date;
            var inputIsToday = localDateTime.Date == today;
            return inputIsToday;
        }
    }
}