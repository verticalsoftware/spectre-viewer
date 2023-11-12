namespace Vertical.SpectreViewer;

internal record MarkupTag(MarkupType Type, int LineOffset, int Position, int Length, string Value)
{
    internal const string CloseTag = "[/]";
}