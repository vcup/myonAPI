namespace myonAPI.Models;

public record ArticleInfo
{
    /// <summary>
    /// title of article
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// content is html format
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// mapping html heading id and heading context
    /// </summary>
    public Dictionary<string, string>? HtmlHeadingIdRelation { get; set; }
}