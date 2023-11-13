namespace Vertical.SpectreViewer;

internal record MarkupTag(MarkupType Type, int LineOffset, string Value)
{
    internal const string CloseTag = "[/]";
}