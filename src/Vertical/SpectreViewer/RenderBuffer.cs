using System.Text;

namespace Vertical.SpectreViewer;

internal sealed class RenderBuffer
{
    private readonly ComputedRenderingOptions _options;
    private readonly StringBuilder _buffer;
    private readonly List<string> _lines = new(20000);
    private readonly List<MarkupTag> _markupTags = new(1000);
    private bool _beginLineCalled;

    internal RenderBuffer(ComputedRenderingOptions options)
    {
        _options = options;
        _buffer = new StringBuilder(options.RenderWidth * 2);
    }

    internal StreamContent GetStreamContent()
    {
        return new StreamContent(_lines, _options);
    }

    internal void AddOpeningMarkupTag(int position, string value)
    {
        AddMarkupTag(MarkupType.Opening, position, value);
    }

    internal void AddClosingMarkupTag(int position)
    {
        AddMarkupTag(MarkupType.Closing, position, MarkupTag.CloseTag);
    }

    private void AddMarkupTag(MarkupType type, int position, string value)
    {
        _markupTags.Add(new MarkupTag(
            type,
            _lines.Count,
            value));
    }

    internal void Write(ReadOnlySpan<char> span)
    {
        if (span.Length == 0) return;
        _buffer.Append(span);
    }

    internal void WriteWhiteSpace(int count)
    {
        _buffer.Append(' ', count);
    }

    internal void BeginLine()
    {
        FlushBuffer();
        _beginLineCalled = true;
    }

    internal void Close() => FlushBuffer();

    internal string Content => string.Join(Environment.NewLine, _lines);

    private void FlushBuffer()
    {
        if (!_beginLineCalled)
            return;
        
        _lines.Add(_buffer.ToString());
        _buffer.Clear();
    }
}