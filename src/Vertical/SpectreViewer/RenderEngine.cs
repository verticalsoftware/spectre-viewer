namespace Vertical.SpectreViewer;

internal static class RenderEngine
{
    internal static IPageContent Write(
        TextReader textReader, 
        IRenderBuffer renderBuffer,
        SpectreViewerOptions options)
    {
        ReadStream(textReader, renderBuffer, options);
        return renderBuffer.GetPageContent(options);
    }

    private static void ReadStream(TextReader textReader, IRenderBuffer buffer, SpectreViewerOptions options)
    {
        while (true)
        {
            var inputLine = textReader.ReadLine()?.TrimEnd();

            if (inputLine == null)
                break;
            
            ReadLine(inputLine, buffer, options);
        }
    }

    private static void ReadLine(string inputLine, IRenderBuffer buffer, SpectreViewerOptions options)
    {
        // Fairly efficient check
        if (string.IsNullOrWhiteSpace(inputLine))
        {
            buffer.WriteLine();
            return;
        }

        var preserveWs = options.PreserveLeadingWhiteSpace;
        var tagPosition = buffer.Position;
        var span = inputLine.AsSpan();
        var width = buffer.Width;
        
        // Read leading whitespace to determine indent
        var indent = 0;
        for (; indent < span.Length && char.IsWhiteSpace(span[indent]); indent++)
        {
        }
        
        // This marks where the true indent is when rendering a line, assigned later
        var subIndent = 0;
        
        // This is where the "virtual" cursor position in
        var virtualCursor = 0;
        
        // Read pointers
        var ptr = 0;
        var len = span.Length;
        
        // Markers & counters
        var bracketStack = 0;

        for (; ptr < len; ptr++)
        {
            if (subIndent > 0)
            {
                buffer.WriteWhitespace(subIndent);
                //subIndent = 0;
                tagPosition = buffer.Position;
                virtualCursor += subIndent;
            }

            if (span[ptr] == '[' && bracketStack % 2 == 0)
            {
                var tagSpan = span[ptr..];
                var tagLength = GetMarkTagLength(tagSpan);
                if (tagLength > 0)
                {
                    buffer.AddMarkupTag(new string(tagSpan[..tagLength]), tagPosition);
                    
                    // Skip reading this span any further
                    ptr += tagLength;
                    tagPosition += tagLength;
                    
                    // Tag could be at the end of the span
                    if (ptr == len)
                        break;
                    
                    // This is new...
                    subIndent = 0;
                    bracketStack = 0;
                    ptr--;
                    continue;
                }
            }
            
            if (preserveWs && virtualCursor == width)
            {
                // At the right margin, break the line
                var w = ptr;
                
                // Back-track sub-indent spacing
                for (var i = 0; i < subIndent; i++)
                {
                    w--;
                }
                
                // Back-track to the previous word break
                for (; w >= 0 && !char.IsWhiteSpace(span[w]); w--)
                {
                }

                if (w == 0)
                {
                    // No word break found, break at the current position
                    w = ptr;
                }
                
                // Write up to w
                buffer.Write(span[..w]);
                buffer.WriteLine();

                ptr = w;
                
                // Advance pointer past whitespace
                for (; ptr < len && char.IsWhiteSpace(span[ptr]); ptr++)
                {
                }
                
                // Slice span
                span = span[ptr..];
                tagPosition += ptr;
                ptr = -1;
                len = span.Length;
                virtualCursor = 0;
                
                // Apply indent to next line
                subIndent = indent;

                bracketStack = 0;
                continue;
            }

            subIndent = 0;
            virtualCursor++;
            tagPosition++;
            
            // Adjust bracket counter
            if (span[ptr] == '[')
                bracketStack++;
            else
                bracketStack = 0;
        }
        
        // Write what's left in span 
        if (span.Length == 0)
            return;

        if (preserveWs && virtualCursor > 0)
        {
            // Preserve whitespace in case no line break was made
            buffer.WriteWhitespace(subIndent);
        }

        buffer.Write(span);

        if (virtualCursor > 0)
        {
            buffer.WriteLine();
        }
    }

    private static int GetMarkTagLength(ReadOnlySpan<char> span)
    {
        var len = span.Length;
        
        // Set pointer after first bracket and forward scan one character
        var s = 1;
        if (s == len || span[s] == '[')
            return 0;

        for (; s < len && span[s] != ']'; s++)
        {
        }

        // Advance past the ending bracket
        s++;
        return s;
    }
}