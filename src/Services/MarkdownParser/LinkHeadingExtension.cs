using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace myonAPI.Services.MarkdownParser;

public class LinkHeading : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        var headingBlockParser = pipeline.BlockParsers.Find<HeadingBlockParser>();
        if (headingBlockParser is not null)
        {
            headingBlockParser.Closed -= HeadingBlockParser_Closed;
            headingBlockParser.Closed += HeadingBlockParser_Closed;
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }

    private void HeadingBlockParser_Closed(BlockProcessor processor, Block block)
    {
        if (block is not HeadingBlock headingBlock)
        {
            return;
        }

        headingBlock.ProcessInlinesEnd += HeadingBlock_ProcessInlinesEnd;
    }

    private void HeadingBlock_ProcessInlinesEnd(InlineProcessor processor, Inline? inline)
    {
        var headingBlock = (HeadingBlock)processor.Block!;
        if (headingBlock.Inline is null)
        {
            return;
        }

        var attributes = headingBlock.GetAttributes();
        if (attributes.Id is null)
        {
            return;
        }

        var linkInline = new LinkInline
        {
            GetDynamicUrl = () => HtmlHelper.Unescape('#' + attributes.Id)
        };
        linkInline.AppendChild(headingBlock.Inline);
        headingBlock.Inline = linkInline;
    }
}