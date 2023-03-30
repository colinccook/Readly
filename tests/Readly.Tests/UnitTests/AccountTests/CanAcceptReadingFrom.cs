using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Readly.Entities;

namespace Readly.Tests.UnitTests.AccountTests
{
    [TestFixture]
    public class CanAcceptReadingFrom
    {
        [TestCase]
        public void IsValidReading_When_NoPreviousReadingGiven()
        {
            // Arrange
            var fixture = new Fixture();
            var sut = fixture.Build<Account>()
                .Without(a => a.MeterReadings)
                .Create();

            // Act
            var result = sut.CanAcceptReadingFrom(DateTime.Now);

            // Assert
            result.Should().BeTrue();
        }

        [TestCase]
        public void IsValidReading_When_PreviousReadingsInThePast()
        {
            // Arrange
            var fixture = new Fixture();
            var sut = fixture.Create<Account>();
            sut.MeterReadings.ToList().ForEach(mr => mr.MeterReadingDateTime = DateTime.Now.AddDays(-1));

            // Act
            var result = sut.CanAcceptReadingFrom(DateTime.Now);

            // Assert
            result.Should().BeTrue();
        }

        [TestCase]
        public void IsNotValidReading_When_PreviousReadingsInTheFuture()
        {
            // Arrange
            var fixture = new Fixture();
            var sut = fixture.Create<Account>();
            sut.MeterReadings.ToList().ForEach(mr => mr.MeterReadingDateTime = DateTime.Now.AddDays(1));

            // Act
            var result = sut.CanAcceptReadingFrom(DateTime.Now);

            // Assert
            result.Should().BeFalse();
        }
    }
}
