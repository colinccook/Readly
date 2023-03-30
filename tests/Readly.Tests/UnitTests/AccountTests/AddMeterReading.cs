using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Readly.Entities;

namespace Readly.Tests.UnitTests.AccountTests
{
    [TestFixture]
    public class AddMeterReading
    {
        [TestCase]
        public void CannotAddMeterReading_When_PreviousReadingsInThePast()
        {
            // Arrange
            var fixture = new Fixture();
            var sut = fixture.Create<Account>();
            sut.MeterReadings.ToList().ForEach(mr => mr.MeterReadingDateTime = DateTime.Now.AddDays(1));

            var newReading = fixture.Build<MeterReading>()
                .With(mr => mr.MeterReadingDateTime, DateTime.Now.AddDays(-1))
                .Create();

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() => sut.AddMeterReading(newReading));
        }

        [TestCase]
        public void HappyPath()
        {
            // Arrange
            var fixture = new Fixture();
            var sut = fixture.Create<Account>();
            sut.MeterReadings.ToList().ForEach(mr => mr.MeterReadingDateTime = DateTime.Now.AddDays(-1));

            var newReading = fixture.Build<MeterReading>()
                .With(mr => mr.MeterReadingDateTime, DateTime.Now.AddDays(1))
                .Create();

            // Act 
            sut.AddMeterReading(newReading);

            // Assert
            sut.MeterReadings.ToList().Should().HaveCount(4);
            sut.MeterReadings.ToList().MaxBy(mr => mr.MeterReadingDateTime).Should().BeEquivalentTo(newReading);
        }
    }
}
