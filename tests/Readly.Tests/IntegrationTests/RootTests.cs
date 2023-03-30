using NUnit.Framework;
using FluentAssertions;

namespace Readly.Tests.IntegrationTests;

[TestFixture]
public class RootTests
{

    [Test]
    public async Task ServesStaticPage_When_AccessingRoot()
    {
        // Arrange
        using var app = new CustomWebApplicationFactory();
        var httpClient = app.CreateClient();

        // Act
        var response = await httpClient.GetAsync("/");
        var responseText = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        responseText.Should().Contain("<label for=\"file\">Select a CSV file:</label>");
    }
}