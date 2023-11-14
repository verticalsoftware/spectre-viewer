namespace Vertical.SpectreViewer;

internal readonly record struct MarkupTagPair(MarkupTag OpenTag, MarkupTag CloseTag) : IComparable<MarkupTagPair>
{
    /// <inheritdoc />
    public int CompareTo(MarkupTagPair other)
    {
        return Comparer<int>.Default.Compare(OpenTag.Id, other.OpenTag.Id);
    }
}