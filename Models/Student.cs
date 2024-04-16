namespace StudentManagementSystem.Models;

/// <summary>
/// Represents a student.
/// </summary>
public class Student(int id, string firstName, string lastName, DateOnly dateOfBirth, float height, string postcode)
{
    public int Id { get; } = id;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public DateOnly DateOfBirth { get; set; } = dateOfBirth;
    public float Height { get; set; } = height;

    public string Postcode { get; set; } = postcode;

    override public string ToString()
    {
        return $"[{Id}] {FirstName} {LastName} - Born {DateOfBirth} - {Height}cm tall - {Postcode}";
    }
}