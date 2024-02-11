using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace BabooTask.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class BambooTaskControllerSingelCall : ControllerBase
    {
        private IMemoryCache cache;
        private static HttpClient client = new HttpClient();
        public BambooTaskControllerSingelCall(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }      
       

        // GET: api/<BambooTask>
      
        [HttpGet("{searchString}")]
        public async Task<JsonResult> Get(string searchString)
        {
            try
            {
                const string BestStoriesApi = "https://hacker-news.firebaseio.com/v0/beststories.json";                

                List<BestStories> stories = new List<BestStories>();

                var response = await client.GetAsync(BestStoriesApi);
                if (response.IsSuccessStatusCode)
                {
                    var storiesResponse = response.Content.ReadAsStringAsync().Result;
                    var bestIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);

                    var tasks = bestIds.Select(GetStoryAsync);
                    stories = (await Task.WhenAll(tasks)).ToList();

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        var search = searchString.ToLower();
                       
                        stories = stories.OrderByDescending(s=>s.score).Take(10).ToList();
                    }
                    else
                    {
                       
                    }

                }
                return new JsonResult(stories);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async Task<BestStories> GetStoryAsync(int storyId)
        {

            const string StoryApiTemplate = "https://hacker-news.firebaseio.com/v0/item/{0}.json";

            return await cache.GetOrCreateAsync<BestStories>(storyId,
                async cacheEntry => {
                    BestStories story = new BestStories();

                    var response = await client.GetAsync(string.Format(StoryApiTemplate, storyId));
                    if (response.IsSuccessStatusCode)
                    {
                        var storyResponse = response.Content.ReadAsStringAsync().Result;
                        story = JsonConvert.DeserializeObject<BestStories>(storyResponse);
                    }
                    else
                    {                      
                        
                    }

                    return story;
                });
        }

      
      
    }
}
