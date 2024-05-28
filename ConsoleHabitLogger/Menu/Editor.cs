using System.Globalization;
using ConsoleHabitLogger.Database;
using Spectre.Console;

namespace ConsoleHabitLogger.Menu;

public class Editor
{
    private static int _habitsPage = 1;

    private static int _activitiesPage = 1;

    public static void OpenHabitEditor()
    {
        while (true)
        {
            CreateLayout(page: _habitsPage);

            int numberOfPages = Utils.NumberOfPages(-1);
            ConsoleKey key = AnsiConsole.Console.Input.ReadKey(true).Value.Key;

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    _habitsPage = _habitsPage == 1 ? numberOfPages : _habitsPage - 1;
                    continue;
                case ConsoleKey.RightArrow:
                    _habitsPage = _habitsPage == numberOfPages ? 1 : _habitsPage + 1;
                    continue;
                case ConsoleKey.A:
                case ConsoleKey.Add:
                    Utils.ConsoleClear();
                    string habit = AnsiConsole.Ask<string>("How do you want to name your [green]habit[/]?");
                    string unit = AnsiConsole.Ask<string>("What [green]unit[/] do you want to use for it?").ToLower();

                    if ((unit.Contains("time") || unit.Contains("second") || unit.Contains("minute") || unit.Contains("hour") || unit.Contains("day") || unit.Contains("week") || unit.Contains("month") || unit.Contains("year")) && AnsiConsole.Confirm("Do you want to use time selector?"))
                    {
                        unit = "time";
                    }

                    Database.Operations.CreateHabit(habit, unit);
                    AnsiConsole.WriteLine($"Successfully created a habit with name \"{habit}\" and unit \"{unit}\".");
                    Console.ReadKey();
                    break;
                case ConsoleKey.Enter:
                    int idToEnter = Validation.SelectId(true);
                    OpenActivityEditor(idToEnter);
                    break;
                case ConsoleKey.E:
                    int idToEdit = Validation.SelectId(true);
                    string editName = AnsiConsole.Ask<string>("How do you want to name your [green]habit[/]?");
                    string editUnit = AnsiConsole.Ask<string>("What [green]unit[/] do you want to use for it?").ToLower();
                    
                    Operations.EditHabit(idToEdit, editName, editUnit);
                    break;
                case ConsoleKey.R:
                case ConsoleKey.Delete:
                    int idToRemove = Validation.SelectId(true);

                    if (AnsiConsole.Confirm($"Are you sure you want to remove habit with id \"{idToRemove}\"?"))
                    {
                        Database.Operations.RemoveHabit(idToRemove);
                    }

                    break;
                case ConsoleKey.Escape:
                    break;
                default:
                    key = AnsiConsole.Console.Input.ReadKey(true).Value.Key;
                    break;
            }

            break;
        }
    }

    public static void OpenActivityEditor(int habitId)
    {
        while (true)
        {
            bool useTimeSelector = Database.Operations.UsesTimeSelector(habitId);

            CreateLayout(habitId, _activitiesPage);

            int numberOfPages = Utils.NumberOfPages(habitId);
            ConsoleKey key = AnsiConsole.Console.Input.ReadKey(true).Value.Key;

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    _activitiesPage = _activitiesPage == 1 ? numberOfPages : _activitiesPage - 1;
                    continue;
                case ConsoleKey.RightArrow:
                    _activitiesPage = _activitiesPage == numberOfPages ? 1 : _activitiesPage + 1;
                    continue;
                case ConsoleKey.A:
                case ConsoleKey.Add:
                    Utils.ConsoleClear();
                    string description = AnsiConsole.Ask<string>("How do you want to describe your [green]activity[/]?");

                    if (!useTimeSelector)
                    {
                        string amountString = AnsiConsole.Ask<string>("What's the [green]amount[/]?").ToLower();
                        double amount = 0;

                        while (!double.TryParse(amountString, out amount))
                        {
                            AnsiConsole.MarkupLine("[red]It has to be a number![/]");
                            amountString = AnsiConsole.Ask<string>("What's the [green]amount[/]?").ToLower();
                        }

                        Database.Operations.CreateActivity(habitId, amount.ToString(CultureInfo.CurrentCulture), description);
                        AnsiConsole.WriteLine($"Successfully created an activity with description \"{description}\" and with amount of \"{amount}\".");
                    }
                    else
                    {
                        TimeSpan duration = TimeSelector(habitId);
                        Database.Operations.CreateActivity(habitId, duration.ToString(), description);
                        AnsiConsole.WriteLine($"Successfully created an activity with description \"{description}\" and with timespan of \"{duration}\".");
                    }

                    Console.ReadKey();
                    break;
                case ConsoleKey.E:
                    int idToEdit = Validation.SelectId(true);
                    string editDescription = AnsiConsole.Ask<string>("How do you want to describe your [green]activity[/]?");
                    
                    if (!useTimeSelector)
                    {
                        string amountString = AnsiConsole.Ask<string>("What's the [green]amount[/]?").ToLower();
                        double amount = 0;

                        while (!double.TryParse(amountString, out amount))
                        {
                            AnsiConsole.MarkupLine("[red]It has to be a number![/]");
                            amountString = AnsiConsole.Ask<string>("What's the [green]amount[/]?").ToLower();
                        }

                        Database.Operations.EditActivity(idToEdit, amount.ToString(CultureInfo.CurrentCulture), editDescription);
                        AnsiConsole.WriteLine($"Successfully edited an activity with description \"{editDescription}\" and with amount of \"{amount}\".");
                    }
                    else
                    {
                        TimeSpan editDuration = TimeSelector(idToEdit);
                        Database.Operations.EditActivity(idToEdit, editDuration.ToString(), editDescription);
                        AnsiConsole.WriteLine(
                            $"Successfully edited an activity with description \"{editDescription}\" and with timespan of \"{editDuration}\".");
                    }

                    break;
                case ConsoleKey.R:
                case ConsoleKey.Delete:
                    int id = Validation.SelectId(false);

                    if (AnsiConsole.Confirm($"Are you sure you want to remove activity with id \"{id}\"?"))
                    {
                        Database.Operations.RemoveActivity(id);
                    }

                    break;
                case ConsoleKey.Escape:
                    break;
                default:
                    key = AnsiConsole.Console.Input.ReadKey(true).Value.Key;
                    break;
            }

            break;
        }
    }

    private static void CreateLayout(int habitId = -1, int page = 1)
    {
        Utils.ConsoleClear();

        int numberOfPages = Utils.NumberOfPages(habitId);

        if (habitId == -1)
        {
            if (_habitsPage > numberOfPages)
            {
                page = 1;
                _habitsPage = 1;
            }

            Table table = new Table();
            BreakdownChart chart = new BreakdownChart().Width(60);

            table.Title($"Habits, {page}/{numberOfPages}");
            table.AddColumn(new TableColumn("ID").Centered());
            table.AddColumn(new TableColumn("Name").Centered());
            table.AddColumn(new TableColumn("Measure unit").Centered());
            table.AddColumn(new TableColumn("Number of entries").Centered());

            foreach (List<string> row in Database.Operations.ReadHabits(10, page * 10 - 10))
            {
                row.Add(Database.Operations.GetAmountOfRows(int.Parse(row[0])));
                table.AddRow(row.ToArray());
                chart.AddItem(row[1], double.Parse(row[3]), Utils.GetRandomColor());
            }

            AnsiConsole.Write(table);

            AnsiConsole.Write(new Rows(
                new Text("Controls:"),
                new Text("Left/Right arrow - change page"),
                new Text("Enter - See records of a habit"),
                new Text("E - Edit a habit"),
                new Text("A - Add a new habit"),
                new Text("R - Remove habit"),
                new Text("")
            ));

            AnsiConsole.Write(chart);
        }
        else
        {
            if (_activitiesPage > numberOfPages)
            {
                page = 1;
                _activitiesPage = 1;
            }

            Table table = new Table();
            BreakdownChart chart = new BreakdownChart().Width(60);

            table.Title($"Activities of {Database.Operations.GetHabitName(habitId)}, {page}/{numberOfPages}");
            table.AddColumn(new TableColumn("ID").Centered());
            table.AddColumn(new TableColumn("Amount").Centered());
            table.AddColumn(new TableColumn("Description").Centered());
            table.AddColumn(new TableColumn("Time created").Centered());

            foreach (List<string> row in Database.Operations.ReadActivities(habitId, 10, page * 10 - 10))
            {
                table.AddRow(row.ToArray());

                if (!double.TryParse(row[1], out double amount))
                {
                    if (TimeSpan.TryParse(row[1], out TimeSpan duration))
                    {
                        amount = duration.TotalMinutes;
                    }
                }

                chart.AddItem(row[0], amount, Utils.GetRandomColor());
            }

            AnsiConsole.Write(table);

            AnsiConsole.Write(new Rows(
                new Text("Controls:"),
                new Text("Left/Right arrow - change page"),
                new Text("E - Edit an activity"),
                new Text("A - Add a new activity"),
                new Text("R - Remove activity"),
                new Text("")
            ));

            AnsiConsole.Write(chart);
        }
    }

    private static TimeSpan TimeSelector(int habitId)
    {
        AnsiConsole.MarkupLine($"Date/time format you should use: [green]{DateTime.Now}[/]");

        TimeSpan duration;

        do
        {
            DateTime startDate;
            string startDateString = AnsiConsole.Ask<string>("What's the [green]start time/date[/]?").ToLower();

            while (!DateTime.TryParse(startDateString, out startDate))
            {
                AnsiConsole.MarkupLine("[red]It has to be a valid date![/]");
                startDateString = AnsiConsole.Ask<string>("What's the [green]start time/date[/]?").ToLower();
            }

            DateTime endDate;
            string endDateString = AnsiConsole.Ask<string>("What's the [green]end time/date[/]?").ToLower();

            while (!DateTime.TryParse(endDateString, out endDate))
            {
                AnsiConsole.MarkupLine("[red]It has to be a valid date![/]");
                endDateString = AnsiConsole.Ask<string>("What's the [green]end time/date[/]?").ToLower();
            }

            duration = endDate.Subtract(startDate);

            if (duration <= TimeSpan.Zero)
            {
                AnsiConsole.MarkupLine("[red]Time spent on an activity can't be equal or lower than 0![/]");
            }
        } while (duration <= TimeSpan.Zero);

        return duration;
    }
}