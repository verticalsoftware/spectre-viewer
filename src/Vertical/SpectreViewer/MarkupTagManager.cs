using System.Text;

namespace Vertical.SpectreViewer;

internal sealed class MarkupTagManager
{
    private readonly record struct RelativeDistance(short Open, short Close);
    private readonly RelativeDistance[,] _matrix;
    private readonly string[] _tags;

    internal MarkupTagManager(IEnumerable<MarkupTag> markupTags, int rowCount)
    {
        var stack = new Stack<MarkupTag>(rowCount / 2);
        var pairs = new List<MarkupTagPair>(rowCount / 2);

        foreach (var tag in markupTags)
        {
            if (tag.Type == MarkupType.Opening)
            {
                stack.Push(tag);

                continue;
            }
            
            pairs.Add(new MarkupTagPair(stack.Pop(), tag));
        }

        pairs.Sort();
        
        _matrix = new RelativeDistance[pairs.Count, rowCount];

        for (var x = 0; x < pairs.Count; x++)
        {
            var tag = pairs[x];
            for (var y = 0; y < rowCount; y++)
            {
                _matrix[x, y] = new RelativeDistance(
                    (short)(tag.OpenTag.LineOffset - y),
                    (short)(tag.CloseTag.LineOffset - y));
            }
        }

        _tags = pairs.Select(pair => pair.OpenTag.Value).ToArray();
    }

    internal void WriteOpenTags(StringBuilder sb, RenderInfo renderInfo)
    {
        var bound = renderInfo.LowerBound;
        for (var x = 0; x < _tags.Length; x++)
        {
            var distance = _matrix[x, bound];
            if (distance is { Open: < 0, Close: >= 0 })
            {
                sb.Append(_tags[x]);
            }
        }
    }

    internal void WriteCloseTags(StringBuilder sb, RenderInfo renderInfo)
    {
        var bound = renderInfo.UpperBound - 1;
        for (var x = 0; x < _tags.Length; x++)
        {
            var distance = _matrix[x, bound];
            if (distance is { Open: <= 0, Close: > 0 })
            {
                sb.Append(MarkupTag.CloseTag);
            }
        }
    }
}