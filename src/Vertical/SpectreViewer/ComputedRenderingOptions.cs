namespace Vertical.SpectreViewer;

internal sealed class ComputedRenderingOptions
{
    internal ComputedRenderingOptions(SpectreViewerOptions options)
    {
        RenderWidth = options.RenderWidth;
        RenderHeight = options.RenderHeight;
        LineNumbers = options.LineNumbers;
        PreserveLeadingWhiteSpace = options.PreserveLeadingWhiteSpace;
        InternalWidth = RenderWidth - Constants.RightMarginSpace - (LineNumbers ? Constants.LineNumberSpaces : 0);
        InternalHeight = RenderHeight - Constants.BottomMarginSpace;
        PageOverlapRows = options.PageOverlapRows;
        CallerOptions = options;
    }

    internal int InternalHeight { get; set; }

    internal int InternalWidth { get; set; }

    internal bool PreserveLeadingWhiteSpace { get; set; }

    internal bool LineNumbers { get; set; }

    internal int RenderHeight { get; set; }

    internal int RenderWidth { get; set; }
    
    internal int PageOverlapRows { get; init; } = 3;
    
    internal SpectreViewerOptions CallerOptions { get; init; }
}