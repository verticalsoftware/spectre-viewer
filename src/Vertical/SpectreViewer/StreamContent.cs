using System.Text;

namespace Vertical.SpectreViewer;

internal class StreamContent
{
    private readonly IReadOnlyList<string> _lines;
    private readonly ComputedRenderingOptions _options;
    private readonly StringBuilder _buffer = new(32000);
    private readonly Lazy<string> _lazyLineFormat;

    internal StreamContent(IReadOnlyList<string> lines, ComputedRenderingOptions options)
    {
        _lines = lines;
        _options = options;
        _lazyLineFormat = new Lazy<string>(BuildLineFormat(lines.Count));
    }

    internal int LowerOffset => _lines.Count > 0 ? 0 : -1;

    internal int UpperOffset => _lines.Count > 0 ? _lines.Count : -1;

    internal int PageRowCount => _options.InternalHeight - _options.PageOverlapRows;

    internal int PageCount => _lines.Count > 0 ? _lines.Count / PageRowCount + 1 : 0;

    internal RenderInfo GetRenderInfo(int offset)
    {
        var upperBound = Math.Min(_lines.Count, Math.Max(0, offset) + PageRowCount);
        var lowerBound = Math.Max(0, upperBound - PageRowCount);
        var pageId = lowerBound / PageRowCount + 1;
        
        return new RenderInfo(
            lowerBound, 
            upperBound,
            lowerBound == 0,
            lowerBound == PageCount * PageRowCount,
            pageId,
            PageCount,
            _lines.Count);
    }

    internal string GetPageContent(RenderInfo info)
    {
        return _options.LineNumbers
            ? BuildAnnotatedPageContent(info)
            : BuildPageContent(info);
    }

    private string BuildAnnotatedPageContent(RenderInfo info)
    {
        var lineFormat = _lazyLineFormat.Value;
        _buffer.Clear();
        for (var c = info.LowerBound; c < info.UpperBound; c++)
        {
            _buffer.Append(string.Format(lineFormat, c));
            _buffer.AppendLine(_lines[c]);
        }

        return _buffer.ToString();
    }

    private string BuildPageContent(RenderInfo info)
    {
        _buffer.Clear();
        for (var c = info.LowerBound; c < info.UpperBound; c++)
        {
            _buffer.AppendLine(_lines[c]);
        }

        return _buffer.ToString();
    }

    private static string BuildLineFormat(int linesCount)
    {
        var digitCount = (int)Math.Floor(Math.Log10(linesCount) + 1);
        var digitFormat = new string('0', digitCount);
        return $"[grey46]{{0:{digitFormat}}}[/] ";
    }
}