using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Utils.Interfaces;
using Pds.DocumentExchange.Web.Implementations.Helpers;
using System;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Tests.Unit.Helpers
{
    [TestClass, TestCategory("Unit")]
    public class DateTimeDisplayHelperTest
    {
        [DataTestMethod, TestCategory("Unit")]
        [DynamicData(nameof(ToTimeAndDateDisplayString_ForPastDate_TestData))]
        public void ToTimeAndDateDisplayString_ForPastDate_FormatsDateAndTimeAsExpected(
            DateTime inputDateTime, string expectedTimeAndDateString)
        {
            // Arrange
            var dateTimeProvider = Mock.Of<IDateTimeProvider>(MockBehavior.Strict);
            var dateTimeHelper = new DateTimeDisplayHelper(dateTimeProvider);

            var mockDateTimeProvider = Mock.Get(dateTimeProvider);

            mockDateTimeProvider
                .Setup(p => p.ConvertToUKTime(inputDateTime))
                .Returns(inputDateTime);

            mockDateTimeProvider
                .Setup(p => p.Now())
                .Returns(DateTime.MaxValue);

            // Act
            var actual = dateTimeHelper.ToTimeAndDateDisplayString(inputDateTime);

            // Assert
            actual.Should().Be(expectedTimeAndDateString);
            mockDateTimeProvider.VerifyAll();
        }

        [DataTestMethod, TestCategory("Unit")]
        [DynamicData(nameof(ToTimeAndDateDisplayString_ForToday_TestData))]
        public void ToTimeAndDateDisplayString_ForToday_FormatsDateAndTimeAsExpected(
            DateTime inputDateTime, string expectedTimeAndDateString)
        {
            // Arrange
            var dateTimeProvider = Mock.Of<IDateTimeProvider>(MockBehavior.Strict);
            var dateTimeHelper = new DateTimeDisplayHelper(dateTimeProvider);

            var mockDateTimeProvider = Mock.Get(dateTimeProvider);

            mockDateTimeProvider
                .Setup(p => p.ConvertToUKTime(inputDateTime))
                .Returns(inputDateTime);

            mockDateTimeProvider
                .Setup(p => p.Now())
                .Returns(inputDateTime);

            // Act
            var actual = dateTimeHelper.ToTimeAndDateDisplayString(inputDateTime);

            // Assert
            actual.Should().Be(expectedTimeAndDateString);
            mockDateTimeProvider.VerifyAll();
        }

        [DataTestMethod, TestCategory("Unit")]
        [DynamicData(nameof(ToSentenceTimeAndDateDisplayString_ForPastDate_TestData))]
        public void ToSentenceTimeAndDateDisplayString_ForPastDate_FormatsDateAndTimeAsExpected(
            DateTime inputDateTime, string expectedTimeAndDateString)
        {
            // Arrange
            var dateTimeProvider = Mock.Of<IDateTimeProvider>(MockBehavior.Strict);
            var dateTimeHelper = new DateTimeDisplayHelper(dateTimeProvider);

            var mockDateTimeProvider = Mock.Get(dateTimeProvider);

            mockDateTimeProvider
                .Setup(p => p.ConvertToUKTime(inputDateTime))
                .Returns(inputDateTime);

            mockDateTimeProvider
                .Setup(p => p.Now())
                .Returns(DateTime.MaxValue);

            // Act
            var actual = dateTimeHelper.ToSentenceTimeAndDateDisplayString(inputDateTime);

            // Assert
            actual.Should().Be(expectedTimeAndDateString);
            mockDateTimeProvider.VerifyAll();
        }

        [DataTestMethod, TestCategory("Unit")]
        [DynamicData(nameof(ToSentenceTimeAndDateDisplayString_ForToday_TestData))]
        public void ToSentenceTimeAndDateDisplayString_ForToday_FormatsDateAndTimeAsExpected(
            DateTime inputDateTime, string expectedTimeAndDateString)
        {
            // Arrange
            var dateTimeProvider = Mock.Of<IDateTimeProvider>(MockBehavior.Strict);
            var dateTimeHelper = new DateTimeDisplayHelper(dateTimeProvider);

            var mockDateTimeProvider = Mock.Get(dateTimeProvider);

            mockDateTimeProvider
                .Setup(p => p.ConvertToUKTime(inputDateTime))
                .Returns(inputDateTime);

            mockDateTimeProvider
                .Setup(p => p.Now())
                .Returns(inputDateTime);

            // Act
            var actual = dateTimeHelper.ToSentenceTimeAndDateDisplayString(inputDateTime);

            // Assert
            actual.Should().Be(expectedTimeAndDateString);
            mockDateTimeProvider.VerifyAll();
        }

        private static IEnumerable<object> ToTimeAndDateDisplayString_ForPastDate_TestData
            => new[]
            {
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 24, 23, 59, 59), DateTimeKind.Utc), "11:59pm on 24 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 0, 0, 0), DateTimeKind.Utc), "12:00am on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 0, 59, 59), DateTimeKind.Utc), "12:59am on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 1, 0, 0), DateTimeKind.Utc), "1:00am on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 11, 59, 59), DateTimeKind.Utc), "11:59am on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 12, 0, 0), DateTimeKind.Utc), "12:00pm on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 12, 59, 59), DateTimeKind.Utc), "12:59pm on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 13, 0, 0), DateTimeKind.Utc), "1:00pm on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 4, 1, 23, 59, 59), DateTimeKind.Utc), "11:59pm on 1 April 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 4, 2, 0, 0, 0), DateTimeKind.Utc), "12:00am on 2 April 2020" }
            };

        private static IEnumerable<object> ToTimeAndDateDisplayString_ForToday_TestData
            => new[]
            {
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 24, 23, 59, 59), DateTimeKind.Utc), "Today 11:59pm" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 0, 0, 0), DateTimeKind.Utc), "Today 12:00am" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 0, 59, 59), DateTimeKind.Utc), "Today 12:59am" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 1, 0, 0), DateTimeKind.Utc), "Today 1:00am" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 11, 59, 59), DateTimeKind.Utc), "Today 11:59am" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 12, 0, 0), DateTimeKind.Utc), "Today 12:00pm" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 12, 59, 59), DateTimeKind.Utc), "Today 12:59pm" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 13, 0, 0), DateTimeKind.Utc), "Today 1:00pm" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 4, 1, 23, 59, 59), DateTimeKind.Utc), "Today 11:59pm" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 4, 2, 0, 0, 0), DateTimeKind.Utc), "Today 12:00am" }
            };

        private static IEnumerable<object> ToSentenceTimeAndDateDisplayString_ForPastDate_TestData
            => new[]
            {
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 24, 23, 59, 59), DateTimeKind.Utc), "at 11:59pm on 24 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 0, 0, 0), DateTimeKind.Utc), "at 12:00am on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 0, 59, 59), DateTimeKind.Utc), "at 12:59am on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 1, 0, 0), DateTimeKind.Utc), "at 1:00am on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 11, 59, 59), DateTimeKind.Utc), "at 11:59am on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 12, 0, 0), DateTimeKind.Utc), "at 12:00pm on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 12, 59, 59), DateTimeKind.Utc), "at 12:59pm on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 13, 0, 0), DateTimeKind.Utc), "at 1:00pm on 25 December 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 4, 1, 23, 59, 59), DateTimeKind.Utc), "at 11:59pm on 1 April 2020" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 4, 2, 0, 0, 0), DateTimeKind.Utc), "at 12:00am on 2 April 2020" }
            };

        private static IEnumerable<object> ToSentenceTimeAndDateDisplayString_ForToday_TestData
            => new[]
            {
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 24, 23, 59, 59), DateTimeKind.Utc), "at 11:59pm today" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 0, 0, 0), DateTimeKind.Utc), "at 12:00am today" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 0, 59, 59), DateTimeKind.Utc), "at 12:59am today" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 1, 0, 0), DateTimeKind.Utc), "at 1:00am today" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 11, 59, 59), DateTimeKind.Utc), "at 11:59am today" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 12, 0, 0), DateTimeKind.Utc), "at 12:00pm today" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 12, 59, 59), DateTimeKind.Utc), "at 12:59pm today" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 12, 25, 13, 0, 0), DateTimeKind.Utc), "at 1:00pm today" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 4, 1, 23, 59, 59), DateTimeKind.Utc), "at 11:59pm today" },
                new object[] { DateTime.SpecifyKind(new DateTime(2020, 4, 2, 0, 0, 0), DateTimeKind.Utc), "at 12:00am today" }
            };
    }
}