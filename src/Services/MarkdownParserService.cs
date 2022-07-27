using Markdig;
using myonAPI.Services.MarkdownParser;

namespace myonAPI.Services;

internal class MarkdownParserService
{
    private static readonly MarkdownPipelineBuilder PipelineBuilder = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseGridTables()
        .UseEmojiAndSmiley();

    public IDictionary<string, string> MarkdownHeadings { get; }

    private readonly MarkdownPipeline _pipeline;

    public MarkdownParserService()
    {
        var autoId = new AutoIdExtension(AutoIdExtensionOption.UseAutoPrefix);
        _pipeline = PipelineBuilder
            .Use(autoId)
            .Use<AlwaysAddOneForHeadingLevelExtension>()
            .Use<LinkHeading>()
            .Build();
        MarkdownHeadings = new Dictionary<string, string>();
    }

    public string GetHtml(int key, TextReader markdown)
    {
        // 如果使用私有字段与构造函数共享该对象
        // 第一个对 autoId.WhenSetId 事件附加处理的实例会一直被调用
        // 其他实例对该事件附加处理程序这不会生效
        var autoId = _pipeline.Extensions.Find<AutoIdExtension>()!;

        autoId.Prefix = key.ToString();

        autoId.WhenSetId += AddHeading;

        var result = Markdown.ToHtml(markdown.ReadToEnd(), _pipeline);

        autoId.WhenSetId -= AddHeading;

        return result;
    }

    private void AddHeading(string id, string heading) =>
        MarkdownHeadings.TryAdd(id, Markdown.ToHtml(heading, _pipeline));
}