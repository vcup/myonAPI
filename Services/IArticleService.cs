using myonAPI.Models;

namespace myonAPI.Services;

public interface IArticleService
{
    public IEnumerable<ArticleInfo> Get();

    public ArticleInfo? Get(string title);

    public ArticleInfo? Get(ArticleInfo articleInfo);

    public bool Create(ArticleInfo articleInfo);

    public bool Remove(string title);

    public bool Remove(ArticleInfo articleInfo);

    public bool Update(ArticleInfo articleInfo);

    public int Count { get; }

    public bool Contains(string title);
    public bool Contains(ArticleInfo articleInfo);
}
