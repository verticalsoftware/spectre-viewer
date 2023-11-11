namespace Vertical.SpectreViewer;

/// <summary>
/// Represents content rendered to an indexed page.
/// </summary>
public interface IPageContent
{
    /// <summary>
    /// Gets content for the specified page index.
    /// </summary>
    /// <param name="index"></param>
    string this[int index] { get; }
    
    /// <summary>
    /// Gets the page count.
    /// </summary>
    int PageCount { get; }
}