namespace Vertical.SpectreViewer;

internal static class RenderEngine
{
    internal static IPageContent Write(
        TextReader textReader, 
        IRenderBuffer renderBuffer)
    {
        using (textReader)
        {
            ReadStream(textReader, renderBuffer);
        }

        return renderBuffer.GetPageContent();
    }

    private static void ReadStream(TextReader textReader, IRenderBuffer renderBuffer)
    {
        while (true)
        {
            var inputLine = textReader.ReadLine()?.TrimEnd();

            if (inputLine == null)
                break;
            
            ReadLine(inputLine, renderBuffer);
        }
    }

    private static void ReadLine(string inputLine, IRenderBuffer writer)
    {
        // Fairly efficient check
        if (string.IsNullOrWhiteSpace(inputLine))
        {
            writer.WriteLine();
            return;
        }

        var tagPosition = writer.Position;
        var span = inputLine.AsSpan();
        var width = writer.Width;
        
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
        var lastChar = '\0';

        for (; ptr < len; ptr++)
        {
            if (subIndent > 0)
            {
                writer.WriteWhitespace(subIndent);
                //subIndent = 0;
                tagPosition = writer.Position;
                virtualCursor += subIndent;
            }

            if (span[ptr] == '[' && lastChar != '[')
            {
                var tagSpan = span[ptr..];
                var tagLength = GetMarkTagLength(tagSpan);
                if (tagLength > 0)
                {
                    writer.AddMarkupTag(new string(tagSpan[..tagLength]), tagPosition);
                    
                    // Skip reading this span any further
                    ptr += tagLength;
                    tagPosition += tagLength;
                    
                    // Tag could be at the end of the span
                    if (ptr == len)
                        break;
                    
                    // This is new...
                    subIndent = 0;
                    lastChar = ']';
                    ptr--;
                    continue;
                }
            }
            
            if (virtualCursor == width)
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
                writer.Write(span[..w]);
                writer.WriteLine();

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

                lastChar = '\0';
                continue;
            }

            subIndent = 0;
            virtualCursor++;
            tagPosition++;
            lastChar = span[ptr];
        }
        
        // Write what's left in span 
        if (span.Length == 0)
            return;

        if (virtualCursor > 0)
        {
            writer.WriteWhitespace(subIndent);
        }

        writer.Write(span);

        if (virtualCursor > 0)
        {
            writer.WriteLine();
        }
    }

    private static int GetMarkTagLength(ReadOnlySpan<char> span)
    {
        // Set pointer after first bracket
        var len = span.Length;
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