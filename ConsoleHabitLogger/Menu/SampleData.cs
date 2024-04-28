using Spectre.Console;

namespace ConsoleHabitLogger.Menu;

internal class SampleData
{
    static public void Open()
    {
        if (AnsiConsole.Confirm("Are you sure you want to create sample data? It can be removed later manually."))
        {
            AnsiConsole.Progress()
                .Start(ctx => 
                {
                    ProgressTask task = ctx.AddTask("[green]Generating sample data...[/]");
                    Database.Operations.GenerateSampleData();
                    
                    while(!ctx.IsFinished)
                    {
                        task.Increment(1.5);
                    }
                });

            AnsiConsole.WriteLine("Done! Press any key to return.");
            Console.ReadKey();
        }
    }
}