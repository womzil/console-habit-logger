namespace habit_logger 
{
	static class Utils
	{
		// Safe alternative to use instead of Console.Clear() to fix problems with Visual Studio Code debugger.
		public static void ConsoleClear()
		{
			try 
			{
				Console.Clear();
			} 
			catch (IOException) 
			{
				Console.WriteLine("IOException - you can ignore it if you're using Visual Studio Code debugger.\n");
			}
		}
	}
}