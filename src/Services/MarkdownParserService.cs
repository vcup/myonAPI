using Markdig;
using myonAPI.Services.MarkdownParser;

namespace myonAPI.Services;

internal class MarkdownParserService
{
    private readonly UniqueNumberService _service;

    private static readonly MarkdownPipelineBuilder PipelineBuilder = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseGridTables()
        .UseEmojiAndSmiley();

    public Dictionary<string, string> MarkdownHeadings { get; set; } = null!;

    private readonly MarkdownPipeline _pipeline;

    public MarkdownParserService(UniqueNumberService service)
    {
        _service = service;

        var autoId = new AutoIdExtension(AutoIdExtensionOption.EnableAll);
        _pipeline = PipelineBuilder.Use(autoId).Build();
    }

    public string GetHtml(string path)
    {
        using var fileReader = File.OpenText(path);

        var key = Path.GetFileNameWithoutExtension(path);
        return GetHtml(key, fileReader.ReadToEnd());
    }

    public string GetHtml(string key, string markdown)
    {
        MarkdownHeadings = new();

        // 如果使用私有字段与构造函数共享该对象
        // 第一个对 autoId.WhenSetId 事件附加处理的实例会一直被调用
        // 其他实例对该事件附加处理程序这不会生效
        var autoId = _pipeline.Extensions.Find<AutoIdExtension>()!;

        _service.TryGetNumber(key, out var n);
        autoId.Prefix = n.ToString();

        autoId.WhenSetId += AddHeading;

        var result = Markdown.ToHtml(markdown, _pipeline);

        autoId.WhenSetId -= AddHeading;

        return result;
    }

    private void AddHeading(string id, string heading) => MarkdownHeadings.TryAdd(id, heading);
}