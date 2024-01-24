namespace StudentManagementSystem.Utilities;

public class Logger
{

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">the message to output.</param>
    public static void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"[INFO] {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">the error message to output.</param>
    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Logs a success message.
    /// </summary>
    /// <param name="message">the success message to output.</param>
    public static void Success(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[SUCCESS] {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">the warning message to output.</param>
    public static void Warning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARNING] {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Logs a fatal error and exits the application.
    /// </summary>
    /// <param name="message">the error message to output.</param>
    /// <param name="code">an optional code to exit the environment with.</param>
    public static void Fatal(string message, int code = 1)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"[FATAL] {message}");
        Console.ResetColor();
        Environment.Exit(code);
    }
}