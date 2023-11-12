using Spectre.Console;

namespace Vertical.SpectreViewer;

/// <summary>
/// Represents options used by the viewer to render content.
/// </summary>
public sealed class SpectreViewerOptions
{
    /// <summary>
    /// Gets the render width (defaults to the console width minus some padding).
    /// </summary>
    public int RenderWidth { get; init; } = AnsiConsole.Profile.Width;

    /// <summary>
    /// Gets the render height (defaults to the console height minus some padding).
    /// </summary>
    public int RenderHeight { get; init; } = AnsiConsole.Profile.Height;

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