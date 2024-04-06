using System.Text.RegularExpressions;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem;

public partial class StudentManager(DatabaseManager db)
{
    private User _user = null!; // null-forgiving operator because _user will never be null when it is used.
    private readonly DatabaseManager _db = db;
    private int _attempts = 0;

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
                    Console.Clear();
                    ViewAllStudentsChoice();
                    break;
                case "2":
                    Console.Clear();
                    GetStudentDetailsChoice();
                    break;
                case "3":
                    Console.Clear();
                    if (_user.Role == "admin")
                    {
                        AddStudentChoice();
                    }
                    else
                    {
                        quit = true;
                    }
                    break;
                case "4":
                    Console.Clear();
                    if (_user.Role == "admin")
                    {
                        ModifyStudentChoice();
                    }
                    else
                    {
                        Logger.Error("Invalid choice, please try again.");
                    }
                    break;
                // case "5":
                //     todo: implement
                // case "6":
                //     todo: implement
                case "7":
                    if (_user.Role == "admin")
                    {
                        quit = true;
                    }
                    else
                    {
                        Logger.Error("Invalid choice, please try again.");
                    }
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
        while (_attempts < 3)
        {
            Logger.Input("Enter your email");
            string email = Console.ReadLine()!;
            Logger.Input("Enter your password");
            string password = Console.ReadLine()!;

            var user = databaseManager.LogUserIn(email, password);
            if (user is null)
            {
                _attempts++;
                Console.Clear();
                Logger.Error("Invalid email or password.");
                Logger.Info($"You have {3 - _attempts} attempt(s) remaining.");
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
        Logger.Info($"You are logged in as '{_user.Email}'.\n");
        Logger.Info("This is an application to manage students.");
        Logger.Info("Admin users have additional options for manipulating student data.");
        Logger.Info("Regular users only have read access.");
        Logger.Info("Please select an option from the menu to start.");
    }

    private static void SayGoodbye()
    {
        Logger.Info("Goodbye! Thank you for using Student Management System.");

    }

    public void DisplayMenu()
    {
        Logger.Log("\n-------- Menu --------");
        Logger.Log("1. View all students");
        Logger.Log("2. Get a student's details");

        if (_user.Role == "admin")
        {
            Logger.Log("3. Add a new student");
            Logger.Log("4. Modify a student's details");
            Logger.Log("5. Delete a student");
            Logger.Log("6. Manage users");
            Logger.Log("7. Exit");
        }
        else
        {
            Logger.Log("3. Exit");
        }

        Logger.Log("-----------------------");
    }

    private void ViewAllStudentsChoice()
    {
        var students = _db.GetAllStudents();
        if (students.Count == 0)
        {
            Logger.Info("No students found.");
        }
        else
        {
            Logger.Log("==== Students ====");
            students.ForEach(student => Logger.Log(student.ToString()));
            Logger.Log("==================");
        }
    }

    private void GetStudentDetailsChoice()
    {
        Logger.Input("Enter the student's ID");
        int id = (int)ReadInputGeneric<int>()!;

        var student = _db.GetStudentById(id);
        if (student is null)
        {
            Logger.Error("Student not found.");
        }
        else
        {
            Logger.Log(student.ToString());
        }
    }

    private void AddStudentChoice()
    {
        Logger.Input("Enter the student's first name");
        string firstName = ReadInput();

        Logger.Input("Enter the student's last name");
        string lastName = ReadInput();

        Logger.Input("Enter the student's date of birth [DD/MM/YYYY]");
        DateOnly dateOfBirth = (DateOnly)ReadInputGeneric<DateOnly>()!;

        Logger.Input("Enter the student's height [in cm]");
        float height = (float)ReadInputGeneric<float>()!;

        _db.AddStudent(firstName, lastName, dateOfBirth, height);
        Logger.Success($"Student {firstName} {lastName} added successfully.");
    }

    private void ModifyStudentChoice()
    {
        Logger.Input("Enter the student's ID");
        int id = (int)ReadInputGeneric<int>()!;

        var student = _db.GetStudentById(id);
        if (student is null)
        {
            Logger.Error("Student not found.");
            return;
        }

        Logger.Input("Enter the student's first name (leave empty to keep the current value)");
        string firstName = ReadInput(true);
        if (firstName == "")
            firstName = student.FirstName;

        Logger.Input("Enter the student's last name (leave empty to keep the current value)");
        string lastName = ReadInput(true);
        if (lastName == "")
            lastName = student.LastName;

        Logger.Input("Enter the student's date of birth [DD/MM/YYYY] (leave empty to keep the current value)");
        DateOnly dateOfBirth = ReadInputGeneric<DateOnly>(true) ?? student.DateOfBirth;

        Logger.Input("Enter the student's height [in cm] (leave empty to keep the current value)");
        float height = ReadInputGeneric<float>(true) ?? student.Height;

        Student newStudent = new(id, firstName, lastName, dateOfBirth, height);

        _db.UpdateStudent(newStudent);
        Logger.Success($"Student {firstName} {lastName} modified successfully.");
    }

    private static string ReadInput(bool acceptEmpty = false)
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (input == null || (input == "" && !acceptEmpty))
            {
                Logger.Error("Invalid input, please try again.");
                Logger.Input("");
            }
            else
            {
                return input == "" ? "" : input.Trim();
            }
        }
    }

    /// <summary>
    /// Reads input from the console and attempts to parse it into the specified type using reflection
    /// to invoke the TryParse method on the type. The type must have a conventional method
    /// signature of: `public static bool TryParse(string input, out T result)`.
    /// Partially generated by ChatGPT.
    /// 
    /// DateOnly is always checked for the format DD/MM/YYYY.
    /// </summary>
    /// <typeparam name="T">The type to attempt to parse the input into</typeparam>
    /// <returns>The input parsed into the type.</returns>
    public static T? ReadInputGeneric<T>(bool acceptEmpty = false) where T : struct // T must be a value type
    {
        var tryParseMethod = typeof(T).GetMethod("TryParse", [typeof(string), typeof(T).MakeByRefType()]);
        if (tryParseMethod is null)
        {
            Logger.Fatal("The type does not have a TryParse method.");
            return null;
        }

        static void WarnInvalid()
        {
            Logger.Error("Invalid input, please try again.");
            Logger.Input("");
        }

        while (true)
        {
            var input = Console.ReadLine();
            if (input is null || (input == "" && !acceptEmpty) || (typeof(T) == typeof(DateOnly) && !IsDateFormatted(input)))
            {
                WarnInvalid();
            }
            else
            {
                var parameters = new object[] { input.Trim(), null! };
                var success = (bool)(tryParseMethod.Invoke(null, parameters) ?? false);
                if (success)
                {
                    return (T)parameters[1];
                }
                else
                {
                    if (acceptEmpty)
                    {
                        return null;
                    }
                    else
                    {
                        WarnInvalid();
                    }
                }
            }
        }
    }

    private static bool IsDateFormatted(string input)
    {
        Regex dateRegex = DateFmtRegex();
        return dateRegex.IsMatch(input);
    }

    /// <summary>
    /// An auto-generated regex for date format DD/MM/YYYY
    /// Regex pattern generated by ChatGPT.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/\d{4}$"
)]
    private static partial Regex DateFmtRegex();
}
