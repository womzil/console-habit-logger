﻿using Spectre.Console;

namespace ConsoleHabitLogger.Menu;

public class Editor
{
    public static void Open()
    {
        Utils.ConsoleClear();
        
        Table tableOfHabits = new Table();

        tableOfHabits.Title("Habits"); 
        tableOfHabits.AddColumn(new TableColumn("ID").Centered());
        tableOfHabits.AddColumn(new TableColumn("Name").Centered());
        tableOfHabits.AddColumn(new TableColumn("Measure unit").Centered());
        tableOfHabits.AddColumn(new TableColumn("Number of entries").Centered());

        tableOfHabits.AddRow("2", "Water drunk", "litres (l)", "500");
        
        AnsiConsole.Write(tableOfHabits);
        
        AnsiConsole.Write(new Rows(
            new Text("Controls:"),
            new Text("E - Edit a habit"),
            new Text("A - Add a new habit"),
            new Text("R - Remove habit"),
            new Text("")
        ));
        
        AnsiConsole.Write(new BreakdownChart()
            .Width(60)
            .AddItem("Water drunk", 30.5, Utils.GetRandomColor())
            .AddItem("HTML", 28.3, Utils.GetRandomColor())
            .AddItem("C#", 22.6, Utils.GetRandomColor())
            .AddItem("JavaScript", 16.2, Utils.GetRandomColor())
            .AddItem("Ruby", 16, Utils.GetRandomColor())
            .AddItem("Shell", 12.1, Utils.GetRandomColor()));
        
        AnsiConsole.Console.Input.ReadKey(false);
    }
}