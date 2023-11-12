// See https://aka.ms/new-console-template for more information

using Vertical.SpectreViewer;

var options = new SpectreViewerOptions
{
    LineNumbers = true,
    RenderHeight = 12,
    RenderWidth = 80,
    PageOverlapRows = 0
};

SpectreConsoleViewer.MarkupWithPaging(File.OpenRead("xp.txt"), options);
