namespace JarFileCleaner;

public static class Logger
{
    public static bool Verbose { get; set; }
    public static void Info(string message)
    {
        if (!Verbose)
            return;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("[INFO]: " + message);
    }
    
    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red; 
        Console.WriteLine("[ERROR]: " + message);
    }
    
    public static void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[WARN]: " + message);
    }
}