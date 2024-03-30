using StudentManagementSystem.Models;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem;

public class StudentManager(DatabaseManager db)
{
    private User _user = null!; // null-forgiving operator because _user will never be null when it is used.
    private readonly DatabaseManager _db = db;
    private int attempts = 0;

    public void Initialise()
    {
        if (!TryLogin(_db))
        {
            Logger.Fatal("You have exceeded the maximum number of login attempts.");
        }
        Greet();
    }

    public void Run()
    {
        bool quit = false;
        while (!quit)
        {
            //todo
        }
    }

    private bool TryLogin(DatabaseManager databaseManager)
    {
        while (attempts < 3)
        {
            Logger.Info("Before you being, please login.");
            Logger.Input("Enter your email");
            string email = Console.ReadLine()!;
            Logger.Input("Enter your password");
            string password = Console.ReadLine()!;

            var user = databaseManager.LogUserIn(email, password);
            if (user is null)
            {
                attempts++;
                Console.Clear();
                Logger.Error("Invalid email or password.");
                Logger.Info($"You have {3 - attempts} attempt(s) remaining.");
                continue;
            }

            _user = user;
            return true;
        }
        return false;
    }

    public void Greet()
    {
        Console.Clear();
        Logger.Info("Hello! Welcome to Student Management System.");
        Logger.Info($"You are logged in as '{_user.Email}'.");
        Logger.Info("This is an application to manage students.");
        Logger.Info("Please select an option from the menu to start.");
    }

    public void DisplayMenu()
    {
        Logger.Log("-------- Menu --------");
        Logger.Log("1. Add a new student");
        Logger.Log("2. View all students");
        Logger.Log("3. Select a student to manage");
        Logger.Log("4. Search for a student");

        if (_user.Role == "admin")
        {
            Logger.Log("5. Manage users");
            Logger.Log("6. Exit");
        }
        else
        {
            Logger.Log("5. Exit");
        }

        Logger.Log("-----------------------");
    }
}