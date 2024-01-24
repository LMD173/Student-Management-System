using StudentManagementSystem.Utilities;

namespace StudentManagementSystem;

/// <summary>
/// The main entry point for the student management system application.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        string? dbPath = Environment.GetEnvironmentVariable("SMS_DB_PATH");
        if (string.IsNullOrEmpty(dbPath))
        {
            Logger.Fatal("The environment variable `SMS_DB_PATH` is not set or could not be found.");
        }

        DatabaseManager db = new(dbPath!);
    }
}