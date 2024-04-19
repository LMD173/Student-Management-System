using System.Globalization;
using System.Text;
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
    /// The database file path used for creating connections to the database.
    /// </summary>
    private readonly string _databasePath;

    /// <summary>
    /// Creates a new instance of the DatabaseController.
    /// </summary>
    /// 
    /// <param name="dbPath">The path to the database file ending in .sqlite, .db or .sqlite3.</param>
    /// 
    /// <exception cref="ArgumentException">Thrown when the database path is invalid.</exception>
    public DatabaseController(string dbPath)
    {
        string[] validDbExtensions = [".sqlite", ".db", ".sqlite3"];

        if (!validDbExtensions.Contains(Path.GetExtension(dbPath)))
            throw new ArgumentException("The database path is invalid; must end with `.sqlite`, `.sqlite3` or `.db`.");

        _databasePath = $"Data Source={dbPath}";
    }

    /// <summary>
    /// Gets all the students in the database.
    /// </summary>
    /// 
    /// <returns>A list of students.</returns>
    public List<Student> GetAllStudents()
    {
        List<Student> students = [];

        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM Student";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            float height = reader.GetFloat(3);
            DateOnly dateOfBirth = DateOnly.ParseExact(reader.GetString(4), "yyyy-mm-dd");
            string postcode = reader.GetString(5);
            string addressLine = reader.GetString(6);
            string contactPhone = reader.GetString(7);
            string contactEmail = reader.GetString(8);

            Student student = new(id, firstName, lastName, dateOfBirth, height, postcode, addressLine, contactPhone, contactEmail);
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
    /// <param name="student">The student object to add.</param>
    /// 
    /// <returns>Whether the student was added.</returns>
    public bool AddStudent(Student student)
    {
        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Student (first_name, last_name, height, date_of_birth, postcode, address_line, contact_phone_number, contact_email) VALUES (@firstName, @lastName, @height, @dateOfBirth, @postcode, @addressLine, @contactPhone, @contactEmail)";
        command.Parameters.AddWithValue("@firstName", student.FirstName);
        command.Parameters.AddWithValue("@lastName", student.LastName);
        command.Parameters.AddWithValue("@height", student.Height);
        command.Parameters.AddWithValue("@dateOfBirth", student.DateOfBirth.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@postcode", student.Postcode);
        command.Parameters.AddWithValue("@addressLine", student.AddressLine);
        command.Parameters.AddWithValue("@contactPhone", student.ContactPhone);
        command.Parameters.AddWithValue("@contactEmail", student.ContactEmail);


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
        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM Student WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        Logger.Log("");
        Student? student = null;
        if (reader.Read())
        {
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            float height = reader.GetFloat(3);
            DateOnly dateOfBirth = DateOnly.ParseExact(reader.GetString(4), "yyyy-mm-dd");
            string postcode = reader.GetString(5);
            string addressLine = reader.GetString(6);
            string contactPhone = reader.GetString(7);
            string contactEmail = reader.GetString(8);

            student = new Student(id, firstName, lastName, dateOfBirth, height, postcode, addressLine, contactPhone, contactEmail);
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
        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"UPDATE Student SET first_name = @firstName, last_name = @lastName, height = @height, date_of_birth = @dateOfBirth, postcode = @postcode, address_line = @addressLine, contact_phone_number = @contactPhone, contact_email = @contactEmail WHERE id = @id";
        command.Parameters.AddWithValue("@id", student.Id);
        command.Parameters.AddWithValue("@firstName", student.FirstName);
        command.Parameters.AddWithValue("@lastName", student.LastName);
        command.Parameters.AddWithValue("@height", student.Height);
        command.Parameters.AddWithValue("@dateOfBirth", student.DateOfBirth.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@postcode", student.Postcode);
        command.Parameters.AddWithValue("@addressLine", student.AddressLine);
        command.Parameters.AddWithValue("@contactPhone", student.ContactPhone);
        command.Parameters.AddWithValue("@contactEmail", student.ContactEmail);

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
        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM User WHERE email = @email";
        command.Parameters.AddWithValue("@email", email);

        using var reader = command.ExecuteReader();
        User? user = null;
        if (reader.Read())
        {
            string pw = reader.GetString(2);
            string salt = reader.GetString(4);
            if (Cryptography.Verify(salt, pw, password))
            {
                user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(3));
            }
        }

        reader.Close();
        connection.Close();
        return user;
    }

    /// <summary>
    /// Checks if a user exists in the database given their email.
    /// </summary>
    /// 
    /// <param name="email">The user's email.</param>
    /// 
    /// <returns>Whether the user exists.</returns>
    public bool UserExists(string email)
    {
        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"SELECT COUNT(*) FROM User WHERE email = @email";
        command.Parameters.AddWithValue("@email", email);

        var result = command.ExecuteScalar();
        connection.Close();
        return (long)result! > 0;
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
        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM Student WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);

        bool result = command.ExecuteNonQuery() > 0;
        connection.Close();
        return result;
    }


    /// <summary>
    /// Searches for students by name and/or postcode as filters.
    /// </summary>
    /// 
    /// <param name="firstName">The first name to search for.</param>
    /// <param name="lastName">The last name to search for.</param>
    /// <param name="postcode">The postcode to search for.</param>
    /// 
    /// <returns>A list of students matching the search criteria.</returns>
    public List<Student> SearchForStudentsByNameOrPostcode(string? firstName, string? lastName, string? postcode)
    {
        if (firstName is null && lastName is null && postcode is null)
            return [];

        List<Student> students = [];

        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        using var command = connection.CreateCommand();

        // 1=1 so we can append AND to the query 
        StringBuilder query = new("SELECT * FROM Student WHERE 1=1");

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            query.Append(" AND LOWER(first_name) = @firstname");
            command.Parameters.AddWithValue("@firstname", $"{firstName.ToLower()}");
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            query.Append(" AND LOWER(last_name) = @lastname");
            command.Parameters.AddWithValue("@lastname", $"{lastName.ToLower()}");
        }

        if (!string.IsNullOrWhiteSpace(postcode))
        {
            query.Append(" AND LOWER(postcode) LIKE @postcode");
            // Using wildcard character here (%) to allow for partial matches 
            command.Parameters.AddWithValue("@postcode", $"%{postcode.ToLower()}%");
        }

        command.CommandText = query.ToString();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string fName = reader.GetString(1);
            string lName = reader.GetString(2);
            float height = reader.GetFloat(3);
            DateOnly dateOfBirth = DateOnly.ParseExact(reader.GetString(4), "yyyy-mm-dd");
            string studentPostcode = reader.GetString(5);
            string addressLine = reader.GetString(6);
            string contactPhone = reader.GetString(7);
            string contactEmail = reader.GetString(8);

            Student student = new(id, fName, lName, dateOfBirth, height, studentPostcode, addressLine, contactPhone, contactEmail);
            students.Add(student);
        }

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

        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
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
    /// <param name="newEmail">The user's new email</param>
    /// <param name="newPassword">The user's new unhashed password</param>
    /// 
    /// <returns>Whether the user was updated.</returns>
    public bool UpdateUser(int id, string? newEmail, string? newPassword)
    {
        if (newEmail is null && newPassword is null)
            return false;

        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        List<string> commands = [];

        if (newEmail is not null)
        {
            commands.Add("email = @newEmail");
            command.Parameters.AddWithValue("@newEmail", newEmail);
        }

        if (newPassword is not null)
        {
            string salt = Cryptography.GenerateSalt();

            commands.Add("password = @password");
            commands.Add("salt = @salt");

            command.Parameters.AddWithValue("@password", Cryptography.Hash(newPassword, salt));
            command.Parameters.AddWithValue("@salt", salt);
        }

        command.Parameters.AddWithValue("@id", id);
        command.CommandText = $"UPDATE User SET {string.Join(", ", commands)} WHERE id = @id";

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
        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO User (email, password, role, salt) VALUES (@email, @password, @role, @salt)";
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@password", Cryptography.Hash(password, email));
        command.Parameters.AddWithValue("@role", role);
        string salt = Cryptography.GenerateSalt();
        command.Parameters.AddWithValue("@salt", salt);

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
    /// <returns>Whether the user was removed or null if they were the last admin user.</returns>
    public bool? DeleteUser(int id)
    {
        using var connection = new SqliteConnection(_databasePath);
        connection.Open();

        using var roleCommand = connection.CreateCommand();
        roleCommand.CommandText = @"SELECT role FROM User where id = @id";
        roleCommand.Parameters.AddWithValue("@id", id);

        using var reader = roleCommand.ExecuteReader();
        string role = "";
        if (reader.Read())
        {
            role = reader.GetString(0);
            reader.Close();
        }

        if (role == "admin")
        {
            using var countCommand = connection.CreateCommand();
            countCommand.CommandText = @"SELECT COUNT(*) FROM User WHERE role = 'admin'";
            var count = countCommand.ExecuteScalar();
            if ((long)count! <= 1)
            {
                connection.Close();
                return null;
            }
        }

        using var command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM User WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);

        bool result = command.ExecuteNonQuery() > 0;
        connection.Close();
        return result;
    }
}
