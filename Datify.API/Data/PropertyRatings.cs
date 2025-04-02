using Datify.Shared.Models;

namespace Datify.API.Data;

public class PropertyRatings : BaseProperties
{
    public long PropertyId { get; set; } // FK to Property
    public Property Property { get; set; } = null!; // Navigation Property
    public string UserId { get; set; } = null!; // FK to User
    public ApplicationUser User { get; set; } = null!; // Navigation Property
    public int Rating { get; set; } // 1-5 Star Rating
    public string? Review { get; set; } // Optional Review Text
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp

    // Constructor to enforce rating range
    public PropertyRatings(long propertyId, string userId, int rating, string? review = null)
    {
        PropertyId = propertyId;
        UserId = userId;
        Rating = rating is >= 1 and <= 5 ? rating : throw new ArgumentException("Rating must be between 1 and 5.");
        Review = review;
        CreatedAt = DateTime.UtcNow;
    }
}

public class PropertyRules : BaseProperties
{
    public long PropertyId { get; set; } // FK to Property
    public Property Property { get; set; } = null!; // Navigation Property
    public string? RuleDescription { get; set; }
}

public class PropertyAllowedEvents : BaseProperties
{
    public long PropertyId { get; set; } // FK to Property
    public Property Property { get; set; } = null!; // Navigation Property
    public string? EventDescription { get; set; }
}