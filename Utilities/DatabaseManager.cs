namespace StudentManagementSystem.Utilities;

using Microsoft.Data.Sqlite;
using StudentManagementSystem.Models;

/// <summary>
/// Provides convenient methods for interacting with the database.
/// </summary>
public class DatabaseManager
{

    /// <summary>
    /// The connection string used for creating connections to the database.
    /// </summary>
    private readonly string _connectionString;

    public DatabaseManager(string dbPath)
    {
        if (!dbPath.EndsWith(".sqlite") || !dbPath.EndsWith(".db"))
            throw new ArgumentException("The database path must end with `.sqlite` or `.db`.");

        _connectionString = $"Data Source={dbPath}";
    }

    /// <summary>
    /// Gets all the students in the database.
    /// </summary>
    /// <returns>A list of students.</returns>
    public List<Student> GetAllStudents()
    {
        List<Student> students = [];

        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        SqliteCommand command = conn.CreateCommand();
        command.CommandText = @"SELECT * FROM Students";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            DateTime dateOfBirth = reader.GetDateTime(3);
            float height = reader.GetFloat(4);

            Student student = new(id)
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Height = height
            };

            students.Add(student);
        }

        reader.Close();
        conn.Close();

        return students;
    }
}
