using ConsoleTables;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Controllers;

/// <summary>
/// A user management wizard that allows users to view, update, add, and delete users depending on their role.
/// </summary>
/// 
/// <param name="user">The current user.</param>
/// <param name="db">The database instance.</param>
public class UserController(User user, DatabaseController db) : IRunner
{
    enum UserOptions
    {
        ViewAllUsers = 1,
        UpdateYourDetails = 2,
        UpdateAnotherUserDetails = 3,
        AddNewUser = 4,
        DeleteUser = 5,
        Exit = 6
    }

    public void Run()
    {
        bool quit = false;
        Greet();
        while (!quit)
        {
            DisplayMenu();
            Logger.Input("Enter your choice (1-6)");
            if (!Enum.TryParse<UserOptions>(Console.ReadLine(), out var choice))
            {
                Logger.LogInvalidChoice();
                continue;
            }

            Console.Clear();
            switch (choice)
            {
                case UserOptions.ViewAllUsers:
                    ViewAllUsersChoice();
                    break;
                case UserOptions.UpdateYourDetails:
                    UpdateUserDetails(user.Id);
                    break;
                case UserOptions.UpdateAnotherUserDetails:
                    PerformAdminAction(UpdateAnotherUserChoice);
                    break;
                case UserOptions.AddNewUser:
                    PerformAdminAction(AddUserChoice);
                    break;
                case UserOptions.DeleteUser:
                    PerformAdminAction(DeleteUserChoice);
                    break;
                case UserOptions.Exit:
                    quit = true;
                    break;
                default:
                    Logger.LogInvalidChoice();
                    break;
            }
        }
    }

    /// <summary>
    /// Performs an admin action if the user is an admin.
    /// </summary>
    /// 
    /// <param name="action">The action to perform (function to call)</param>
    private void PerformAdminAction(Action action)
    {
        if (user.Role == "admin")
            action();
        else
            Logger.LogInvalidChoice();
    }

    public void DisplayMenu()
    {
        Logger.Log("\n-------- User Menu --------");
        Logger.Log("1. View all users");
        Logger.Log("2. Update your details");
        if (user.Role == "admin")
        {
            Logger.Log("3. Update another user's details");
            Logger.Log("4. Add a new user");
            Logger.Log("5. Delete a user");

        }
        else
        {
            Logger.Log("3. Update another user's details", ConsoleColor.DarkGray);
            Logger.Log("4. Add a new user", ConsoleColor.DarkGray);
            Logger.Log("5. Delete a user", ConsoleColor.DarkGray);
        }
        Logger.Log("6. Exit");
        Logger.Log("---------------------------");
    }

    public void Greet()
    {
        Console.Clear();
        Logger.Info($"Welcome to the user management wizard, {user.Email}!");
        if (user.Role == "admin")
        {
            Logger.Info("You are an admin user, so have write access to all users.");
        }
        else
        {
            Logger.Log("You are a standard user, so only have write access to your own details.");
        }
    }

    /// <summary>
    /// Displays all users in the system.
    /// </summary>
    private void ViewAllUsersChoice()
    {
        List<User> users = db.GetAllUsers();
        var count = users.Count;
        if (count == 0)
        {
            Logger.Info("There are no users in the system.");
        }
        else
        {
            var table = ConsoleTable.From(users);
            Console.Clear();
            Logger.Log(table.ToString());
        }
    }

    /// <summary>
    /// Gets input from the user to update a specific user's details.
    /// </summary>
    /// 
    /// <param name="id">The user ID.</param>
    private void UpdateUserDetails(int id)
    {
        Logger.Input("Enter the new email address (leave empty to keep the current value)");
        string? email = Input.ReadInput(true, regex: RegexValues.EmailRegex().ToString());
        if (email == "")
            email = null;

        Logger.Input("Enter the new password (leave empty to keep the current value)");
        string? password = Input.ReadInput(true);
        if (password == "")
            password = null;

        if (email is null && password is null)
        {
            Logger.Warning("No values provided, user details not updated.");
            return;
        }

        var updated = db.UpdateUser(id, email, password);
        if (updated)
        {
            Logger.Success("Details updated successfully.");
            if (email is not null)
                user.Email = email;
        }
        else
        {
            Logger.Error("Failed to update details, please try again.");
        }
    }

    /// <summary>
    /// Asks user for the ID of the user to update to then updates the details.
    /// </summary>
    private void UpdateAnotherUserChoice()
    {
        Logger.Input("Enter the ID of the user to update");
        int id = (int)Input.ReadInputGeneric<int>()!;
        UpdateUserDetails(id);
    }

    /// <summary>
    /// Gets input from the user to add a new user to the database.
    /// </summary>
    private void AddUserChoice()
    {
        Logger.Input("Enter the email address of the new user");
        string email = Input.ReadInput(regex: RegexValues.EmailRegex().ToString());

        if (db.UserExists(email))
        {
            Logger.Error("This user already exists.");
            return;
        }

        Logger.Input("Enter the role of the new user [user or admin]");
        string role = Input.ReadInput(regex: RegexValues.RoleRegex().ToString());

        Logger.Input("Enter the password of the new user:");
        string password = Input.ReadInput();

        var added = db.AddUser(email, password, role);
        if (added)
        {
            Logger.Success("User added successfully.");
        }
        else
        {
            Logger.Error("Failed to add user, please try again.");
        }
    }

    /// <summary>
    /// Gets input from the user to delete a user from the database.
    /// </summary>
    private void DeleteUserChoice()
    {
        Logger.Input("Enter the ID of the user to delete");
        int id = (int)Input.ReadInputGeneric<int>()!;
        bool? deleted = db.DeleteUser(id);
        if (deleted is null)
        {
            Logger.Error("Cannot delete the last admin user.");
        }
        else if (deleted == true)
        {
            Logger.Success("User deleted successfully.");
        }
        else
        {
            Logger.Error("User does not exist.");
        }
    }
}