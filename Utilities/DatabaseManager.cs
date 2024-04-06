namespace StudentManagementSystem.Utilities;

using Microsoft.Data.Sqlite;
using StudentManagementSystem.Models;

/// <summary>
/// Provides methods for interacting with the database.
/// </summary>
public class DatabaseManager
{

    /// <summary>
    /// The connection string used for creating connections to the database.
    /// </summary>
    private readonly string _connectionString;

    public DatabaseManager(string dbPath)
    {
        string[] validDbExtensions = [".sqlite", ".db", ".sqlite3"];

        if (!validDbExtensions.Contains(Path.GetExtension(dbPath)))
            throw new ArgumentException("The database path is invalid; must end with `.sqlite`, `.sqlite3` or `.db`.");

        _connectionString = $"Data Source={dbPath}";
    }

    /// <summary>
    /// Gets all the students in the database.
    /// </summary>
    /// <returns>A list of students.</returns>
    public List<Student> GetAllStudents()
    {
        List<Student> students = [];

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM Student";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            float height = reader.GetFloat(3);

            if (!DateOnly.TryParse(reader.GetString(4), out DateOnly dateOfBirth))
            {
                Logger.Error($"Failed to parse date of birth for student with ID {id}");
                continue;
            }

            Student student = new(id, firstName, lastName, dateOfBirth, height);
            students.Add(student);
        }

        reader.Close();
        connection.Close();

        return students;
    }

    /// <summary>
    /// Adds a student to the database.
    /// </summary>
    /// <param name="firstName">The student's first name.</param>
    /// <param name="lastName">The student's last name.</param>
    /// <param name="dateOfBirth">The student's date of birth.</param>
    /// <param name="height">The student's height.</param>
    /// <returns>Whether the student was successfully added.</returns>
    public bool AddStudent(string firstName, string lastName, DateOnly dateOfBirth, float height)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Student (first_name, last_name, height, date_of_birth) VALUES (@firstName, @lastName, @height, @dateOfBirth)";
        command.Parameters.AddWithValue("@firstName", firstName);
        command.Parameters.AddWithValue("@lastName", lastName);
        command.Parameters.AddWithValue("@height", height);
        command.Parameters.AddWithValue("@dateOfBirth", dateOfBirth.ToString());

        return command.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// Logs a user into the system. If the user exists, returns a User object with the user's details.
    /// </summary>
    /// <param name="email">the user's email.</param>
    /// <param name="password">the user's unhashed password.</param>
    /// <param name="user">the User object to set.</param>
    /// <returns>A User object if the user is valid.</returns>
    public User? LogUserIn(string email, string password)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"SELECT id, email, role FROM User WHERE email = @email AND password = @password";
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@password", Cryptography.Hash(password));

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
        }

        return null;
    }

}
