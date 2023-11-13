namespace Vertical.SpectreViewer;

internal readonly record struct RenderInfo(
    int LowerBound, 
    int UpperBound,
    bool OffsetAtTop,
    bool OffsetAtOrPastEnd,
    int PageId,
    int PageCount,
    int RowCount)
{
    internal static RenderInfo Empty = new(-1, -1, false, false, -1, 0, 0);
}