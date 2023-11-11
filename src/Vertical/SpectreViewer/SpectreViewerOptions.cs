using Spectre.Console;

namespace Vertical.SpectreViewer;

/// <summary>
/// Represents options used by the viewer to render content.
/// </summary>
public sealed class SpectreViewerOptions
{
    private const int RightMargin = 5;
    internal const int BottomMargin = 4;
    
    /// <summary>
    /// Gets the render width (defaults to the console width).
    /// </summary>
    public int RenderWidth { get; init; } = AnsiConsole.Profile.Width - RightMargin;

    /// <summary>
    /// Gets the render height (defaults to the console height).
    /// </summary>
    public int RenderHeight { get; init; } = AnsiConsole.Profile.Height - BottomMargin;

    /// <summary>
    /// Gets the console to render the output to.
    /// </summary>
    public IAnsiConsole Console { get; init; } = AnsiConsole.Console;
}