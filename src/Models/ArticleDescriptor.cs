using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace myonAPI.Models;

public record ArticleDescriptor
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string SubTitle { get; set; } = string.Empty;
    
    [JsonIgnore]
    public string ContentFilePath { get; set; } = string.Empty;
    
    [JsonIgnore]
    public string PicturePath { get; set; } = string.Empty;

    /// <summary>
    /// content is html format
    /// </summary>
    [YamlIgnore]
    public string? Content { get; set; }

    /// <summary>
    /// mapping html heading id and heading context
    /// </summary>
    [YamlIgnore]
    public IDictionary<string, string>? HtmlHeadingIdRelation { get; set; }
}