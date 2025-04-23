using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using MyDotNetWebApiApp.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using Moq.Protected;
using System.Threading;
using System.Net;

namespace my_dotnet_webapi_app.tests.UnitTests
{
    public class ServiceTests
    {
        private readonly HackerNewsService _service;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public ServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _service = new HackerNewsService(_httpClient, _memoryCache);
        }

        [Fact]
        public async Task GetNewStoryIdsAsync_ReturnsStoryIds_FromApi()
        {
            // Arrange
            var expectedStoryIds = new List<int> { 1, 2, 3, 4, 5 };
            var responseContent = JsonConvert.SerializeObject(expectedStoryIds);
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Act
            var storyIds = await _service.GetNewStoryIdsAsync();

            // Assert
            Assert.NotNull(storyIds);
            Assert.Equal(expectedStoryIds, storyIds);
        }

        [Fact]
        public async Task GetStoryByIdAsync_ReturnsStory_FromApi()
        {
            // Arrange
            var storyId = 1;
            var expectedStory = new Story { Title = "Story 1", Url = "http://example.com/1", Content = "Sample content" };
            var responseContent = JsonConvert.SerializeObject(expectedStory);
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Act
            var story = await _service.GetStoryByIdAsync(storyId);

            // Assert
            Assert.NotNull(story);
            Assert.Equal(expectedStory.Title, story.Title);
            Assert.Equal(expectedStory.Url, story.Url);
            Assert.Equal(expectedStory.Content, story.Content);
        }

        [Fact]
        public async Task SearchStoriesAsync_ReturnsMatchingStories()
        {
            // Arrange
            var query = "example";
            var storyIds = new List<int> { 1, 2 };
            var stories = new List<Story>
            {
                new Story { Title = "Example Story 1", Url = "http://example.com/1", Content = "Sample content" },
                new Story { Title = "Another Example Story", Url = "http://example.com/2", Content = "More content" }
            };

            _httpMessageHandlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(storyIds))
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(stories[0]))
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(stories[1]))
                });

            // Act
            var matchingStories = await _service.SearchStoriesAsync(query);

            // Assert
            Assert.NotNull(matchingStories);
            Assert.Equal(2, matchingStories.Count());
            Assert.Contains(matchingStories, s => s.Title == "Example Story 1");
            Assert.Contains(matchingStories, s => s.Title == "Another Example Story");
        }
    }
}