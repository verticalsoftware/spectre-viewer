using Spectre.Console;

namespace Vertical.SpectreViewer;

internal static class Pager
{
    internal static void Show(IAnsiConsole console, IPageContent content, SpectreViewerOptions options)
    {
        try
        {
            ShowInCatch(console, content, options);
        }
        catch
        {
            AnsiConsole.WriteLine("Rendering stopped.");
            throw;
        }
    }
    
    internal static void ShowInCatch(IAnsiConsole console, IPageContent content, SpectreViewerOptions options)
    {
        var width = options.RenderWidth;
        var height = options.RenderHeight;
        var index = 0;
        var clearText = new string(' ', width - 1);
        var cursorBottom = height + SpectreViewerOptions.BottomMargin;
        
        while (true)
        {
            console.Clear();
            TryPrintMarkup(console, content[index], index, options);
            console.Cursor.MoveDown(2);

            var prompt = $"Page {index + 1}/{content.PageCount}: ";
            var reprint = false;
            
            while (!reprint)
            {
                console.Cursor.SetPosition(0, cursorBottom);
                console.Write(clearText);
                console.Cursor.SetPosition(0, cursorBottom);
                console.Write(prompt);
                
                var top = index == 0;
                var bottom = index == content.PageCount - 1;
                var keyInfo = console.Input.ReadKey(intercept: true);

                switch (keyInfo)
                {
                    case null:
                        return;
                    
                    case { Key: ConsoleKey.PageDown, Modifiers: ConsoleModifiers.Shift } when !bottom:
                    case { Key: ConsoleKey.DownArrow, Modifiers: ConsoleModifiers.Shift } when !bottom:
                        index = content.PageCount - 1;
                        reprint = true;
                        break;
                    
                    case { Key: ConsoleKey.PageDown } when !bottom:
                    case { Key: ConsoleKey.DownArrow } when !bottom:
                        ++index;
                        reprint = true;
                        break;
                    
                    case { Key: ConsoleKey.PageDown }:
                    case { Key: ConsoleKey.DownArrow }:
                        prompt = "(End of file): ";
                        break;
                    
                    case { Key: ConsoleKey.PageUp, Modifiers: ConsoleModifiers.Shift } when !top:
                    case { Key: ConsoleKey.UpArrow, Modifiers: ConsoleModifiers.Shift } when !top:
                        index = 0;
                        reprint = true;
                        break;
                    
                    case { Key: ConsoleKey.PageUp } when !top:
                    case { Key: ConsoleKey.UpArrow } when !top:
                        --index;
                        reprint = true;
                        break;
                    
                    case { Key: ConsoleKey.PageUp }:
                    case { Key: ConsoleKey.UpArrow }:
                        prompt = "(Top): ";
                        break;
                    
                    case { Key: ConsoleKey.Q }:
                    case { Key: ConsoleKey.Escape }:
                        return;
                    
                    default:
                        var commands = new List<string>(6);
                        if (!top)
                        {
                            commands.Add("PgUp");
                            commands.Add("Shift+PgUp=top");
                        }

                        if (!bottom)
                        {
                            commands.Add("PgDown");
                            commands.Add("Shift+PgDown=bottom");
                        }
                        commands.Add("[q|escape]=quit");
                        prompt = $"[{string.Join(", ", commands)}]: ";
                        break;
                }
            }
        }
    }

    private static void TryPrintMarkup(IAnsiConsole console, string str, int page, SpectreViewerOptions options)
    {
        var restoreColor = AnsiConsole.Foreground;

        try
        {
            console.Markup(str);
        }
        catch (Exception exception)
        {
            var lowerLine = options.RenderHeight * page + 1;
            var upperLine = lowerLine + options.RenderHeight;
            AnsiConsole.Markup("[red]Markup error:[/] ");
            AnsiConsole.MarkupLine(exception.Message);
            AnsiConsole.MarkupLine("Error occurred while rendering page [darkorange]{0}[/], which are lines "
                                   + "[darkorange]{1}[/]-[darkorange]{2}[/] of the source stream.",
                page,
                lowerLine,
                upperLine);
            AnsiConsole.MarkupLine("The current console dimensions are width=[darkorange]{0}[/], height=[darkorange]{1}[/]",
                console.Profile.Width,
                console.Profile.Height);
            
            AnsiConsole.Foreground = Color.Grey46;
            AnsiConsole.WriteLine(str);
            throw;
        }
        finally
        {
            AnsiConsole.Foreground = restoreColor;
        }
    }
}