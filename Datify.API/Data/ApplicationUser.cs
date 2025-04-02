using Microsoft.AspNetCore.Identity;

namespace Datify.API.Data;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string? ResetToken { get; set; } 
    public DateTime? ResetTokenExpiry { get; set; } = DateTime.UtcNow.AddMinutes(15);
}