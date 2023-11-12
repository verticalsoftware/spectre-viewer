namespace Vertical.SpectreViewer;

internal interface IRenderBuffer
{
    int LineCount { get; }
    int Position { get; }
    IPageContent GetPageContent(ComputedRenderingOptions options);
    void WriteWhitespace(int count = 1);
    void Write(ReadOnlySpan<char> span);
    void WriteLine();
    void AddMarkupTag(string tag, int position);
}