namespace myonAPI.Models
{
    public class ArticleInfo
    {
        public ArticleInfo(string title)
        {
            Title = title;
        }

        public string Title { get; set; }

        public string? Content { get; set; }
    }
}
