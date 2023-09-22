using Moq;
using Moq.Protected;
using Music.APIs.Spotify;
using System.Net;
using FluentAssertions;

namespace Music.Tests.APIs.Tests
{
    [TestFixture]
    internal sealed class SpotifyServiceTests
    {
        private SpotifyService _spotifyService;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;

        [SetUp]
        public void Setup()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        }

        private async Task SetupSpotifyService()
        {
            var json = await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "APIs.Tests\\test_access_token.json"));

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json),
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(client);
            _spotifyService = new SpotifyService(_mockHttpClientFactory.Object);
        }

        [Test]
        public async Task GetAccessTokenAsync_TokenReceived_ReturnsToken()
        {
            // Arrange
            // Act
            await SetupSpotifyService();
        }

        [Test]
        public async Task SearchTrackAsync_TrackFound_ReturnsTrack()
        {
            // Arrange
            await SetupSpotifyService();

            var json = await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "APIs.Tests\\test_api_response.json"));
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json),
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var output = await _spotifyService.SearchTrackAsync("fakeTrack");

            // Assert
            output.Should().NotBeNull();
            output?.Items.Should().HaveCount(1);
        }
    }
}