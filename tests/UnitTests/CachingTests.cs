using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using MyDotNetWebApiApp.Services;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class CachingTests
    {
        private readonly CacheProvider _cacheProvider;
        private readonly Mock<IHackerNewsService> _mockService;

        public CachingTests()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheProvider = new CacheProvider(memoryCache);
            _mockService = new Mock<IHackerNewsService>();
        }

        [Fact]
        public async Task GetNewStories_CachesDataCorrectly()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3, 4, 5 };
            _mockService.Setup(s => s.GetNewStoryIdsAsync()).ReturnsAsync(storyIds);
            var cacheKey = "newstories";

            // Act
            _cacheProvider.Set(cacheKey, storyIds, TimeSpan.FromMinutes(5));
            var cachedStoryIds = _cacheProvider.Get<List<int>>(cacheKey);

            // Assert
            Assert.NotNull(cachedStoryIds);
            Assert.Equal(5, cachedStoryIds.Count);
            Assert.Equal(1, cachedStoryIds[0]);
        }

        [Fact]
        public async Task GetStoryById_CachesDataCorrectly()
        {
            // Arrange
            var story = new Story { Title = "Story 1", Url = "http://example.com/1" };
            _mockService.Setup(s => s.GetStoryByIdAsync(1)).ReturnsAsync(story);
            var cacheKey = "story_1";

            // Act
            _cacheProvider.Set(cacheKey, story, TimeSpan.FromMinutes(5));
            var cachedStory = _cacheProvider.Get<Story>(cacheKey);

            // Assert
            Assert.NotNull(cachedStory);
            Assert.Equal("Story 1", cachedStory.Title);
            Assert.Equal("http://example.com/1", cachedStory.Url);
        }

        [Fact]
        public async Task SearchStories_CachesDataCorrectly()
        {
            // Arrange
            var query = "example";
            var stories = new List<Story>
            {
        new Story { Title = "Story 1", Url = "http://example.com/1" },
        new Story { Title = "Story 2", Url = "http://example.com/2" }
    };
            _mockService.Setup(s => s.SearchStoriesAsync(query)).ReturnsAsync(stories);
            var cacheKey = $"search_{query}";

            // Act
            _cacheProvider.Set(cacheKey, stories, TimeSpan.FromMinutes(5));
            var cachedStories = _cacheProvider.Get<List<Story>>(cacheKey);

            // Assert
            Assert.NotNull(cachedStories);
            Assert.Equal(2, cachedStories.Count);
            Assert.Equal("Story 1", cachedStories[0].Title);
        }

    }
}