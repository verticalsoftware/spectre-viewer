using System.Buffers;

namespace Vertical.SpectreViewer;

internal sealed class RenderBuffer : IRenderBuffer
{
    private readonly char[] _environmentNewLineChars = Environment.NewLine.ToCharArray();
    private readonly ArrayBufferWriter<char> _buffer;
    private readonly List<BreakPosition> _breakPositions = new(5000);
    private readonly List<MarkupTag> _markupTags = new(512);
    private int _bufferMark;

    internal RenderBuffer(int width, int height)
    {
        _buffer = new ArrayBufferWriter<char>((int)(height * width * 1.25));
    }

    public int LineCount => _breakPositions.Count;

    public IPageContent GetPageContent(ComputedRenderingOptions options)
    {
        return new PageContent(options, _buffer, _breakPositions, _markupTags);
    }

    public void WriteWhitespace(int count = 1)
    {
        if (count == 0)
            return;

        var span = _buffer.GetSpan(count);
        for (var c = 0; c < count; c++)
        {
            span[c] = ' ';
        }
        _buffer.Advance(count);
    }

    public void Write(ReadOnlySpan<char> span)
    {
        _buffer.Write(span);
    }

    public void WriteLine()
    {
        var position = _buffer.WrittenCount;
        var length = position - _bufferMark;
        _breakPositions.Add(new BreakPosition(_bufferMark, length));
        _bufferMark = position;
        Write(_environmentNewLineChars);
    }

    public int Position => _buffer.WrittenCount;

    /// <inheritdoc />
    public override string ToString() => new(_buffer.WrittenSpan);

    public void AddMarkupTag(string tag, int position)
    {
        _markupTags.Add(new MarkupTag(
            tag == "[/]" ? MarkupType.Closing : MarkupType.Opening,
            position, 
            new string(tag),
            _breakPositions.Count));
    }
}