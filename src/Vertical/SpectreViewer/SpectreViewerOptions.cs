using Spectre.Console;

namespace Vertical.SpectreViewer;

/// <summary>
/// Represents options used by the viewer to render content.
/// </summary>
public sealed class SpectreViewerOptions
{
    private const int RightMargin = 4;
    internal const int BottomMargin = 4;
    
    /// <summary>
    /// Gets the render width (defaults to the console width minus some padding).
    /// </summary>
    public int RenderWidth { get; init; } = AnsiConsole.Profile.Width - RightMargin;

    /// <summary>
    /// Gets the render height (defaults to the console height minus some padding).
    /// </summary>
    public int RenderHeight { get; init; } = AnsiConsole.Profile.Height - BottomMargin;

    /// <summary>
    /// Gets the number of rows to overlap when splitting rows between pages.
    /// </summary>
    public int OverlapHeight { get; init; } = 4;
    
    /// <summary>
    /// Gets whether to display line numbers for troubleshooting purposes.
    /// </summary>
    public bool LineNumbers { get; init; }

    /// <summary>
    /// Gets whether to preserve leading whitespace, meaning lines that are indented remain indented
    /// when they are split due to the length exceeding the console width.
    /// </summary>
    public bool PreserveLeadingWhiteSpace { get; init; } = true;
}