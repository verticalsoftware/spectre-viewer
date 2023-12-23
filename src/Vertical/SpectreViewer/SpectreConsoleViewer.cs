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
    public static void Show(string str, SpectreViewerOptions? options = null)
    {
        using var stringReader = new StringReader(str);
        Show(stringReader, options);
    }
    
    /// <summary>
    /// Renders the content read from the given <see cref="StringBuilder"/> to the console defined in the static
    /// <see cref="AnsiConsole"/> type.
    /// </summary>
    /// <param name="stringBuilder">StringBuilder that contains the content.</param>
    /// <param name="options">Options.</param>
    public static void Show(
        StringBuilder stringBuilder,
        SpectreViewerOptions? options = null)
    {
        Show(stringBuilder.ToString(), options);
    }

    /// <summary>
    /// Renders the content read form the provided <see cref="Stream"/> to the console defined in the static
    /// <see cref="AnsiConsole"/> type.
    /// </summary>
    /// <param name="path">Path to the help file.</param>
    /// <param name="options">Options.</param>
    public static void ShowFile(string path, SpectreViewerOptions? options = null)
    {
        Show(File.OpenRead(path), options);
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
    public static void Show(Stream stream, SpectreViewerOptions? options = null)
    {
        using var textReader = new StreamReader(stream);
        Show(textReader, options);
    }
    
    /// <summary>
    /// Renders the content read from the provided <see cref="TextReader"/>.
    /// </summary>
    /// <param name="textReader"><see cref="TextReader"/> that contains the content.</param>
    /// <param name="options">Options.</param>
    /// <remarks>
    /// This method does not dispose <paramref name="textReader"/>
    /// </remarks>
    public static void Show(
        TextReader textReader,
        SpectreViewerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(textReader);

         
        Show(textReader, new ComputedRenderingOptions(options ?? new SpectreViewerOptions()));
    }
    
    private static void Show(
        TextReader textReader,
        ComputedRenderingOptions options)
    {
        ArgumentNullException.ThrowIfNull(textReader);

        var console = AnsiConsole.Console;
        var buffer = new RenderBuffer(options);
        var formattingEngine = new FormatterEngine(options, buffer);
        formattingEngine.ReadStream(textReader);
        
        var pager = new Pager(console, options, buffer.GetStreamContent());
        pager.EnterPagingMode();
    }
}