namespace Domain.Entities;

public sealed class User(
    Guid id,
    string role,
    string firstName,
    string lastName,
    int age,
    DateTime dateOfBirth,
    string address)
{
    public Guid Id { get; private set; } = id;
    public string Role { get; private set; } = role;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public int Age { get; private set; } = age;
    public DateTime DateOfBirth { get; private set; } = dateOfBirth;
    public string Address { get; private set; } = address;
}