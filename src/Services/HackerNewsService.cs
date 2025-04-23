using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MyDotNetWebApiApp.Services;
using Newtonsoft.Json;

public class HackerNewsService : IHackerNewsService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private const string NewStoriesCacheKey = "NewStories";

    public HackerNewsService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<IEnumerable<int>> GetNewStoryIdsAsync()
    {
        if (!_cache.TryGetValue(NewStoriesCacheKey, out IEnumerable<int> newStoryIds))
        {
            var response = await _httpClient.GetStringAsync("https://hacker-news.firebaseio.com/v0/newstories.json");
            newStoryIds = JsonConvert.DeserializeObject<IEnumerable<int>>(response);
            // Cache the new stories for 5 minutes
            _cache.Set(NewStoriesCacheKey, newStoryIds, TimeSpan.FromMinutes(5));
        }
        return newStoryIds;
    }

    public async Task<Story> GetStoryByIdAsync(int id)
    {
        var storyUrl = $"https://hacker-news.firebaseio.com/v0/item/{id}.json";
        var response = await _httpClient.GetStringAsync(storyUrl);
        var story = JsonConvert.DeserializeObject<Story>(response);
        return story;
    }

    public async Task<IEnumerable<Story>> SearchStoriesAsync(string query)
    {
        // For demonstration, search in the first 100 new stories
        var allStoryIds = await GetNewStoryIdsAsync();
        var tasks = allStoryIds.Take(100)
            .Select(id => GetStoryByIdAsync(id));
        var stories = await Task.WhenAll(tasks);

        // Ensure stories is not null and filter out null elements
        if (stories == null)
        {
            return Enumerable.Empty<Story>();
        }

        // Simple search matching title or content
        return stories.Where(s => s != null &&
            ((s.Title?.IndexOf(query, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
             (s.Content?.IndexOf(query, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0));
    }
}

    

