// See https://aka.ms/new-console-template for more information

using Vertical.SpectreViewer;

var options = new SpectreViewerOptions
{
    LineNumbers = false,
    PageOverlapRows = 0,
    //RenderHeight = 16,
    //RenderWidth = 132,
    Styles =
    {
        ["section"] = "darkorange",
        ["opt"] = "dodgerblue1",
        ["var"] = "fuchsia"
    } 
};

SpectreConsoleViewer.MarkupWithPaging(File.OpenRead("grep.txt"), options);
