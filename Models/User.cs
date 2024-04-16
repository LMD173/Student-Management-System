namespace StudentManagementSystem.Models;

/// <summary>
/// Represents a User.
/// </summary>
public class User(int id, string email, string role)
{
    public int Id { get; } = id;
    public string Email { get; set; } = email;
    public string Role { get; set; } = role;


    override public string ToString()
    {
        return $"{Email} ({Id}): {Email} ({Role})";
    }
}
