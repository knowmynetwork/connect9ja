using System.ComponentModel.DataAnnotations;

namespace Datify.Shared.Models;

public class BookingCreationDto
{
    
    public long Id { get; set; }
    public string BookingUserId { get; set; }
    
    [Required]
    public long PropertyId { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    public string Type { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    [Required]
    public DateTime? AvailableStartDate { get; set; }
    [Required]
    public DateTime? AvailableEndDate { get; set; }
    [Required]
    public int? MobileNo { get; set; }
    [Required]
    public int? NumberOfNights { get; set; }
    public int? OccupantsNumber { get; set; }
    
    public decimal? Price { get; set; }
    
    public string? Address { get; set; }
    
    public bool IsBookingForSoneoneElse { get; set; }
}