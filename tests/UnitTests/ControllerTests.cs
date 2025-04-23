using Microsoft.AspNetCore.Mvc;
using Moq;
using my_dotnet_webapi_app.Controllers;
using MyDotNetWebApiApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class ControllerTests
    {
        private readonly StoriesController _controller;
        private readonly Mock<IHackerNewsService> _mockService;

        public ControllerTests()
        {
            _mockService = new Mock<IHackerNewsService>();
            _controller = new StoriesController(_mockService.Object);
        }
        [Fact]
        public async Task GetNewStories_ReturnsOkResult_WithTop20StoryIds()
        {
            // Arrange
            var storyIds = Enumerable.Range(1, 30).ToList(); // Simulate 30 story IDs
            _mockService.Setup(service => service.GetNewStoryIdsAsync()).ReturnsAsync(storyIds);

            // Act
            var result = await _controller.GetNewStories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnStoryIds = Assert.IsAssignableFrom<IEnumerable<int>>(okResult.Value);
            Assert.Equal(20, returnStoryIds.Count()); // Ensure only top 20 are returned
            Assert.Equal(storyIds.Take(20), returnStoryIds);
        }

        [Fact]
        public async Task GetStory_ReturnsOkResult_WithStory()
        {
            // Arrange
            var storyId = 1;
            var expectedStory = new Story { Title = "Story 1", Url = "http://example.com/1", Content = "Sample content" };
            _mockService.Setup(service => service.GetStoryByIdAsync(storyId)).ReturnsAsync(expectedStory);

            // Act
            var result = await _controller.GetStory(storyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnStory = Assert.IsType<Story>(okResult.Value);
            Assert.Equal(expectedStory.Title, returnStory.Title);
            Assert.Equal(expectedStory.Url, returnStory.Url);
            Assert.Equal(expectedStory.Content, returnStory.Content);
        }

        [Fact]
        public async Task GetStory_ReturnsNotFound_WhenStoryDoesNotExist()
        {
            // Arrange
            var storyId = 999; // Non-existent story ID
            _mockService.Setup(service => service.GetStoryByIdAsync(storyId)).ReturnsAsync((Story)null);

            // Act
            var result = await _controller.GetStory(storyId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SearchStories_ReturnsOkResult_WithMatchingStories()
        {
            // Arrange
            var query = "example";
            var stories = new List<Story>
            {
                new Story { Title = "Example Story 1", Url = "http://example.com/1" },
                new Story { Title = "Another Example Story", Url = "http://example.com/2" }
            };
            _mockService.Setup(service => service.SearchStoriesAsync(query)).ReturnsAsync(stories);

            // Act
            var result = await _controller.SearchStories(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnObject = Assert.IsType<dynamic>(okResult.Value);
            var returnStories = Assert.IsAssignableFrom<IEnumerable<Story>>(returnObject.hits);
            Assert.Equal(2, returnStories.Count());
            Assert.Contains(returnStories, (Predicate<Story>)(s => s.Title == "Example Story 1"));
            Assert.Contains(returnStories, (Predicate<Story>)(s => s.Title == "Another Example Story"));
        }

       
    }
}