using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace myonAPI.Services.MarkdownParser;

internal class AutoIdExtension : IMarkdownExtension
{
    private readonly AutoIdExtensionOption _option;

    private const string AutoIdExtensionKey = "AutoIDExtension";
    public string? Prefix { get; set; }

    public AutoIdExtension(AutoIdExtensionOption option)
    {
        _option = option;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        var headingBlockParser = pipeline.BlockParsers.Find<HeadingBlockParser>();
        if (headingBlockParser is null) return;
        headingBlockParser.Closed -= HeadingBlockParser_Close;
        headingBlockParser.Closed += HeadingBlockParser_Close;
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }

    private void HeadingBlockParser_Close(BlockProcessor processor, Block block)
    {
        if (block is not HeadingBlock headingBlock) return;
        var wrapper = new Wrapper(this, headingBlock);
        headingBlock.ProcessInlinesEnd += wrapper.HeadingBlock_ProcessInlineEnd;
    }

    private class Wrapper
    {
        private readonly string _heading;

        private readonly AutoIdExtension _parent;

        public Wrapper(AutoIdExtension parent, HeadingBlock headingBlock)
        {
            _parent = parent;
            _heading = headingBlock.Lines.Lines[0].ToString();
        }

        public void HeadingBlock_ProcessInlineEnd(InlineProcessor processor, Inline? _)
        {
            var sectionNOs = processor.Document.GetData(AutoIdExtensionKey) as Dictionary<int, int>;
            if (sectionNOs is null)
            {
                sectionNOs = new();
                processor.Document.SetData(AutoIdExtensionKey, sectionNOs);
            }

            var headingBlock = (processor.Block as HeadingBlock)!;
            if (headingBlock.Inline is null)
            {
                return;
            }

            var attributes = headingBlock.GetAttributes();

            #region Generate SectionNumber

            var level = headingBlock.Level;
            sectionNOs.TryAdd(level, 0);
            sectionNOs[headingBlock.Level]++;

            var headingId = "";
            while (level > 0)
            {
                sectionNOs.TryGetValue(level, out var lastSectionNo);
                headingId = string.IsNullOrEmpty(headingId) ? lastSectionNo.ToString() : $"{lastSectionNo}-{headingId}";
                level--;
            }

            #endregion

            if ((_parent._option & AutoIdExtensionOption.UseAutoPrefix) != 0 && _parent.Prefix is not null)
                headingId = $"{_parent.Prefix}-{headingId}";

            if ((_parent._option & AutoIdExtensionOption.UseOriginalId) != 0)
                headingId = $"{headingId}-{attributes.Id}";

            _parent.WhenSetId?.Invoke(attributes.Id = headingId, _heading);
        }
    }

    public delegate void IdSettedIdHandler(string id, string heading);
    public event IdSettedIdHandler? WhenSetId;
}
