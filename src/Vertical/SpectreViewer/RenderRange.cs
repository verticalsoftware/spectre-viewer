namespace Vertical.SpectreViewer;

internal readonly record struct RenderRange(int LowerBound, int UpperBound)
{
    internal static RenderRange Empty = new(-1, -1);
}