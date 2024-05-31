using System.Globalization;
using System.Linq;
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
                    string habit = AnsiConsole.Ask<string>(Localization.GetString("habit_name"));
                    string unit = AnsiConsole.Ask<string>(Localization.GetString("habit_unit")).ToLower();

                    if ((unit.Contains(Localization.GetString("time")) || unit.Contains(Localization.GetString("second")) || unit.Contains(Localization.GetString("minute")) || unit.Contains(Localization.GetString("hour")) || unit.Contains(Localization.GetString("day")) || unit.Contains(Localization.GetString("week")) || unit.Contains(Localization.GetString("month")) || unit.Contains(Localization.GetString("year"))) && AnsiConsole.Confirm(Localization.GetString("time_selector")))
                    {
                        unit = "time";
                    }

                    Database.Operations.CreateHabit(habit, unit);
                    AnsiConsole.WriteLine(Localization.GetString("habit_created"), habit, unit);
                    Console.ReadKey();
                    break;
                case ConsoleKey.Enter:
                    int idToEnter = Validation.SelectId(true);
                    OpenActivityEditor(idToEnter);
                    break;
                case ConsoleKey.E:
                    int idToEdit = Validation.SelectId(true);
                    string editName = AnsiConsole.Ask<string>(Localization.GetString("habit_name"));
                    string editUnit = AnsiConsole.Ask<string>(Localization.GetString("habit_unit")).ToLower();
                    
                    Operations.EditHabit(idToEdit, editName, editUnit);
                    break;
                case ConsoleKey.R:
                case ConsoleKey.Delete:
                    int idToRemove = Validation.SelectId(true);

                    if (AnsiConsole.Confirm(String.Format(Localization.GetString("habit_remove"), idToRemove)))
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
                    string description = AnsiConsole.Ask<string>(Localization.GetString("activity_description"));

                    if (!useTimeSelector)
                    {
                        string amountString = AnsiConsole.Ask<string>(Localization.GetString("activity_amount")).ToLower();
                        double amount = 0;

                        while (!double.TryParse(amountString, out amount))
                        {
                            AnsiConsole.MarkupLine(Localization.GetString("number_required"));
                            amountString = AnsiConsole.Ask<string>(Localization.GetString("activity_amount")).ToLower();
                        }

                        Database.Operations.CreateActivity(habitId, amount.ToString(CultureInfo.CurrentCulture), description);
                        AnsiConsole.WriteLine(Localization.GetString("activity_created"), description, amount);
                    }
                    else
                    {
                        TimeSpan duration = TimeSelector(habitId);
                        Database.Operations.CreateActivity(habitId, duration.ToString(), description);
                        AnsiConsole.WriteLine(Localization.GetString("activity_created_time"), description, duration);
                    }

                    Console.ReadKey();
                    break;
                case ConsoleKey.E:
                    int idToEdit = Validation.SelectId(false);
                    string editDescription = AnsiConsole.Ask<string>(Localization.GetString("activity_description"));
                    
                    if (!useTimeSelector)
                    {
                        string amountString = AnsiConsole.Ask<string>(Localization.GetString("activity_amount")).ToLower();
                        double amount = 0;

                        while (!double.TryParse(amountString, out amount))
                        {
                            AnsiConsole.MarkupLine(Localization.GetString("number_required"));
                            amountString = AnsiConsole.Ask<string>(Localization.GetString("activity_amount")).ToLower();
                        }

                        Database.Operations.EditActivity(idToEdit, amount.ToString(CultureInfo.CurrentCulture), editDescription);
                        AnsiConsole.WriteLine(Localization.GetString("activity_edited"), editDescription, amount);
                    }
                    else
                    {
                        TimeSpan editDuration = TimeSelector(idToEdit);
                        Database.Operations.EditActivity(idToEdit, editDuration.ToString(), editDescription);
                        AnsiConsole.WriteLine(
                            Localization.GetString("activity_edited_time"), editDescription, editDuration);
                    }

                    break;
                case ConsoleKey.R:
                case ConsoleKey.Delete:
                    int id = Validation.SelectId(false);

                    if (AnsiConsole.Confirm(String.Format(Localization.GetString("activity_remove_confirm"), id)))
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

            table.Title(string.Format(Localization.GetString("habits"), page, numberOfPages));
            table.AddColumn(new TableColumn(Localization.GetString("id")).Centered());
            table.AddColumn(new TableColumn(Localization.GetString("name")).Centered());
            table.AddColumn(new TableColumn(Localization.GetString("unit")).Centered());
            table.AddColumn(new TableColumn(Localization.GetString("entries")).Centered());

            foreach (List<string> row in Database.Operations.ReadHabits(10, page * 10 - 10))
            {
                row.Add(Database.Operations.GetAmountOfRows(int.Parse(row[0])));
                table.AddRow(row.ToArray());
                chart.AddItem(row[1], double.Parse(row[3]), Utils.GetRandomColor());
            }

            AnsiConsole.Write(table);

            AnsiConsole.Write(new Rows(
                new Text(Localization.GetString("controls")),
                new Text(Localization.GetString("change_page")),
                new Text(Localization.GetString("see_habit")),
                new Text(Localization.GetString("add_habit")),
                new Text(Localization.GetString("remove_habit")),
                new Text(Localization.GetString("edit_habit")),
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

            table.Title(string.Format(Localization.GetString("activities"), Database.Operations.GetHabitName(habitId), page, numberOfPages));
            table.AddColumn(new TableColumn(Localization.GetString("id")).Centered());
            table.AddColumn(new TableColumn(Localization.GetString("amount")).Centered());
            table.AddColumn(new TableColumn(Localization.GetString("description")).Centered());
            table.AddColumn(new TableColumn(Localization.GetString("time_created")).Centered());

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

                chart.AddItem(row[2], amount, Utils.GetRandomColor());
            }

            AnsiConsole.Write(table);

            AnsiConsole.Write(new Rows(
                new Text(Localization.GetString("controls")),
                new Text(Localization.GetString("change_page")),
                new Text(Localization.GetString("edit_activity")),
                new Text(Localization.GetString("add_activity")),
                new Text(Localization.GetString("remove_activity")),
                new Text("")
            ));

            AnsiConsole.Write(chart);
        }
    }

    private static TimeSpan TimeSelector(int habitId)
    {
        AnsiConsole.MarkupLine(Localization.GetString("date_format"), DateTime.Now);

        TimeSpan duration;

        do
        {
            DateTime startDate;
            string startDateString = AnsiConsole.Ask<string>(Localization.GetString("start_time")).ToLower();

            while (!DateTime.TryParse(startDateString, out startDate))
            {
                AnsiConsole.MarkupLine(Localization.GetString("invalid_date"));
                startDateString = AnsiConsole.Ask<string>(Localization.GetString("start_time")).ToLower();
            }

            DateTime endDate;
            string endDateString = AnsiConsole.Ask<string>(Localization.GetString("end_time")).ToLower();

            while (!DateTime.TryParse(endDateString, out endDate))
            {
                AnsiConsole.MarkupLine(Localization.GetString("invalid_date"));
                endDateString = AnsiConsole.Ask<string>(Localization.GetString("end_time")).ToLower();
            }

            duration = endDate.Subtract(startDate);

            if (duration <= TimeSpan.Zero)
            {
                AnsiConsole.MarkupLine(Localization.GetString("time_less_than_zero"));
            }
        } while (duration <= TimeSpan.Zero);

        return duration;
    }
}