using myonAPI.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace myonAPI.Services;

internal class ArticleService : IArticleService
{
    private const string ArticleIndexFile = "./Assets/Articles/Index.yaml";
    private readonly HashSet<ArticleDescriptor> _articles = new();

    public ArticleService(IServiceProvider provider)
    {
        using var indexReadStream = File.OpenText(ArticleIndexFile);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        var descriptors = deserializer.Deserialize<IEnumerable<ArticleDescriptor>>(indexReadStream);
        var articleStartPath = Path.GetDirectoryName(ArticleIndexFile);
        foreach (var descriptor in descriptors)
        {
            descriptor.ContentFilePath = Path.Join(articleStartPath, descriptor.ContentFilePath);
            descriptor.PicturePath = Path.Join(articleStartPath, descriptor.PicturePath);
            descriptor.Build(provider.GetRequiredService<MarkdownParserService>());
            _articles.Add(descriptor);
        }
    }

    public int Count => _articles.Count;

    public bool Create(ArticleDescriptor articleContent)
    {
        if (_articles.Any(info => info.Title == articleContent.Title))
        {
            return false;
        }

        if (string.IsNullOrEmpty(articleContent.Content))
        {
            articleContent.Content = File.ReadAllText(Path.Join(ArticleIndexFile, articleContent.Title + ".md"));
        }

        return _articles.Add(articleContent);
    }

    public IEnumerable<ArticleDescriptor> Get()
    {
        return _articles;
    }

    public ArticleDescriptor? Get(int id)
    {
        return _articles.FirstOrDefault(info => info.Id == id);
    }

    public ArticleDescriptor? Get(ArticleDescriptor articleContent)
    {
        return Get(articleContent.Id);
    }

    public bool Remove(int id)
    {
        return _articles.RemoveWhere(info => info.Id == id) != 0;
    }

    public bool Remove(ArticleDescriptor articleContent)
    {
        return Remove(articleContent.Id);
    }

    public bool Update(ArticleDescriptor articleContent)
    {
        if (!Contains(articleContent.Id))
        {
            return false;
        }

        Remove(articleContent);
        return _articles.Add(articleContent);
    }

    public bool Contains(int id)
    {
        return _articles.Any(info => info.Id == id);
    }

    public bool Contains(ArticleDescriptor articleContent)
    {
        return _articles.Contains(articleContent) || _articles.Any(info => info.Title == articleContent.Title);
    }
}