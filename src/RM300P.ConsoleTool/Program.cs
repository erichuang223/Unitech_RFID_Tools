using RM300P.Core.Commands;
using RM300P.ConsoleTool.Menu;
using Spectre.Console;

AnsiConsole.MarkupLine("[bold cyan]RM300P Bench Tool[/]  啟動中…");

using var client = new Rm300pClient();
var runner = new MenuRunner(client);

try
{
    await runner.RunAsync();
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
}
finally
{
    AnsiConsole.MarkupLine("[dim]已結束。[/]");
}
