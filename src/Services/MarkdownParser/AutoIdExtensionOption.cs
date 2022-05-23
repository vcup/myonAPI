namespace myonAPI.Services.MarkdownParser;

[Flags]
internal enum AutoIdExtensionOption
{
    None = 0b00,
    UseAutoPrefix = 0b01,
    UseOriginalId = 0b10,
    EnableAll = 0b11,
}