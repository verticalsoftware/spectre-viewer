namespace Vertical.SpectreViewer;

internal sealed class ComputedRenderingOptions
{
    internal ComputedRenderingOptions(SpectreViewerOptions options)
    {
        RenderWidth = options.RenderWidth;
        RenderHeight = options.RenderHeight;
        LineNumbers = options.LineNumbers;
        PreserveLeadingWhiteSpace = options.PreserveLeadingWhiteSpace;
        InternalWidth = RenderWidth - Constants.RightMarginSpace - (LineNumbers ? PageContent.LineNumberRenderWidth : 0);
        InternalHeight = RenderHeight - Constants.BottomMarginSpace;
    }

    internal int InternalHeight { get; set; }

    internal int InternalWidth { get; set; }

    internal bool PreserveLeadingWhiteSpace { get; set; }

    internal bool LineNumbers { get; set; }

    internal int RenderHeight { get; set; }

    internal int RenderWidth { get; set; }
}