namespace StudentManagementSystem;
using StudentManagementSystem.Utilities;
using StudentManagementSystem.Models;

/// <summary>
/// The main entry point for the student management system application.
/// </summary>
class Program
{
    static void Main()
    {
        string? dbPath = Environment.GetEnvironmentVariable("SMS_DB_PATH");
        if (string.IsNullOrEmpty(dbPath))
        {
            Logger.Fatal("The environment variable `SMS_DB_PATH` is not set or could not be found.");
        }

        DatabaseManager db = null!;
        try
        {
            db = new(dbPath!);
        }
        catch (Exception e)
        {
            Logger.Fatal(e.Message);
        }

        StudentManager studentManager = new(db!);
        studentManager.Initialise();
        studentManager.Run();
    }
}

// a8dHA73*!&£aHA4@