// See https://aka.ms/new-console-template for more information

using Vertical.SpectreViewer;

var options = new SpectreViewerOptions
{
    LineNumbers = true,
    RenderWidth = 188,
    RenderHeight = 60
};

SpectreConsoleViewer.MarkupWithPaging(File.OpenRead("troubleshoot.txt"), options);
