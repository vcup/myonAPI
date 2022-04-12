using myonAPI.Models;
using System.Text.Json;
using System.Text;

namespace myonAPI.Services
{
    public class ArticleService : IArticleService
    {
        private string articlesPath = "./Assets/Articles";
        private string indexFilePath = "./Assets/Articles/Index.json";
        private HashSet<ArticleInfo> articles;

        public ArticleService()
        {
            if (!File.Exists(indexFilePath))
            {
                using FileStream fs = File.Create(indexFilePath);
                var initByte = new UTF8Encoding(true).GetBytes("[]");
                fs.Write(initByte, 0, initByte.Length);
            }
            using FileStream indexFile = File.OpenRead(indexFilePath);
            articles = JsonSerializer.Deserialize<HashSet<ArticleInfo>>(indexFile) ?? new();

            var titles = articles.Select(info => info.Title).ToList();
            var filenames = Directory.EnumerateFiles(articlesPath, "*.md").Select(Path.GetFileNameWithoutExtension).ToList();

            var results = filenames.Except(titles).Select(Create!).ToList();
        }

        public int Count => articles.Count;

        public bool Create(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return false;
            }
            
            var articleFilePath = Path.Join(articlesPath, title + ".md");
            if (!File.Exists(articleFilePath))
            {
                return false;
            }

            ArticleInfo article = new(title);
            article.Content = File.ReadAllText(articleFilePath);

            return Create(article);
        }

        public bool Create(ArticleInfo articleInfo)
        {
            if (articles.Where(info => info.Title == articleInfo.Title).Any())
                return false;

            if (string.IsNullOrEmpty(articleInfo.Content))
            {
                articleInfo.Content = File.ReadAllText(Path.Join(articlesPath, articleInfo.Title));
            }

            return articles.Add(articleInfo);
        }
        public IEnumerable<ArticleInfo> Get()
        {
            return articles;
        }

        public ArticleInfo? Get(string title)
        {
            return articles.FirstOrDefault(info => info.Title == title);
        }

        public ArticleInfo? Get(ArticleInfo articleInfo)
        {
            return Get(articleInfo.Title);
        }


        public bool Remove(string title)
        {
            return articles.RemoveWhere(info => info.Title == title) != 0;
        }

        public bool Remove(ArticleInfo articleInfo)
        {
            return Remove(articleInfo.Title);
        }

        public bool Update(ArticleInfo articleInfo)
        {
            if (!articles.Where(info => info.Title == articleInfo.Title).Any()) 
            {
                return false;
            }
            else if (!articles.Where(info => info.Content == articleInfo.Content).Any()) 
            {
                return false;
            }
            else
            {
                Remove(articleInfo);
            }

            return articles.Add(articleInfo);
        }

        public bool Contains(string title)
        {
            return articles.Where(info => info.Title == title).Any();
        }

        public bool Contains(ArticleInfo articleInfo)
        {
            if (articles.Contains(articleInfo))
            {
                return true;
            }
            return articles.Where(info => info.Title == articleInfo.Title).Any();
        }
    }
}
