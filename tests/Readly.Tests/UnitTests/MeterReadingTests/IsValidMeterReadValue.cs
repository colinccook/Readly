using FluentAssertions;
using NUnit.Framework;
using Readly.Entities;

namespace Readly.Tests.UnitTests.MeterReadingTests
{
    [TestFixture]
    public class IsValidMeterReadValue
    {
        [TestCase]
        public void IsValidMeterRead_When_ReadIsFiveDigitsLong()
        {
            // Arrange / Act
            var result = MeterReading.IsValidMeterReadValue("12345");

            // Assert
            result.Should().BeTrue();
        }

        [TestCase("1234")]
        [TestCase(" 1234")]
        [TestCase("abcde")]
        [TestCase("123456")]
        [TestCase("     ")]
        [TestCase("")]
        [TestCase(null)]
        public void IsNotValidMeterRead_When_ReadIsNotFiveDigitsLong(string badRead)
        {
            // Arrange / Act
            var result = MeterReading.IsValidMeterReadValue(badRead);

            // Assert
            result.Should().BeFalse();
        }
    }
}
