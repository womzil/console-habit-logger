namespace ConsoleHabitLogger.HabitType;

public class Time(string id, DateTime startTime, DateTime endTime)
{
    private string id = id;
    private DateTime startTime = startTime;
    private DateTime endTime = endTime;
    private TimeSpan duration = endTime.Subtract(startTime);
}