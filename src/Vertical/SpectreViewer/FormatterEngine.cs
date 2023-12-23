namespace Vertical.SpectreViewer;

internal sealed class FormatterEngine
{
    private readonly ComputedRenderingOptions _options;
    private readonly RenderBuffer _buffer;
    private readonly IReadOnlyDictionary<string, string> _styles;

    internal FormatterEngine(ComputedRenderingOptions options, RenderBuffer buffer)
    {
        _options = options;
        _buffer = buffer;
        _styles = BuildStyles(options);
    }

    internal void ReadStream(TextReader textReader)
    {
        while (true)
        {
            var inputLine = textReader.ReadLine();
            if (inputLine == null)
                break;

            ReadInputLine(inputLine);
            _buffer.EnqueueNewLine();
        }

        _buffer.Close();
    }

    private void ReadInputLine(string inputLine)
    {
        // Efficiency check
        if (string.IsNullOrWhiteSpace(inputLine))
        {
            _buffer.NewLine(wrapping: false);
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

            virtualCursorPos = ptr;
        }

        // Mark indentation point
        var indent = ptr;
        var subIndent = 0;
        var bracketStack = 0;

        // Note: Pointer is positioned after whitespace
        for (; ptr < span.Length; ptr++)
        {
            while (subIndent > 0)
            {
                _buffer.WriteWhiteSpace(subIndent);
                virtualCursorPos = subIndent;
                subIndent = 0;
            }

            if (span[ptr] == '[' && bracketStack % 2 == 0)
            {
                var subSpan = span[ptr..];
                
                if (TryReadMarkupTag(subSpan, out var length))
                {
                    var contentSpan = span[..ptr];
                    _buffer.Write(contentSpan);
                    
                    var tagValue = span[ptr..(ptr + length)].ToString();
                    _buffer.WriteMarkupTag(ResolveReplacementStyle(tagValue));

                    // Advance past the tag
                    ptr += length;

                    if (ptr == span.Length)
                    {
                        span = ReadOnlySpan<char>.Empty;
                        break;
                    }

                    bracketStack = 0;
                    span = span[ptr..];
                    ptr = -1;
                    continue;
                }
            }

            if (virtualCursorPos > width)
            {
                // Break line
                var b = ptr;

                // Backtrack to word break
                for (; b >= 0 && !char.IsWhiteSpace(span[b]); b--)
                {
                }
                
                if (b == -1)
                {
                    // No word break found, we break at 0
                    b = 0;
                }
                
                _buffer.Write(span[..b]);
                _buffer.NewLine(wrapping: true);
                
                // Left trim span starting at b
                for (; b < span.Length && char.IsWhiteSpace(span[b]); b++)
                {
                }
                
                span = span[b..];
                virtualCursorPos = 0;
                subIndent = indent;
                ptr = -1;
                bracketStack = 0;
                continue;
            }

            bracketStack += span[ptr] switch
            {
                '[' => 1,
                ']' => -1,
                _ => -bracketStack
            };
            
            virtualCursorPos++;
        }
        
        // Whatever is left in span goes to buffer
        _buffer.Write(span);
    }

    private static bool TryReadMarkupTag(ReadOnlySpan<char> span, out int length)
    {
        var ptr = 1;
        length = 0;

        if (ptr == span.Length || span[ptr] == '[')
            return false;

        for (; ptr < span.Length && span[ptr] != ']'; ptr++)
        {
        }

        if (ptr == span.Length)
        {
            throw new InvalidOperationException(
                "Unbalanced markup");
        }

        length = ptr + 1;
        return true;
    }

    private string ResolveReplacementStyle(string tag)
    {
        return _styles.TryGetValue(tag, out var style)
            ? style
            : tag;
    }
    
    private static IReadOnlyDictionary<string, string> BuildStyles(ComputedRenderingOptions options)
    {
        return options
            .CallerOptions
            .Styles
            .ToDictionary(kv => $"[{kv.Key}]", kv => $"[{kv.Value}]");
    }
}