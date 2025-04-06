using Datify.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace Datify.API.Data;

public class ApplicationUser : IdentityUser
{
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
    public List<Interest>? Interests { get; set; } // Updated to use Interest class
    public string? Location { get; set; }
    public List<Photo>? Photos { get; set; } // Updated to use Photo class
}

public class Interest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class Photo
{
    public Guid Id { get; set; }
    public string Url { get; set; }
}