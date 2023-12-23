namespace Vertical.SpectreViewer;

/// <summary>
/// Defines paging modes
/// </summary>
public enum PagingMode
{
    /// <summary>
    /// Paging mode is entered if the content does not fit the console window, otherwise the content
    /// is rendered and control is returned. 
    /// </summary>
    Default,
    
    /// <summary>
    /// Paging mode is entered regardless of the content size.
    /// </summary>
    Page,
    
    /// <summary>
    /// The content is printed regardless of size and control is immediately returned.
    /// </summary>
    Print
}