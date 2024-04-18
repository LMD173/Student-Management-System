using ConsoleTables;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Controllers;

/// <summary>
/// A student management wizard that allows users to view, update, add, and delete students depending on their role.
/// </summary>
/// <param name="db">The database instance.</param>
public class StudentController(User user, DatabaseController db) : IRunner
{
    enum StudentControllerMenuOptions
    {
        ViewAllStudents = 1,
        SearchStudentById = 2,
        SearchStudentByNameOrPostcode = 3,
        AddStudent = 4,
        ModifyStudent = 5,
        DeleteStudent = 6,
        ManageUsers = 7,
        Exit = 8
    }

    public void Run()
    {
        bool quit = false;
        Greet();
        while (!quit)
        {
            DisplayMenu();
            Logger.Input("Enter your choice (1-8)");
            if (!Enum.TryParse<StudentControllerMenuOptions>(Input.ReadInput().ToLower(), out var choice))
            {
                Logger.LogInvalidChoice();
                continue;
            }

            Console.Clear();
            switch (choice)
            {
                case StudentControllerMenuOptions.ViewAllStudents:
                    ViewAllStudentsChoice();
                    break;
                case StudentControllerMenuOptions.SearchStudentById:
                    SearchStudentByIdChoice();
                    break;
                case StudentControllerMenuOptions.SearchStudentByNameOrPostcode:
                    SearchStudentByNameOrPostcodeChoice();
                    break;
                case StudentControllerMenuOptions.AddStudent:
                    PerformAdminAction(AddStudentChoice);
                    break;
                case StudentControllerMenuOptions.ModifyStudent:
                    PerformAdminAction(ModifyStudentChoice);
                    break;
                case StudentControllerMenuOptions.DeleteStudent:
                    PerformAdminAction(DeleteStudentChoice);
                    break;
                case StudentControllerMenuOptions.ManageUsers:
                    new UserController(user, db).Run();
                    break;
                case StudentControllerMenuOptions.Exit:
                    quit = true;
                    break;
                default:
                    Logger.LogInvalidChoice();
                    break;
            }
        }
        SayGoodbye();
    }

    /// <summary>
    /// Performs an admin action if the user is an admin.
    /// </summary>
    /// <param name="action">The action to perform (function to call)</param>
    private void PerformAdminAction(Action action)
    {
        if (user.Role == "admin")
            action();
        else
            Logger.LogInvalidChoice();
    }

    public void Greet()
    {
        Console.Clear();
        Logger.Info("Hello! Welcome to Student Management System.");
        Logger.Info($"You are logged in as '{user.Email}'.\n");
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
        Logger.Log("\n-------- Student Menu --------");
        Logger.Log("1. View all students");
        Logger.Log("2. Search for a student by ID");
        Logger.Log("3. Search for a student by name or postcode");

        if (user.Role == "admin")
        {
            Logger.Log("4. Add a new student");
            Logger.Log("5. Modify a student's details");
            Logger.Log("6. Delete a student");
            Logger.Log("7. Manage users");
        }
        else
        {
            Logger.Log("4. Add a new student", ConsoleColor.DarkGray);
            Logger.Log("5. Modify a student's details", ConsoleColor.DarkGray);
            Logger.Log("6. Delete a student", ConsoleColor.DarkGray);
            Logger.Log("7. Manage your account");
        }

        Logger.Log("8. Exit");
        Logger.Log("-------------------------------");
    }

    /// <summary>
    /// Displays all students in the system.
    /// </summary>
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
            var table = TableFromStudents(students);
            Console.Clear();
            Logger.Log(table.ToString());
        }
    }

    /// <summary>
    /// Creates a console table from a list of students.
    /// </summary>
    /// 
    /// <param name="students">The list of students to display.</param>
    /// 
    /// <returns>The console table.</returns>
    private static ConsoleTable TableFromStudents(List<Student> students)
    {
        var table = new ConsoleTable("ID", "First", "Last", "Date of Birth", "Email", "Phone", "Address", "Postcode");
        foreach (var student in students)
        {
            table.AddRow(student.Id, student.FirstName, student.LastName, student.DateOfBirth.ToString("yyyy-MM-dd"), student.ContactEmail, student.ContactPhone, student.AddressLine, student.Postcode);
        }
        return table;
    }

    /// <summary>
    /// Asks the user for a student ID to then displays the student's details.
    /// </summary>
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

    /// <summary>
    /// Gets user input to search for a student by name or postcode.
    /// </summary>
    private void SearchStudentByNameOrPostcodeChoice()
    {
        Logger.Log("==== Search for a student by name or postcode as filters ====");
        Logger.Input("Enter the student's first name (leave empty to skip)");
        string firstName = Input.ReadInput(true);

        Logger.Input("Enter the student's last name (leave empty to skip)");
        string lastName = Input.ReadInput(true);

        Logger.Input("Enter the student's postcode (leave empty to skip)");
        string postcode = Input.ReadInput(true);

        var students = db.SearchForStudentsByNameOrPostcode(firstName, lastName, postcode);
        var count = students.Count;
        if (count == 0)
        {
            Logger.Info("No students found with the provided criteria.");
        }
        else
        {
            var table = TableFromStudents(students);
            Console.Clear();
            Logger.Log(table.ToString());
        }
    }

    /// <summary>
    /// Gets user input to add a new student.
    /// </summary>
    private void AddStudentChoice()
    {
        Logger.Log("==== Add a new student ====");
        Logger.Input("Enter the student's first name");
        string firstName = Input.ReadInput();

        Logger.Input("Enter the student's last name");
        string lastName = Input.ReadInput();

        Logger.Input("Enter the student's date of birth [yyyy-mm-dd]");
        DateOnly dateOfBirth = (DateOnly)Input.ReadInputGeneric<DateOnly>(regex: RegexValues.DateFmtRegex().ToString())!;

        Logger.Input("Enter the student's height [in cm]");
        float height = (float)Input.ReadInputGeneric<float>()!;

        Logger.Input("Enter the student's postcode");
        string postcode = Input.ReadInput(regex: RegexValues.PostcodeRegex().ToString());

        Logger.Input("Enter the student's address line");
        string addressLine = Input.ReadInput();

        Logger.Input("Enter the student's contact phone");
        string contactPhone = Input.ReadInput(); // not using regex for phone number because there are too many possible variations

        Logger.Input("Enter the student's contact email");
        string contactEmail = Input.ReadInput(regex: RegexValues.EmailRegex().ToString());

        db.AddStudent(new Student(0, firstName, lastName, dateOfBirth, height, postcode, addressLine, contactPhone, contactEmail));
        Logger.Success($"Student {firstName} {lastName} added successfully.");
    }

    /// <summary>
    /// Gets user input to modify a student's details.
    /// </summary>
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

        Logger.Input("Enter the student's date of birth [yyyy-mm-dd] (leave empty to keep the current value)");
        DateOnly dateOfBirth = Input.ReadInputGeneric<DateOnly>(true, RegexValues.DateFmtRegex().ToString()) ?? student.DateOfBirth;

        Logger.Input("Enter the student's height [in cm] (leave empty to keep the current value)");
        float height = Input.ReadInputGeneric<float>(true) ?? student.Height;

        Logger.Input("Enter the student's postcode (leave empty to keep the current value)");
        string postcode = Input.ReadInput(true, regex: RegexValues.PostcodeRegex().ToString());
        if (postcode == "")
            postcode = student.Postcode;

        Logger.Input("Enter the student's address line (leave empty to keep the current value)");
        string addressLine = Input.ReadInput(true);
        if (addressLine == "")
            addressLine = student.AddressLine;

        Logger.Input("Enter the student's contact phone (leave empty to keep the current value)");
        string contactPhone = Input.ReadInput(true);
        if (contactPhone == "")
            contactPhone = student.ContactPhone;

        Logger.Input("Enter the student's contact email (leave empty to keep the current value)");
        string contactEmail = Input.ReadInput(true, regex: RegexValues.EmailRegex().ToString());
        if (contactEmail == "")
            contactEmail = student.ContactEmail;

        Student newStudent = new(id, firstName, lastName, dateOfBirth, height, postcode, addressLine, contactPhone, contactEmail);

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

    /// <summary>
    /// Gets user input to delete a student.
    /// </summary>
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
