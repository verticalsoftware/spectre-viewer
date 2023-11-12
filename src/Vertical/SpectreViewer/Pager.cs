using Spectre.Console;

namespace Vertical.SpectreViewer;

internal class Pager
{
    private enum UserCommand
    {
        Unknown,
        FirstPage,
        LastPage,
        PreviousLine,
        PreviousHalfPage,
        PreviousPage,
        NextLine,
        NextHalfPage,
        NextPage,
        Quit,
        Help
    }
    private readonly IAnsiConsole _console;
    private readonly ComputedRenderingOptions _options;
    private readonly StreamContent _streamContent;
    private readonly (int Column, int Line) _inputPosition;
    private readonly string _clearText;

    internal Pager(
        IAnsiConsole console,
        ComputedRenderingOptions options,
        StreamContent streamContent)
    {
        _console = console;
        _options = options;
        _streamContent = streamContent;
        _inputPosition = (1, console.Profile.Height);
        _clearText = new string(' ', console.Profile.Width);
    }

    internal void EnterPagingMode()
    {
        var offset = _streamContent.LowerOffset;
        var lastRenderedRange = RenderRange.Empty;
        
        while (true)
        {
            var renderRange = _streamContent.GetRenderOffset(offset);

            if (offset < renderRange.LowerBound)
            {
                offset = renderRange.LowerBound;
            }
            else if (offset > renderRange.LowerBound)
            {
                offset = renderRange.LowerBound;
            }
            
            // Print again?
            if (!renderRange.Equals(lastRenderedRange))
            {
                PrintPage(renderRange);
                lastRenderedRange = renderRange;
            }

                        

            switch (GetUserCommand())
            {
                case UserCommand.Quit:
                    PrintPrompt("(Exit)");
                    return;
                case UserCommand.NextLine:
                    offset++;
                    break;
                case UserCommand.NextHalfPage:
                    offset += _streamContent.PageRowCount / 2;
                    break;
                case UserCommand.NextPage:
                    offset += _streamContent.PageRowCount;
                    break;
                case UserCommand.PreviousLine:
                    offset--;
                    break;
                case UserCommand.PreviousHalfPage:
                    offset -= _streamContent.PageRowCount / 2;
                    break;
                case UserCommand.PreviousPage:
                    offset -= _streamContent.PageRowCount;
                    break;
                case UserCommand.FirstPage:
                    offset = 0;
                    break;
                case UserCommand.LastPage:
                    offset = _streamContent.UpperOffset;
                    break;
                case UserCommand.Help when !_options.InternalHelpMode:
                    ShowInternalHelp();
                    lastRenderedRange = RenderRange.Empty;
                    break;
            }
        }
    }

    private void PrintPage(RenderRange range)
    {
        var pageContent = _streamContent.GetPageContent(range);
        _console.Clear();
        _console.Markup(pageContent);
    }

    private void PrintPrompt(string? message = null)
    {
        _console.Cursor.SetPosition(_inputPosition.Column, _inputPosition.Line);
        _console.Write(_clearText);
        _console.Cursor.SetPosition(_inputPosition.Column, _inputPosition.Line);
        _console.Write(message ?? "I'm here");
    }

    private UserCommand GetUserCommand()
    {
        var key = _console.Input.ReadKey(intercept: true);

        return key switch
        {
            { Key: ConsoleKey.DownArrow, Modifiers: ConsoleModifiers.Shift } => UserCommand.NextPage, 
            { Key: ConsoleKey.PageDown } => UserCommand.NextPage,
            { Key: ConsoleKey.Spacebar } => UserCommand.NextPage,
            { Key: ConsoleKey.UpArrow, Modifiers: ConsoleModifiers.Shift } => UserCommand.PreviousPage,
            { Key: ConsoleKey.PageUp } => UserCommand.PreviousPage,
            { Key: ConsoleKey.B } => UserCommand.PreviousPage,
            { Key: ConsoleKey.DownArrow, Modifiers: ConsoleModifiers.Control } => UserCommand.NextHalfPage,
            { Key: ConsoleKey.D } => UserCommand.NextHalfPage,
            { Key: ConsoleKey.UpArrow, Modifiers: ConsoleModifiers.Control } => UserCommand.PreviousHalfPage,
            { Key: ConsoleKey.U } => UserCommand.PreviousHalfPage,
            { Key: ConsoleKey.DownArrow } => UserCommand.NextLine,
            { Key: ConsoleKey.J } => UserCommand.NextLine,
            { Key: ConsoleKey.UpArrow } => UserCommand.PreviousLine,
            { Key: ConsoleKey.K } => UserCommand.PreviousLine,
            { Key: ConsoleKey.G, Modifiers: ConsoleModifiers.Shift } => UserCommand.LastPage,
            { Key: ConsoleKey.G } => UserCommand.FirstPage,
            { Key: ConsoleKey.Escape } => UserCommand.Quit,
            { Key: ConsoleKey.Q } => UserCommand.Quit,
            { Key: ConsoleKey.Oem2, Modifiers: ConsoleModifiers.Shift } => UserCommand.Help,
            { Key: ConsoleKey.H } => UserCommand.Help,
            _ => UserCommand.Unknown
        };
    }

    private void ShowInternalHelp()
    {
        var options = new ComputedRenderingOptions(_options.CallerOptions, internalHelpMode: true);
        using var contentReader = new StringReader(InternalHelpContent.Value);
        _console.MarkupWithPaging(contentReader, options);
    }
}