using StudentManagementSystem.Utilities;
using StudentManagementSystem.Controllers;
using StudentManagementSystem.Models;
using System.Globalization;

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

        User? user = TryLogin(db);
        if (user is null)
        {
            Logger.Fatal("You have exceeded the maximum number of attempts (3). Exiting...");
            return;
        }

        StudentController studentManager = new(user, db);
        studentManager.Run();
    }

    /// <summary>
    /// Tries to login the user. Returns the user if the login is successful, null otherwise (after 3 attempts).
    /// </summary>
    /// 
    /// <param name="db">the database controller instance.</param>
    /// 
    /// <returns>The user object, or null.</returns>
    private static User? TryLogin(DatabaseController db)
    {
        Logger.Info("Before you begin, please login.");
        int attempts = 0;
        while (attempts < 3)
        {
            Logger.Input("Enter your email");
            string email = Console.ReadLine()!.Trim();
            Logger.Input("Enter your password");
            string password = Console.ReadLine()!.Trim();

            var user = db.GetUser(email, password);
            if (user is null)
            {
                attempts++;
                Console.Clear();
                Logger.Error("Invalid email or password.");
                Logger.Info($"You have {3 - attempts} attempt(s) remaining.");
                continue;
            }

            return user;
        }
        return null;
    }
}