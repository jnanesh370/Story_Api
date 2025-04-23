using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyDotNetWebApiApp.Services;
using System.Linq;

namespace my_dotnet_webapi_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;

        public StoriesController(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet("newstories")]
        public async Task<ActionResult<IEnumerable<int>>> GetNewStories()
        {
            var storyIds = await _hackerNewsService.GetNewStoryIdsAsync();
            return Ok(storyIds.Take(20));
        }

        [HttpGet("story/{id}")]
        public async Task<ActionResult<Story>> GetStory(int id)
        {
            var story = await _hackerNewsService.GetStoryByIdAsync(id);
            if (story == null)
            {
                return NotFound();
            }
            return Ok(story);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Story>>> SearchStories([FromQuery] string query)
        {
            var stories = await _hackerNewsService.SearchStoriesAsync(query);

            // Handle null or empty results
            if (stories == null || !stories.Any())
            {
                return NotFound(new { message = "No stories found matching the query." });
            }

            return Ok(new { hits = stories });
        }
    }
}