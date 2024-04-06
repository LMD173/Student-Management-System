using StudentManagementSystem.Models;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem;

public class StudentManager(DatabaseManager db)
{
    private User _user = null!; // null-forgiving operator because _user will never be null when it is used.
    private readonly DatabaseManager _db = db;
    private int attempts = 0;

    /// <summary>
    /// Initialises the student manager. Fatally stops execution if something
    /// goes wrong.
    /// </summary>
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
            DisplayMenu();
            Logger.Input("Enter your choice");
            string choice = ReadInput();
            switch (choice)
            {
                case "1":
                    AddStudentChoice();
                    break;
                // case "2":
                //     ViewStudents();
                //     break;
                // case "3":
                //     ManageStudent();
                //     break;
                // case "4":
                //     SearchStudent();
                //     break;
                // case "5":
                //     if (_user.Role == "admin")
                //     {
                //         ManageUsers();
                //     }
                //     else
                //     {
                //         quit = true;
                //     }
                //     break;
                case "6":
                    quit = true;
                    break;
                default:
                    Logger.Error("Invalid choice, please try again.");
                    break;

            }
        }
        SayGoodbye();
    }

    private bool TryLogin(DatabaseManager databaseManager)
    {
        Logger.Info("Before you being, please login.");
        while (attempts < 3)
        {
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

    private static void SayGoodbye()
    {
        Logger.Info("Goodbye! Thank you for using Student Management System.");

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

    private void AddStudentChoice()
    {
        Logger.Input("Enter the student's first name");
        string firstName = ReadInput();

        Logger.Input("Enter the student's last name");
        string lastName = ReadInput();

        Logger.Input("Enter the student's date of birth (yyyy-mm-dd)");
        DateOnly dateOfBirth = ReadInputGeneric<DateOnly>();

        Logger.Input("Enter the student's height (in cm)");
        float height = ReadInputGeneric<float>();

        _db.AddStudent(firstName, lastName, dateOfBirth, height);
        Logger.Info("Student added successfully.");
    }

    private static string ReadInput()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (input == null || input == "")
            {
                Logger.Error("Invalid input, please try again.");
                Logger.Input("");
            }
            else
            {
                return input.Trim();
            }
        }
    }

    /// <summary>
    /// Reads input from the console and attempts to parse it into the specified type using reflection
    /// to invoke the TryParse method on the type. The type must have a conventional method
    /// signature of: `public static bool TryParse(string input, out T result)`.
    /// Partially generated by ChatGPT.
    /// </summary>
    /// <typeparam name="T">The type to attempt to parse the input into</typeparam>
    /// <returns>The input parsed into the type.</returns>
    public static T ReadInputGeneric<T>() where T : struct // T must be a value type
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Logger.Error("Invalid input, please try again.");
                Logger.Input("");
            }
            else
            {
                var tryParseMethod = typeof(T).GetMethod("TryParse", [typeof(string), typeof(T).MakeByRefType()]);
                if (tryParseMethod != null)
                {
                    var parameters = new object[] { input.Trim(), null! };
                    var success = (bool)(tryParseMethod.Invoke(null, parameters) ?? false);
                    if (success)
                    {
                        return (T)parameters[1];
                    }
                    else
                    {
                        Logger.Error("Invalid format, please try again.");
                        Logger.Input("");
                    }
                }
                else
                {
                    Logger.Fatal("The type does not have a TryParse method.");
                }
            }
        }
    }

}
