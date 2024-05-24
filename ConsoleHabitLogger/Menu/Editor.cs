using Spectre.Console;

namespace ConsoleHabitLogger.Menu;

public class Editor
{
    public static void Open()
    {
        Utils.ConsoleClear();

        Table table = new Table();

        table.Title("Habits");
        table.AddColumn(new TableColumn("ID").Centered());
        table.AddColumn(new TableColumn("Name").Centered());
        table.AddColumn(new TableColumn("Measure unit").Centered());
        table.AddColumn(new TableColumn("Number of entries").Centered());

        BreakdownChart chart = new BreakdownChart().Width(60);

        foreach (List<string> row in Database.Operations.ReadHabits(10, 0))
        {
            row.Add(Database.Operations.GetAmountOfRows(int.Parse(row[0])));
            table.AddRow(row.ToArray());
            chart.AddItem(row[1], double.Parse(row[3]), Utils.GetRandomColor());
        }

        AnsiConsole.Write(table);

        AnsiConsole.Write(new Rows(
            new Text("Controls:"),
            new Text("Enter - See records of a habit"),
            new Text("E - Edit a habit"),
            new Text("A - Add a new habit"),
            new Text("R - Remove habit"),
            new Text("")
        ));

        AnsiConsole.Write(chart);

        ConsoleKey key = AnsiConsole.Console.Input.ReadKey(true).Value.Key;

        switch (key)
        {
            case ConsoleKey.A:
            case ConsoleKey.Add:
                Utils.ConsoleClear();
                string habit = AnsiConsole.Ask<string>("How do you want to name your [green]habit[/]?");
                string unit = AnsiConsole.Ask<string>("What [green]unit[/] do you want to use for it?").ToLower();

                if ((unit.Contains("time") || unit.Contains("second") || unit.Contains("minute") ||
                    unit.Contains("hour") || unit.Contains("day") || unit.Contains("week") || unit.Contains("month") ||
                    unit.Contains("year")) && AnsiConsole.Confirm("Do you want to use time selector?"))
                {
                    unit = "time";
                }

                Database.Operations.CreateHabit(habit, unit);
                AnsiConsole.WriteLine($"Successfully created a habit with name \"{habit}\" and unit \"{unit}\".");
                Console.ReadKey();
                break;
            case ConsoleKey.Enter:
                Validation.SelectId(true);
                break;
            case ConsoleKey.E:
                Validation.SelectId(true);
                break;
            case ConsoleKey.R:
            case ConsoleKey.Delete:
                int id = Validation.SelectId(true);

                if (AnsiConsole.Confirm($"Are you sure you want to remove habit with id \"{id}\"?"))
                {
                    Database.Operations.RemoveHabit(id);
                }

                break;
        }
    }


}