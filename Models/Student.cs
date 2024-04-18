using System.Globalization;

namespace StudentManagementSystem.Models;

/// <summary>
/// Represents a student.
/// </summary>
public class Student(int id, string firstName, string lastName, DateOnly dateOfBirth, float height, string postcode, string addressLine, string contactPhone, string contactEmail)
{
    public int Id { get; } = id;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public DateOnly DateOfBirth { get; set; } = dateOfBirth;
    public float Height { get; set; } = height;
    public string AddressLine { get; set; } = addressLine;
    public string ContactPhone { get; set; } = contactPhone;
    public string ContactEmail { get; set; } = contactEmail;
    public string Postcode { get; set; } = postcode;

    override public string ToString()
    {
        return $"[{Id}] {FirstName} {LastName} - Born {DateOfBirth:yyyy-MM-dd} - {Height}cm tall - {AddressLine} {Postcode} - Contact Phone: {ContactPhone}, Contact Email: {ContactEmail}";
    }
}