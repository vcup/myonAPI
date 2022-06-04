using myonAPI.Models;

namespace myonAPI.Services;

public interface IArticleService
{
    public IEnumerable<ArticleDescriptor> Get();

    public ArticleDescriptor? Get(int id);

    public ArticleDescriptor? Get(ArticleDescriptor articleContent);

    public bool Create(ArticleDescriptor articleContent);

    public bool Remove(int id);

    public bool Remove(ArticleDescriptor articleContent);

    public bool Update(ArticleDescriptor articleContent);

    public int Count { get; }

    public bool Contains(int id);
    public bool Contains(ArticleDescriptor articleContent);
}