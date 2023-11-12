namespace Vertical.SpectreViewer;

internal record PageContent(
    int Offset,
    (int LowerBound, int Count) Bounds,
    string Value);