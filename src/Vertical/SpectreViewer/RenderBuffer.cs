using System.Text;

namespace Vertical.SpectreViewer;

internal sealed class RenderBuffer
{
    private readonly ComputedRenderingOptions _options;
    private readonly StringBuilder _buffer;
    private readonly List<string> _lines = new(20000);
    private readonly List<MarkupTag> _markupTags = new(1000);
    
    private int _contentCharsWritten = 0;
    private int _queuedNewLines = 0;

    internal RenderBuffer(ComputedRenderingOptions options)
    {
        _options = options;
        _buffer = new StringBuilder(options.RenderWidth * 2);
    }

    internal StreamContent GetStreamContent()
    {
        return new StreamContent(_lines, _markupTags, _options);
    }
    
    internal void WriteMarkupTag(string value)
    {
        var tag = new MarkupTag(
            value == MarkupTag.CloseTag ? MarkupType.Closing : MarkupType.Opening,
            _lines.Count,
            value);
        _markupTags.Add(tag);
        _buffer.Append(value);
    }

    internal void Write(ReadOnlySpan<char> span)
    {
        CheckEnqueuedLines();
        if (span.Length == 0) return;
        _buffer.Append(span);
        _contentCharsWritten += span.Length;
    }

    internal void WriteWhiteSpace(int count = 1)
    {
        CheckEnqueuedLines();
        _buffer.Append(' ', count);
        _contentCharsWritten += count;
    }

    internal void NewLine(bool wrapping)
    {
        _lines.Add(_buffer.ToString());
        if (!wrapping) _lines.Add(string.Empty);
        _buffer.Clear();
        _contentCharsWritten = 0;
        _queuedNewLines = 0;
    }

    internal void EnqueueNewLine()
    {
        if (_contentCharsWritten == 0)
            return;
        _queuedNewLines++;
    }

    internal void Close()
    {
        if (_buffer.Length == 0)
            return;
        
        _lines.Add(_buffer.ToString());
        _contentCharsWritten = 0;
        _queuedNewLines = 0;
    }

    internal string Content => string.Join(Environment.NewLine, _lines);

    private void CheckEnqueuedLines()
    {
        if (_queuedNewLines == 0)
            return;

        if (_contentCharsWritten == 0)
            return;
        
        _lines.Add(_buffer.ToString());
        _buffer.Clear();
        _contentCharsWritten = 0;
        _queuedNewLines = 0;
    }
}