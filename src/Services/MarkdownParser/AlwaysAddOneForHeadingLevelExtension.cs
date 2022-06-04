using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax;

namespace myonAPI.Services.MarkdownParser;

public class AlwaysAddOneForHeadingLevelExtension : IMarkdownExtension
{
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
        headingBlock.Level++;
    }
}