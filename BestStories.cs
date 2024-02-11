namespace BabooTask
{
    public class BestStories
    {
        public string title { get; set; }
        public string uri { get; set; }
        public string postedBy { get; set; }
        public int time { get; set; }
        public int score { get; set; }       
        public int commentCount { get; set; }
        public List<int> kids { get; set; }
    }
}
