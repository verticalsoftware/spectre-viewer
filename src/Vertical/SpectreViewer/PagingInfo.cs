namespace Vertical.SpectreViewer;

public record PagingInfo(
    int PageNumber,
    int PageCount,
    bool OnFirstPage,
    bool OnLastPage,
    int Rows);