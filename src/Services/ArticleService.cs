using System.Diagnostics;
using myonAPI.Models;

namespace myonAPI.Services;

internal class ArticleService : IArticleService
{
    private readonly MarkdownParserService _service;
    private readonly string articlesPath = "./Assets/Articles";
    private readonly HashSet<ArticleInfo> _articles = new();

    public ArticleService(MarkdownParserService service)
    {
        _service = service;
        var titles = _articles.Select(info => info.Title).ToList();
        var filenames = Directory.EnumerateFiles(articlesPath, "*.md").Select(Path.GetFileNameWithoutExtension).ToList();

        // Create all markdown article on filesystem to .net object
        var result = filenames.Except(titles)
            .Select(Create!)
            .All(result => result);
        Debug.Assert(result);
    }

    public int Count => _articles.Count;

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

        var article = new ArticleInfo
        {
            Title = title,
            Content = _service.GetHtml(articleFilePath),
            HtmlHeadingIdRelation = _service.MarkdownHeadings
        };

        return Create(article);
    }

    public bool Create(ArticleInfo articleInfo)
    {
        if (_articles.Any(info => info.Title == articleInfo.Title))
        {
            return false;
        }

        if (string.IsNullOrEmpty(articleInfo.Content))
        {
            articleInfo.Content = File.ReadAllText(Path.Join(articlesPath, articleInfo.Title + ".md"));
        }

        return _articles.Add(articleInfo);
    }
    public IEnumerable<ArticleInfo> Get()
    {
        return _articles;
    }

    public ArticleInfo? Get(string title)
    {
        return _articles.FirstOrDefault(info => info.Title == title);
    }

    public ArticleInfo? Get(ArticleInfo articleInfo)
    {
        return Get(articleInfo.Title);
    }


    public bool Remove(string title)
    {
        return _articles.RemoveWhere(info => info.Title == title) != 0;
    }

    public bool Remove(ArticleInfo articleInfo)
    {
        return Remove(articleInfo.Title);
    }

    public bool Update(ArticleInfo articleInfo)
    {
        if (!_articles.Any(info => info == articleInfo)) 
        {
            return false;
        }
            
        Remove(articleInfo);
        return _articles.Add(articleInfo);
    }

    public bool Contains(string title)
    {
        return _articles.Any(info => info.Title == title);
    }

    public bool Contains(ArticleInfo articleInfo)
    {
        return _articles.Contains(articleInfo) || _articles.Any(info => info.Title == articleInfo.Title);
    }
}