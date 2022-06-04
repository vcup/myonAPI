using myonAPI.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace myonAPI.Services;

internal class ArticleService : IArticleService
{
    private const string ArticleIndexFile = "./Assets/Articles/Index.yaml";
    public readonly List<ArticleDescriptor> Articles;

    public ArticleService(IServiceProvider provider)
    {
        using var indexReadStream = File.OpenText(ArticleIndexFile);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        var descriptors = deserializer.Deserialize<List<ArticleDescriptor>>(indexReadStream);
        var articleStartPath = Path.GetDirectoryName(ArticleIndexFile);
        foreach (var descriptor in descriptors)
        {
            descriptor.ContentFilePath = Path.Join(articleStartPath, descriptor.ContentFilePath);
            descriptor.PicturePath = Path.Join(articleStartPath, descriptor.PicturePath);
            descriptor.Build(provider.GetRequiredService<MarkdownParserService>());
        }

        Articles = descriptors;
    }

    public int Count => Articles.Count;

    public bool Create(ArticleDescriptor articleContent)
    {
        if (Articles.Any(info => info.Title == articleContent.Title))
        {
            return false;
        }

        if (string.IsNullOrEmpty(articleContent.Content))
        {
            articleContent.Content = File.ReadAllText(Path.Join(ArticleIndexFile, articleContent.Title + ".md"));
        }

        Articles.Add(articleContent);
        return true;
    }

    public IEnumerable<ArticleDescriptor> Get()
    {
        return Articles;
    }

    public ArticleDescriptor? Get(int id)
    {
        return Articles.FirstOrDefault(info => info.Id == id);
    }

    public ArticleDescriptor? Get(ArticleDescriptor articleContent)
    {
        return Get(articleContent.Id);
    }

    public bool Remove(int id)
    {
        return Articles.RemoveAll(descriptor => descriptor.Id == id) != 0;
    }

    public bool Remove(ArticleDescriptor articleContent)
    {
        return Articles.Remove(articleContent);
    }

    public bool Update(ArticleDescriptor articleContent)
    {
        if (!Remove(articleContent))
        {
            return false;
        }

        Articles.Add(articleContent);
        return true;
    }

    public bool Contains(int id)
    {
        return Articles.Any(info => info.Id == id);
    }

    public bool Contains(ArticleDescriptor articleContent)
    {
        return Articles.Contains(articleContent) || Articles.Any(info => info.Title == articleContent.Title);
    }
}