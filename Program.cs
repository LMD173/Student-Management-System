using StudentManagementSystem.Utilities;
using StudentManagementSystem.Controllers;

namespace StudentManagementSystem;

class Program
{
    /// <summary>
    /// The entry point of the program.
    /// </summary>
    static void Main()
    {
        string dbPath = Environment.GetEnvironmentVariable("SMS_DB_PATH") ?? "./sms-data.db";
        if (!File.Exists(dbPath))
        {
            Logger.Fatal("Database file not found.");
            return;
        }


        DatabaseController db = null!;
        try
        {
            db = new(dbPath!);
        }
        catch (Exception e)
        {
            Logger.Fatal(e.Message);
        }

        StudentController studentManager = new(db);
        studentManager.Initialise();
        studentManager.Run();
    }
}

// a8dHA73*!&£aHA4@
// 9Hdb&263*2bd9d5£