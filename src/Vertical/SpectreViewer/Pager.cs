using Spectre.Console;

namespace Vertical.SpectreViewer;

internal static class Pager
{
    internal static void Show(IAnsiConsole console, IPageContent content, SpectreViewerOptions options)
    {
        var width = options.RenderWidth;
        var height = options.RenderHeight;
        var index = 0;
        var clearText = new string(' ', width - 1);
        var cursorBottom = height + SpectreViewerOptions.BottomMargin;
        
        while (true)
        {
            console.Clear();
            console.Markup(content[index]);
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
                        index = content.PageCount - 1;
                        reprint = true;
                        break;
                    
                    case { Key: ConsoleKey.PageDown } when !bottom:
                        ++index;
                        reprint = true;
                        break;
                    
                    case { Key: ConsoleKey.PageDown }:
                        prompt = "(End of file): ";
                        break;
                    
                    case { Key: ConsoleKey.PageUp, Modifiers: ConsoleModifiers.Shift } when !top:
                        index = 0;
                        reprint = true;
                        break;
                    
                    case { Key: ConsoleKey.PageUp } when !top:
                        --index;
                        reprint = true;
                        break;
                    
                    case { Key: ConsoleKey.PageUp }:
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
}