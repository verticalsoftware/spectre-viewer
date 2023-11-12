namespace Vertical.SpectreViewer;

internal sealed class FormattingEngine
{
    private readonly ComputedRenderingOptions _options;
    private readonly RenderBuffer _buffer;

    internal FormattingEngine(ComputedRenderingOptions options, RenderBuffer buffer)
    {
        _options = options;
        _buffer = buffer;
    }

    internal void ReadStream(TextReader textReader)
    {
        while (true)
        {
            var inputLine = textReader.ReadLine();
            if (inputLine == null)
                break;

            ReadInputLine(inputLine);
        }

        _buffer.Close();
    }

    private void ReadInputLine(string inputLine)
    {
        _buffer.BeginLine();
        
        // Efficiency check
        if (string.IsNullOrWhiteSpace(inputLine))
        {
            return;
        }

        var span = inputLine.TrimEnd().AsSpan();
        var ptr = 0;
        var virtualCursorPos = ptr;
        var width = _options.InternalWidth;
        
        // Process indentation
        if (_options.PreserveLeadingWhiteSpace)
        {
            for (; ptr < span.Length && char.IsWhiteSpace(span[ptr]); ptr++)
            {
            }

            if (ptr >= width)
            {
                // We can't format this line because indent exceed horizontal space,
                // we will print as is
                _buffer.Write(span);
                return;
            }
        }

        // Mark indentation point
        var indent = ptr;
        var subIndent = 0;

        // Note: Pointer is positioned after whitespace
        for (; ptr < span.Length; ptr++)
        {
            while (subIndent > 0)
            {
                _buffer.WriteWhiteSpace(subIndent);
                virtualCursorPos = subIndent;
                subIndent = 0;
            }

            if (virtualCursorPos > width)
            {
                // Break line
                var b = ptr;

                // Backtrack to word break
                for (; b >= 0 && !char.IsWhiteSpace(span[b]); b--)
                {
                }
                
                if (b == 0)
                {
                    // No word break found, we break at ptr
                    b = ptr;
                }
                
                _buffer.Write(span[..b]);
                _buffer.BeginLine();
                
                // Left trim span starting at b
                for (; b < span.Length && char.IsWhiteSpace(span[b]); b++)
                {
                }
                
                span = span[b..];
                virtualCursorPos = 0;
                subIndent = indent;
                ptr = 0;
                continue;
            }

            virtualCursorPos++;
        }
        
        // Whatever is left in span goes to buffer
        _buffer.Write(span);
    }
}