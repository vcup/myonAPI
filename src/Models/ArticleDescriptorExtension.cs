using myonAPI.Services;

namespace myonAPI.Models;

internal static class ArticleDescriptorExtension
{
    public static void Build(this ArticleDescriptor descriptor, MarkdownParserService service)
    {
        using var reader = File.OpenText(descriptor.ContentFilePath);
        descriptor.Content = service.GetHtml(descriptor.Id, reader);
        descriptor.HtmlHeadingIdRelation = service.MarkdownHeadings;
    }
}