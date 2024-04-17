namespace StudentManagementSystem.Models;

/// <summary>
/// Represents a menu class that can be run (provide actions for a user to take).
/// </summary>
public interface IRunner
{
    /// <summary>
    /// Runs the main flow of the class. Typically starts execution of 
    /// different functions based on user input.
    /// </summary>
    void Run();

    /// <summary>
    /// Displays the menu for the class.
    /// </summary>
    void DisplayMenu();

    /// <summary>
    /// Greets the user.
    /// </summary>
    void Greet();
}