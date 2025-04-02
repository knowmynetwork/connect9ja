namespace Datify.Shared.Models;

public class CreatePropertyRatingDto
{
    public long PropertyId { get; set; } // The Property being rated
    public int Rating { get; set; } // 1-5 star rating
    public string? Review { get; set; } // Optional Review
}


public class PropertyRatingDto
{
    public long Id { get; set; } // Rating ID
    public string UserId { get; set; } = null!; // User who rated
    public int Rating { get; set; } // 1-5 star rating
    public string? Review { get; set; } // Optional review
    public DateTime CreatedAt { get; set; } // When the rating was submitted
}