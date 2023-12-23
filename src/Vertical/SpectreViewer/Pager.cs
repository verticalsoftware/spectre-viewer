using System.Text;
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
        _clearText = new string(' ', console.Profile.Width );
    }

    internal void EnterPagingMode()
    {
        var offset = _streamContent.LowerOffset;
        var lastRenderedRange = RenderInfo.Empty;
        var paging = _streamContent.RowCount > _options.RenderHeight
                     || _options.CallerOptions.PagingMode == PagingMode.Page;
        
        while (true)
        {
            var renderInfo = _streamContent.GetRenderInfo(offset);

            if (offset < renderInfo.LowerBound)
            {
                offset = renderInfo.LowerBound;
            }
            else if (offset > renderInfo.LowerBound)
            {
                offset = renderInfo.LowerBound;
            }
            
            // Print again?
            if (!renderInfo.Equals(lastRenderedRange))
            {
                PrintPage(renderInfo, paging);

                if (!paging)
                    return;
                
                lastRenderedRange = renderInfo;
                PrintPrompt(renderInfo);
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
                case UserCommand.Help:
                    ShowInternalHelp(_console);
                    lastRenderedRange = RenderInfo.Empty;
                    break;
            }
        }
    }

    private void PrintPage(RenderInfo info, bool paging)
    {
        if (paging)
        {
            _console.Clear();
        }
        
        var pageContent = _streamContent.GetPageContent(info);
        _console.Markup(pageContent);
    }

    private void PrintPrompt(RenderInfo renderInfo)
    {
        var prompt = renderInfo.RowCount > 0
            ? $"{renderInfo.LowerBound + 1}-{renderInfo.UpperBound}/{renderInfo.RowCount}: "
            : "(Help empty) ";
        
        PrintPrompt(prompt);
    }
    
    private void PrintPrompt(string message)
    {
        MoveToPromptAndClear();
        _console.Write(message);
    }

    private void MoveToPromptAndClear()
    {
        _console.Cursor.SetPosition(_inputPosition.Column, _inputPosition.Line);
        _console.Write(_clearText);
        _console.Cursor.SetPosition(_inputPosition.Column, _inputPosition.Line);
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
            { KeyChar: '?' } => UserCommand.Help,
            { Key: ConsoleKey.H } => UserCommand.Help,
            _ => UserCommand.Unknown
        };
    }

    private void ShowInternalHelp(IAnsiConsole console)
    {
        var help = InternalHelpContent.Values;
        var center = console.Profile.Width / 2 - help[0].Length / 2;
        var top = console.Profile.Height / 2 - help.Length / 2;
        
        foreach(var line in help)
        {
            console.Cursor.SetPosition(center, top++);
            console.Markup($"[white on grey15]{line}[/]");
        }

        MoveToPromptAndClear();
        console.Write("Press any key.. ");
        console.Input.ReadKey(intercept: true);
    }
}