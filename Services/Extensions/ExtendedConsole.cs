namespace Services.Extensions
{
    public static class ExtendedConsole
    {
        public static void ExitApplicationWithMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ResetColor();
            Console.WriteLine("Hit ENTER te exit");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
