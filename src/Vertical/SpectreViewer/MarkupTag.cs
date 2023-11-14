namespace Vertical.SpectreViewer;

internal record MarkupTag(MarkupType Type, int LineOffset, string Value)
{
    private static int _id;
    
    internal const string CloseTag = "[/]";

    internal int Id { get; } = _id++;
}