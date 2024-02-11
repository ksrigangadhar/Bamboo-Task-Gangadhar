using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace BabooTask.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class BambooTaskController : ControllerBase
    {
        private IMemoryCache cache;
        private static HttpClient client = new HttpClient();
        public BambooTaskController(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }      
       

        // GET: api/<BambooTask>
      
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            try
            {
                const string bestStoriesApi = "https://hacker-news.firebaseio.com/v0/beststories.json";  

                List<int> bestIds = new List<int>();

                return await cache.GetOrCreateAsync<JsonResult>("topstories",
                                async cacheEntry => {                                   
                                    var response = await client.GetAsync(bestStoriesApi);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var storiesResponse = response.Content.ReadAsStringAsync().Result;
                                        bestIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);
                                    }
                                    else
                                    {
                                        return new JsonResult(bestIds);
                                    }

                                    return new JsonResult(bestIds);
                                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //Crete cache based on id
        //to retive Single story based on ID
        [HttpGet("{Id}")]        
        public async Task<BestStories> GetStoryAsync(string Id)
        {

            const string StoryApiTemplate = "https://hacker-news.firebaseio.com/v0/item/{0}.json";

            return await cache.GetOrCreateAsync<BestStories>(Id,
                async cacheEntry => {
                    BestStories story = new BestStories();

                    var response = await client.GetAsync(string.Format(StoryApiTemplate, Id));
                    if (response.IsSuccessStatusCode)
                    {
                        var storyResponse = response.Content.ReadAsStringAsync().Result;
                        story = JsonConvert.DeserializeObject<BestStories>(storyResponse);
                    }
                    else
                    {
                        return story;
                    }

                    return story;
                });
        }



     

    }
}
