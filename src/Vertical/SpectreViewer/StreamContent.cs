using System.Text;

namespace Vertical.SpectreViewer;

internal class StreamContent
{
    private readonly IReadOnlyList<string> _lines;
    private readonly ComputedRenderingOptions _options;
    private readonly StringBuilder _stringBuilder = new(32000);
    private readonly Lazy<string> _lazyLineFormat;
    private readonly MarkupTagManager _markupTagManager;

    internal StreamContent(
        IReadOnlyList<string> lines,
        IEnumerable<MarkupTag> markupTags,
        ComputedRenderingOptions options)
    {
        _lines = lines;
        _options = options;
        _lazyLineFormat = new Lazy<string>(BuildLineFormat(lines.Count));
        _markupTagManager = new MarkupTagManager(markupTags, lines.Count);
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

    private string BuildAnnotatedPageContent(RenderInfo renderInfo) =>
        BuildPageContent(renderInfo, _lazyLineFormat.Value);

    private string BuildPageContent(RenderInfo renderInfo) => BuildPageContent(renderInfo, null);

    private string BuildPageContent(RenderInfo renderInfo, string? lineFormat)
    {
        _stringBuilder.Clear();

        _markupTagManager.WriteOpenTags(_stringBuilder, renderInfo);
        
        for (var c = renderInfo.LowerBound; c < renderInfo.UpperBound; c++)
        {
            if (lineFormat != null)
            {
                _stringBuilder.Append(string.Format(lineFormat, c));
            }

            _stringBuilder.AppendLine(_lines[c]);
        }
        _markupTagManager.WriteCloseTags(_stringBuilder, renderInfo);

        return _stringBuilder.ToString();
    }

    private static string BuildLineFormat(int linesCount)
    {
        var digitCount = (int)Math.Floor(Math.Log10(linesCount) + 1);
        var digitFormat = new string('0', digitCount);
        return $"[grey46]{{0:{digitFormat}}}[/] ";
    }
}