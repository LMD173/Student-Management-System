using ConsoleTables;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Controllers;

/// <summary>
/// A student management wizard that allows users to view, update, add, and delete students depending on their role.
/// </summary>
/// <param name="db">the database instance.</param>
public class StudentManager(DatabaseManager db) : IRunner
{
    private User _user = null!; // null-forgiving operator because _user will never be null when it is used.
    private int _attempts = 0;

    /// <summary>
    /// Initialises the student manager. Fatally stops execution if the maximum number of login attempts is exceeded.
    /// </summary>
    public void Initialise()
    {
        if (!TryLogin(db))
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
            string choice = Input.ReadInput();
            switch (choice)
            {
                case "1":
                    Console.Clear();
                    ViewAllStudentsChoice();
                    break;
                case "2":
                    Console.Clear();
                    SearchStudentByIdChoice();
                    break;
                case "3":
                    Console.Clear();
                    SearchStudentByNameOrPostcode();
                    break;
                case "4":
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
                case "5":
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
                case "6":
                    Console.Clear();
                    if (_user.Role == "admin")
                    {
                        DeleteStudentChoice();
                    }
                    else
                    {
                        Logger.Error("Invalid choice, please try again.");
                    }
                    break;
                case "7":
                    if (_user.Role == "admin")
                    {
                        UserManager userManager = new(_user, db);
                        userManager.Run();
                    }
                    else
                    {
                        Logger.Error("Invalid choice, please try again.");
                    }
                    break;
                case "8":
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
        Logger.Info("Before you begin, please login.");
        while (_attempts < 3)
        {
            Logger.Input("Enter your email");
            string email = Console.ReadLine()!.Trim();
            Logger.Input("Enter your password");
            string password = Console.ReadLine()!.Trim();

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
        Logger.Log("2. Search for a student by ID");
        Logger.Log("3. Search for a student by name or postcode");

        if (_user.Role == "admin")
        {
            Logger.Log("4. Add a new student");
            Logger.Log("5. Modify a student's details");
            Logger.Log("6. Delete a student");
            Logger.Log("7. Manage users");
            Logger.Log("8. Exit");
        }
        else
        {
            Logger.Log("4. Exit");
        }

        Logger.Log("-----------------------");
    }

    private void ViewAllStudentsChoice()
    {
        var students = db.GetAllStudents();
        var count = students.Count;
        if (count == 0)
        {
            Logger.Info("No students found.");
        }
        else
        {
            var table = ConsoleTable.From(students);
            Console.Clear();
            Logger.Log(table.ToString());
        }
    }

    private void SearchStudentByIdChoice()
    {
        Logger.Log("==== Get a student's details ====");
        Logger.Input("Enter the student's ID");
        int id = (int)Input.ReadInputGeneric<int>()!;

        var student = db.GetStudentById(id);
        if (student is null)
        {
            Logger.Error("Student not found.");
        }
        else
        {
            Logger.Log(student.ToString());
        }
    }

    private void SearchStudentByNameOrPostcode()
    {
        Logger.Log("==== Search for a student by name or postcode ====");
        Logger.Input("Enter the student's first name (leave empty to skip)");
        string firstName = Input.ReadInput(true);

        Logger.Input("Enter the student's last name (leave empty to skip)");
        string lastName = Input.ReadInput(true);

        Logger.Input("Enter the student's postcode (leave empty to skip)");
        string postcode = Input.ReadInput(true);

        var students = db.SearchForStudentsByNameOrPostcode(string.Concat(firstName, lastName), postcode);
        var count = students.Count;
        if (count == 0)
        {
            Logger.Info("No students found with the provided criteria.");
        }
        else
        {
            var table = ConsoleTable.From(students);
            Console.Clear();
            Logger.Log(table.ToString());
        }
    }

    private void AddStudentChoice()
    {
        Logger.Log("==== Add a new student ====");
        Logger.Input("Enter the student's first name");
        string firstName = Input.ReadInput();

        Logger.Input("Enter the student's last name");
        string lastName = Input.ReadInput();

        Logger.Input("Enter the student's date of birth [DD/MM/YYYY]");
        DateOnly dateOfBirth = (DateOnly)Input.ReadInputGeneric<DateOnly>(regex: RegexValues.DateFmtRegex().ToString())!;

        Logger.Input("Enter the student's height [in cm]");
        float height = (float)Input.ReadInputGeneric<float>()!;

        Logger.Input("Enter the student's postcode");
        string postcode = Input.ReadInput(regex: RegexValues.PostcodeRegex().ToString());

        db.AddStudent(firstName, lastName, dateOfBirth, height, postcode);
        Logger.Success($"Student {firstName} {lastName} added successfully.");
    }

    private void ModifyStudentChoice()
    {
        Logger.Log("==== Modify a student's details ====");
        Logger.Input("Enter the student's ID");
        int id = (int)Input.ReadInputGeneric<int>()!;

        var student = db.GetStudentById(id);
        if (student is null)
        {
            Logger.Error("Student not found.");
            return;
        }

        Logger.Input("Enter the student's first name (leave empty to keep the current value)");
        string firstName = Input.ReadInput(true);
        if (firstName == "")
            firstName = student.FirstName;

        Logger.Input("Enter the student's last name (leave empty to keep the current value)");
        string lastName = Input.ReadInput(true);
        if (lastName == "")
            lastName = student.LastName;

        Logger.Input("Enter the student's date of birth [DD/MM/YYYY] (leave empty to keep the current value)");
        DateOnly dateOfBirth = Input.ReadInputGeneric<DateOnly>(true, RegexValues.DateFmtRegex().ToString()) ?? student.DateOfBirth;

        Logger.Input("Enter the student's height [in cm] (leave empty to keep the current value)");
        float height = Input.ReadInputGeneric<float>(true) ?? student.Height;

        Logger.Input("Enter the student's postcode (leave empty to keep the current value)");
        string postcode = Input.ReadInput(true, regex: RegexValues.PostcodeRegex().ToString());
        if (postcode == "")
            postcode = student.Postcode;

        Student newStudent = new(id, firstName, lastName, dateOfBirth, height, postcode);

        var updated = db.UpdateStudent(newStudent);
        if (!updated)
        {
            Logger.Error("Failed to modify student.");
            return;
        }
        else
        {
            Logger.Success($"Student {firstName} {lastName} modified successfully.");
        }
    }

    private void DeleteStudentChoice()
    {
        Logger.Log("==== Delete a student ====");
        Logger.Input("Enter the student's ID");
        int id = (int)Input.ReadInputGeneric<int>()!;

        var student = db.GetStudentById(id);
        if (student is null)
        {
            Logger.Error("Student not found.");
            return;
        }

        var deleted = db.DeleteStudent(id);
        if (!deleted)
        {
            Logger.Error("Failed to delete student.");
            return;
        }
        else
        {
            Logger.Success($"Student {student.FirstName} {student.LastName} deleted successfully.");
        }
    }
}
