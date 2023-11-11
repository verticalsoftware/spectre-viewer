namespace Vertical.SpectreViewer;

internal interface IRenderBuffer
{
    int Width { get; }
    int Height { get; }
    int LineCount { get; }
    int Position { get; }
    IPageContent GetPageContent();
    void WriteWhitespace(int count = 1);
    void Write(ReadOnlySpan<char> span);
    void WriteLine();
    void AddMarkupTag(string tag, int position);
}