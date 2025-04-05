namespace Datify.Shared.Models;

public class RegisterModelDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Gender { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public String? Hobies { get; set; }
    public String? Location { get; set; }
}