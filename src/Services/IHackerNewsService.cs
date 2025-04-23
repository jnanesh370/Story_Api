using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyDotNetWebApiApp.Services
{
    public interface IHackerNewsService
    {
        Task<IEnumerable<int>> GetNewStoryIdsAsync();
        Task<Story> GetStoryByIdAsync(int id);
        Task<IEnumerable<Story>> SearchStoriesAsync(string query);
    }
}