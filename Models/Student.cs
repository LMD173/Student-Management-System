namespace StudentManagementSystem.Models;

/// <summary>
/// Represents a student.
/// </summary>
public class Student
{
    public int Id { get; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public float Height { get; set; }

    public Student(int id)
    {
        Id = id;
    }
}