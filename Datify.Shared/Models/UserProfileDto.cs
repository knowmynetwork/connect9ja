using Microsoft.AspNetCore.Http;

namespace Datify.Shared.Models;

public class UserProfileDto
{
    public string? Email { get; set; }
    public Guid Id { get; set; }
    public string? PhoneNumber { get; set; }
    public string? NickName { get; set; }
    public string? Bio { get; set; }
    public string? Hobies { get; set; }
    public string? Location { get; set; }
    public IFormFile? ProfilePicture { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }    
    public string? ResetToken { get; set; } 
    public DateTime? ResetTokenExpiry { get; set; } = DateTime.UtcNow.AddMinutes(15);
    public string? Nickname { get; set; }
    public DateTime? Birthday { get; set; }
    public string? RelationshipGoals { get; set; }
    public double? DistancePreference { get; set; }
    public List<InterestDto>? Interests { get; set; } // Updated to use InterestDto
    public List<PhotoDto>? Photos { get; set; } // Updated to use PhotoDto
}