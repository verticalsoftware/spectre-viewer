namespace Vertical.SpectreViewer;

internal static class InternalHelpContent
{
    internal const string Value =
        """
        Content navigation:
        
            Next page            PgDown, Shift+DownArrow, Space
            Previous page        PgUp, Shift+UpArrow, b
            Next half page       Ctrl+DownArrow, d
            Previous half page   Ctrl+UpArrow, u
            Next line            DownArrow, j
            Previous line        UpArrow, k
            Last page            G
            First page           g
            ----------------------------------------------------
            Navigation help      h, ?
            Quit                 Escape, q
        """;

    internal static readonly string[] Values =
    {
        "                                                    ",
        "  Next page              PgDown, Shift+Down, Space  ",
        "  Previous page          PgUp, Shift+Up, b          ",
        "  Next 1/2 page          Ctrl+Down, d               ",
        "  Previous 1/2 page      Ctrl+Up, u                 ",
        "  Next line              Down, j                    ",
        "  Previous line          Up, k                      ",
        "  Last page              G                          ",
        "  First page             g                          ",
        "  ------------------------------------------------  ",
        "  Navigation help        h, ?                       ",
        "  Quit                   Escape, q                  ",
        "                                                    "
    };
}