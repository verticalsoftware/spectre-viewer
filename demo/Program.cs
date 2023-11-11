// See https://aka.ms/new-console-template for more information

using Vertical.SpectreViewer;

var options = new SpectreViewerOptions
{
    LineNumbers = true
};

SpectreConsoleViewer.MarkupWithPaging(File.OpenRead("troubleshoot.txt"), options);
