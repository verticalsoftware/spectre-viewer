﻿using System.Buffers;
using System.Text;

namespace Vertical.SpectreViewer;

internal sealed class PageContent : IPageContent
{
    private readonly int _width;
    private readonly int _height;
    private readonly IReadOnlyList<BreakPosition> _breakPositions;
    private readonly IReadOnlyCollection<MarkupTag> _markupTags;
    private readonly ArrayBufferWriter<char> _buffer;
    private readonly Dictionary<int, string> _cachedRenderedPages = new(128);
    private readonly Lazy<ILookup<int, string>> _lazyPageTags;

    internal PageContent(
        int width, 
        int height,
        ArrayBufferWriter<char> bufferWriter,
        IReadOnlyList<BreakPosition> breakPositions,
        IReadOnlyCollection<MarkupTag> markupTags)
    {
        _width = width;
        _height = height;
        _breakPositions = breakPositions;
        _markupTags = markupTags;
        _buffer = bufferWriter;
        _lazyPageTags = new Lazy<ILookup<int, string>>(BuildPageTagLookup);
        PageCount = _breakPositions.Count / height + 1;
    }

    /// <summary>
    /// Gets the page content at the given index.
    /// </summary>
    /// <param name="index">Index</param>
    public string this[int index] => GetRenderedPage(index);

    /// <inheritdoc />
    public int PageCount { get; }

    private string GetRenderedPage(int index)
    {
        if (_cachedRenderedPages.TryGetValue(index, out var str))
            return str;

        var (lower, upper) = GetPageCharRange(index);
        var sb = new StringBuilder((int)(_width * _height * 1.5));
        
        foreach (var tag in GetOpenTags(index))
        {
            sb.Append(tag);
        }
        
        var span = _buffer.WrittenSpan[lower..upper];
        sb.Append(span);
        
        foreach (var tag in GetCloseTags(index))
        {
            sb.Append(tag);
        }

        var completeRender = sb.ToString();
        _cachedRenderedPages[index] = completeRender;
        
        return completeRender; 
    }
    
    private ILookup<int, string> BuildPageTagLookup()
    {
        // Since the utility can breakup opening and closing markup tags to display pages
        // (which are subsets of the original markup), we need to track them because SpectreConsole
        // throws when the stack is unbalanced. This means we need to track them.
        // Tracking them =
        // (1) determine which opened tags would not be closed by the end of the page
        // (2) make sure they get closed when the page is rendered
        // (3) make sure they get re-opened when the next page is rendered to continue the styling and ensure that the
        // closing tag can be paired.
        
        var stack = new Stack<MarkupTag>();
        var queue = new Queue<MarkupTag>(_markupTags);
        var writeTags = new List<KeyValuePair<int, string>>(_markupTags.Count * 4);
        var pageCount = _breakPositions.Count / _height + 1;
        
        for (var page = 0; page < pageCount; page++)
        {
            // The stack contains tags closed on the last page - mark them to get re-opened on
            // this page
            writeTags.AddRange(stack.Reverse().Select(tag => new KeyValuePair<int, string>(page, tag.Value)));

            var (lower, upper) = GetPageCharRange(page);

            while (queue.TryPeek(out var top))
            {
                if (top.Position < lower || top.Position >= upper)
                    break;

                queue.Dequeue();
                
                if (top.Type == MarkupType.Opening)
                    stack.Push(top);
                else
                    stack.Pop();
            }
            
            // The stack contains open tags, which must be closed before the page is rendered - mark
            // them to get closed.
            writeTags.AddRange(Enumerable.
                Range(0, stack.Count)
                .Select(_ => new KeyValuePair<int, string>(page, "[/]")));
        }
        
        return writeTags.ToLookup(e => e.Key, e => e.Value);
    }

    private (int Lower, int Upper) GetPageCharRange(int index)
    {
        var startBreakIndex = Math.Min(_breakPositions.Count - 1, index * _height);
        var endBreakIndex = startBreakIndex + _height;
        var startCharIndex = _breakPositions[startBreakIndex].Position;
        var endCharIndex = endBreakIndex < _breakPositions.Count
            ? _breakPositions[endBreakIndex].Position
            : _buffer.WrittenCount;

        return (startCharIndex, endCharIndex);
    }
    
    private IEnumerable<string> GetOpenTags(int index) => _lazyPageTags
        .Value[index]
        .Where(tag => tag != "[/]");
    
    private IEnumerable<string> GetCloseTags(int index) => _lazyPageTags
        .Value[index]
        .Where(tag => tag == "[/]");
}