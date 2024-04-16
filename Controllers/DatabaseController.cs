using Microsoft.Data.Sqlite;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Controllers;

/// <summary>
/// Provides methods for interacting with the database.
/// </summary>
public class DatabaseController
{

    /// <summary>
    /// The connection string used for creating connections to the database.
    /// </summary>
    private readonly string _connectionString;

    public DatabaseController(string dbPath)
    {
        string[] validDbExtensions = [".sqlite", ".db", ".sqlite3"];

        if (!validDbExtensions.Contains(Path.GetExtension(dbPath)))
            throw new ArgumentException("The database path is invalid; must end with `.sqlite`, `.sqlite3` or `.db`.");

        _connectionString = $"Data Source={dbPath}";
    }

    /// <summary>
    /// Gets all the students in the database.
    /// </summary>
    /// 
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
            DateOnly dateOfBirth = DateOnly.Parse(reader.GetString(4));
            string postcode = reader.GetString(5);

            Student student = new(id, firstName, lastName, dateOfBirth, height, postcode);
            students.Add(student);
        }

        reader.Close();
        connection.Close();

        return students;
    }

    /// <summary>
    /// Adds a student to the database.
    /// </summary>
    /// 
    /// <param name="firstName">The student's first name.</param>
    /// <param name="lastName">The student's last name.</param>
    /// <param name="dateOfBirth">The student's date of birth.</param>
    /// <param name="height">The student's height.</param>
    /// <param name="postcode">The student's postcode.</param>
    /// 
    /// <returns>Whether the student was added.</returns>
    public bool AddStudent(string firstName, string lastName, DateOnly dateOfBirth, float height, string postcode)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Student (first_name, last_name, height, date_of_birth, postcode) VALUES (@firstName, @lastName, @height, @dateOfBirth, @postcode)";
        command.Parameters.AddWithValue("@firstName", firstName);
        command.Parameters.AddWithValue("@lastName", lastName);
        command.Parameters.AddWithValue("@height", height);
        command.Parameters.AddWithValue("@dateOfBirth", dateOfBirth.ToString());
        command.Parameters.AddWithValue("@postcode", postcode);

        bool result = command.ExecuteNonQuery() > 0;
        connection.Close();
        return result;
    }

    /// <summary>
    /// Gets a student by their ID.
    /// </summary>
    /// 
    /// <param name="id">The student's id.</param>
    /// 
    /// <returns>The student, or null if not found.</returns>
    public Student? GetStudentById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM Student WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        Student? student = null;
        if (reader.Read())
        {
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            float height = reader.GetFloat(3);
            DateOnly dateOfBirth = DateOnly.Parse(reader.GetString(4));
            string postcode = reader.GetString(5);

            student = new Student(id, firstName, lastName, dateOfBirth, height, postcode);
        }

        reader.Close();
        connection.Close();
        return student;
    }

    /// <summary>
    /// Updates a student's details in the database.
    /// </summary>
    /// 
    /// <param name="student">The student object with the details to change.</param>
    /// 
    /// <returns>Whether the student was updated.</returns>
    public bool UpdateStudent(Student student)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"UPDATE Student SET first_name = @firstName, last_name = @lastName, height = @height, date_of_birth = @dateOfBirth, postcode = @postcode WHERE id = @id";
        command.Parameters.AddWithValue("@id", student.Id);
        command.Parameters.AddWithValue("@firstName", student.FirstName);
        command.Parameters.AddWithValue("@lastName", student.LastName);
        command.Parameters.AddWithValue("@height", student.Height);
        command.Parameters.AddWithValue("@dateOfBirth", student.DateOfBirth.ToString());
        command.Parameters.AddWithValue("@postcode", student.Postcode);

        bool result = command.ExecuteNonQuery() > 0;
        connection.Close();
        return result;
    }

    /// <summary>
    /// Gets a user by their email and password.
    /// </summary>
    /// 
    /// <param name="email">The user's email.</param>
    /// <param name="password">The user's unhashed password.</param>
    /// 
    /// <returns>A User object if the user is valid.</returns>
    public User? GetUser(string email, string password)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"SELECT id, email, role FROM User WHERE email = @email AND password = @password";
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@password", Cryptography.Hash(password));

        using var reader = command.ExecuteReader();
        User? user = null;
        if (reader.Read())
        {
            user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
        }

        reader.Close();
        connection.Close();
        return user;
    }

    /// <summary>
    /// Deletes a student from the database.
    /// </summary>
    /// 
    /// <param name="id">The student's id.</param>
    /// 
    /// <returns>Whether the student was removed.</returns>
    public bool DeleteStudent(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM Student WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);

        bool result = command.ExecuteNonQuery() > 0;
        connection.Close();
        return result;
    }


    /// <summary>
    /// Searches for students by name and/or postcode.
    /// </summary>
    /// 
    /// <param name="name">The name to search for.</param>
    /// <param name="postcode">The postcode to search for.</param>
    /// 
    /// <returns>A list of students matching the search criteria.</returns>
    public List<Student> SearchForStudentsByNameOrPostcode(string? name, string? postcode)
    {
        List<Student> students = [];

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM Student WHERE first_name LIKE @name OR last_name LIKE @name";
        command.Parameters.AddWithValue("@name", $"%{name}%"); // in SQLite, % is a wildcard character which, in this case, matches any characters either side of the name
        command.Parameters.AddWithValue("@postcode", postcode);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            float height = reader.GetFloat(3);
            DateOnly dateOfBirth = DateOnly.Parse(reader.GetString(4));
            string studentPostcode = reader.GetString(5);

            Student student = new(id, firstName, lastName, dateOfBirth, height, studentPostcode);
            students.Add(student);
        }

        reader.Close();
        connection.Close();

        return students;
    }

    /// <summary>
    /// Gets all users in the database.
    /// </summary>
    /// 
    /// <returns>A list of users.</returns>
    public List<User> GetAllUsers()
    {
        List<User> users = [];

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM User";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string email = reader.GetString(1);
            string role = reader.GetString(3);

            User user = new(id, email, role);
            users.Add(user);
        }

        reader.Close();
        connection.Close();

        return users;
    }

    /// <summary>
    /// Updates a user's details in the database.
    /// </summary>
    /// 
    /// <param name="id">The user's id</param>
    /// <param name="email">The user's (new) email</param>
    /// <param name="password">The user's (new) unhashed password</param>
    /// 
    /// <returns>Whether the user was updated.</returns>
    public bool UpdateUser(int id, string email, string? password)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        if (password is null)
        {
            command.CommandText = @"UPDATE User SET email = @email WHERE id = @id";
        }
        else
        {
            command.CommandText = @"UPDATE User SET email = @email, password = @password WHERE id = @id";
            command.Parameters.AddWithValue("@password", password is null ? null : Cryptography.Hash(password));
        }
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@email", email);

        bool result = command.ExecuteNonQuery() > 0;
        connection.Close();
        return result;
    }

    /// <summary>
    /// Adds a user to the database.
    /// </summary>
    /// 
    /// <param name="email">The user's email.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="role">The user's role.</param>
    /// 
    /// <returns>Whether the user was added.</returns>
    public bool AddUser(string email, string password, string role)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO User (email, password, role) VALUES (@email, @password, @role)";
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@password", Cryptography.Hash(password));
        command.Parameters.AddWithValue("@role", role);

        bool result = command.ExecuteNonQuery() > 0;
        connection.Close();
        return result;
    }

    /// <summary>
    /// Deletes a user from the database.
    /// </summary>
    /// 
    /// <param name="id">The user's id.</param>
    /// 
    /// <returns>Whether the user was removed.</returns>
    public bool DeleteUser(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM User WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);

        bool result = command.ExecuteNonQuery() > 0;
        connection.Close();
        return result;
    }
}
