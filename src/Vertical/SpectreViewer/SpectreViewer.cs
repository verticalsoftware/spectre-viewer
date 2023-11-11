using System.Text;

namespace Vertical.SpectreViewer;

public static class SpectreViewer
{
    /// <summary>
    /// Renders the content read from the given <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="str">String that contains the content.</param>
    /// <param name="options">Options.</param>
    public static void Render(string str, SpectreViewerOptions? options = null)
    {
        using var reader = new StringReader(str);
        Render(reader, options);
    }
    
    /// <summary>
    /// Renders the content read from the given <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">StringBuilder that contains the content.</param>
    /// <param name="options">Options.</param>
    public static void Render(StringBuilder stringBuilder, SpectreViewerOptions? options = null)
    {
        using var reader = new StringReader(stringBuilder.ToString());
        Render(reader, options);
    }
    
    /// <summary>
    /// Renders the content read form the provided <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">Stream that contains the content.</param>
    /// <param name="options">Options.</param>
    public static void Render(Stream stream, SpectreViewerOptions? options = null)
    {
        using var textReader = new StreamReader(stream);
        Render(textReader, options);
    }
    
    /// <summary>
    /// Renders the content read from the provided <see cref="TextReader"/>.
    /// </summary>
    /// <param name="textReader"><see cref="TextReader"/> that contains the content.</param>
    /// <param name="options">Options.</param>
    public static void Render(TextReader textReader, SpectreViewerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(textReader);

        options ??= new SpectreViewerOptions();

        var renderBuffer = new RenderBuffer(options.RenderWidth, options.RenderHeight);
        var pageContent = RenderEngine.Write(textReader, renderBuffer);
        
        Pager.Show(options.Console, pageContent, options);
    }
}