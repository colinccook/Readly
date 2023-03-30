using NUnit.Framework;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Readly.DTOs;
using Newtonsoft.Json;

namespace Readly.Tests.IntegrationTests;

[TestFixture]
public class MeterReadingUploadsTests
{
    private static T FromResponse<T>(string response)
    {
        return JsonConvert.DeserializeObject<T>(response);
    }

    [TestCase]
    public async Task HappyPath()
    {
        // Arrange
        using var app = new CustomWebApplicationFactory();
        var httpClient = app.CreateClient();

        using var file = File.OpenRead(@"TestFiles\Meter_Reading.csv");
        using var content = new StreamContent(file);
        using var formData = new MultipartFormDataContent
        {
            { content, "files", "test.csv" }
        };

        // Act
        var response = await httpClient.PostAsync("/meter-reading-uploads", formData);
        var responseText = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // customer 2345 has a single valid reading
        var customer = app.DatabaseContext.Customers.Include(c => c.MeterReadings).Single(c => c.AccountId == 2345);
        var reading = customer.MeterReadings.Should().ContainSingle().Subject;
        reading.Should().BeEquivalentTo(new { MeterReadingDateTime = new DateTime(2019, 04, 22, 12, 25, 0), MeterReadValue = "45522" });
    }

    [TestCase]
    public async Task CountsAsAFailedRead_When_AccountDoesNotExist()
    {
        // Arrange
        using var app = new CustomWebApplicationFactory();
        var httpClient = app.CreateClient();

        using var file = File.OpenRead(@"TestFiles\NonMatchingAccount.csv");
        using var content = new StreamContent(file);
        using var formData = new MultipartFormDataContent
        {
            { content, "files", "test.csv" }
        };

        // Act
        var response = await httpClient.PostAsync("/meter-reading-uploads", formData);
        var body = await response.Content.ReadAsStringAsync();
        var result = FromResponse<MeterReadingsUploadResult>(body);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        result.SuccessfulReads.Should().Be(0);
        result.FailedReads.Should().Be(1);
    }

    [TestCase]
    public async Task CountsAsAFailedRead_When_DateFormatIsBad()
    {
        // Arrange
        using var app = new CustomWebApplicationFactory();
        var httpClient = app.CreateClient();

        using var file = File.OpenRead(@"TestFiles\BadDateFormat.csv");
        using var content = new StreamContent(file);
        using var formData = new MultipartFormDataContent
        {
            { content, "files", "test.csv" }
        };

        // Act
        var response = await httpClient.PostAsync("/meter-reading-uploads", formData);
        var body = await response.Content.ReadAsStringAsync();
        var result = FromResponse<MeterReadingsUploadResult>(body);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        result.SuccessfulReads.Should().Be(0);
        result.FailedReads.Should().Be(1);
    }

    [TestCase]
    public async Task CountsAsAFailedRead_When_MeterReadValueIsBad()
    {
        // Arrange
        using var app = new CustomWebApplicationFactory();
        var httpClient = app.CreateClient();

        using var file = File.OpenRead(@"TestFiles\BadMeterReadValue.csv");
        using var content = new StreamContent(file);
        using var formData = new MultipartFormDataContent
        {
            { content, "files", "test.csv" }
        };

        // Act
        var response = await httpClient.PostAsync("/meter-reading-uploads", formData);
        var body = await response.Content.ReadAsStringAsync();
        var result = FromResponse<MeterReadingsUploadResult>(body);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        result.SuccessfulReads.Should().Be(0);
        result.FailedReads.Should().Be(1);
    }

    [TestCase]
    public async Task CountsAsAFailedRead_When_SameEntryProvidedTwice()
    {
        // Arrange
        using var app = new CustomWebApplicationFactory();
        var httpClient = app.CreateClient();

        using var file = File.OpenRead(@"TestFiles\SameEntryTwice.csv");
        using var content = new StreamContent(file);
        using var formData = new MultipartFormDataContent
        {
            { content, "files", "test.csv" }
        };

        // Act
        var response = await httpClient.PostAsync("/meter-reading-uploads", formData);
        var body = await response.Content.ReadAsStringAsync();
        var result = FromResponse<MeterReadingsUploadResult>(body);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        result.SuccessfulReads.Should().Be(1);
        result.FailedReads.Should().Be(1);

        // customer 2345 has a single valid reading
        var customer = app.DatabaseContext.Customers.Include(c => c.MeterReadings).Single(c => c.AccountId == 2345);
        var reading = customer.MeterReadings.Should().ContainSingle().Subject;
        reading.Should().BeEquivalentTo(new { MeterReadingDateTime = new DateTime(2019, 04, 22, 12, 25, 0), MeterReadValue = "45522" });
    }
}