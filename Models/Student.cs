namespace StudentManagementSystem.Models;

/// <summary>
/// Represents a student.
/// </summary>
public class Student(int id, string firstName, string lastName, DateOnly dateOfBirth, float height)
{
    public int Id { get; } = id;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public DateOnly DateOfBirth { get; set; } = dateOfBirth;
    public float Height { get; set; } = height;

    override public string ToString()
    {
        return $"[{Id}] {FirstName} {LastName} - Born {DateOfBirth} - {Height}cm tall.";
    }
}