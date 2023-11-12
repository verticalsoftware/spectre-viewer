using System.Text;
using Spectre.Console;

namespace Vertical.SpectreViewer;

public static class SpectreConsoleViewer
{
    /// <summary>
    /// Renders the content read from the given <see cref="StringBuilder"/> to the console defined in the static
    /// <see cref="AnsiConsole"/> type.
    /// </summary>
    /// <param name="str">String that contains the content.</param>
    /// <param name="options">Options.</param>
    public static void MarkupWithPaging(string str, SpectreViewerOptions? options = null)
    {
        AnsiConsole.Console.MarkupWithPaging(str, options);         
    }
    
    /// <summary>
    /// Renders the content read from the given <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="console">The console to render to.</param>
    /// <param name="str">String that contains the content.</param>
    /// <param name="options">Options.</param>
    public static void MarkupWithPaging(this IAnsiConsole console, string str, SpectreViewerOptions? options = null)
    {
        using var reader = new StringReader(str);
        MarkupWithPaging(console, reader, options);
    }
    
    /// <summary>
    /// Renders the content read from the given <see cref="StringBuilder"/> to the console defined in the static
    /// <see cref="AnsiConsole"/> type.
    /// </summary>
    /// <param name="stringBuilder">StringBuilder that contains the content.</param>
    /// <param name="options">Options.</param>
    public static void MarkupWithPaging(
        StringBuilder stringBuilder,
        SpectreViewerOptions? options = null)
    {
        AnsiConsole.Console.MarkupWithPaging(stringBuilder, options);
    }

    /// <summary>
    /// Renders the content read from the given <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="console">The console to render to.</param>
    /// <param name="stringBuilder">StringBuilder that contains the content.</param>
    /// <param name="options">Options.</param>
    public static void MarkupWithPaging(
        this IAnsiConsole console,
        StringBuilder stringBuilder,
        SpectreViewerOptions? options = null)
    {
        using var reader = new StringReader(stringBuilder.ToString());
        MarkupWithPaging(console, reader, options);
    }
    
    /// <summary>
    /// Renders the content read form the provided <see cref="Stream"/> to the console defined in the static
    /// <see cref="AnsiConsole"/> type.
    /// </summary>
    /// <param name="stream">Stream that contains the content.</param>
    /// <param name="options">Options.</param>
    /// <remarks>
    /// This method disposes of <paramref name="stream"/> after rendering. If the stream needs to remain
    /// open, provide a <see cref="TextReader"/> instead.
    /// </remarks>
    public static void MarkupWithPaging(Stream stream, SpectreViewerOptions? options = null)
    {
        AnsiConsole.Console.MarkupWithPaging(stream, options);
    }

    /// <summary>
    /// Renders the content read form the provided <see cref="Stream"/>.
    /// </summary>
    /// <param name="console">The console to render to.</param>
    /// <param name="stream">Stream that contains the content.</param>
    /// <param name="options">Options.</param>
    /// <remarks>
    /// This method disposes of <paramref name="stream"/> after rendering. If the stream needs to remain
    /// open, provide a <see cref="TextReader"/> instead.
    /// </remarks>
    public static void MarkupWithPaging(this IAnsiConsole console, Stream stream, SpectreViewerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        
        using var textReader = new StreamReader(stream);
        MarkupWithPaging(console, textReader, options);
    }
    
    /// <summary>
    /// Renders the content read from the provided <see cref="TextReader"/> to the console defined in the static
    /// <see cref="AnsiConsole"/> type.
    /// </summary>
    /// <param name="textReader"><see cref="TextReader"/> that contains the content.</param>
    /// <param name="options">Options.</param>
    /// <remarks>
    /// This method does not dispose <paramref name="textReader"/>.
    /// </remarks>
    public static void MarkupWithPaging(
        TextReader textReader,
        SpectreViewerOptions? options = null)
    {
        AnsiConsole.Console.MarkupWithPaging(textReader, options);
    }

    /// <summary>
    /// Renders the content read from the provided <see cref="TextReader"/>.
    /// </summary>
    /// <param name="console">The console to render to.</param>
    /// <param name="textReader"><see cref="TextReader"/> that contains the content.</param>
    /// <param name="options">Options.</param>
    /// <remarks>
    /// This method does not dispose <paramref name="textReader"/>
    /// </remarks>
    public static void MarkupWithPaging(
        this IAnsiConsole console,
        TextReader textReader,
        SpectreViewerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(textReader);

        var renderOptions = new ComputedRenderingOptions(options ?? new SpectreViewerOptions());
        
        var renderBuffer = new RenderBuffer(renderOptions.InternalWidth, renderOptions.InternalHeight);
        var pageContent = RenderEngine.Write(textReader, renderBuffer, renderOptions);

        Pager.Show(console, pageContent, renderOptions);
    }
}