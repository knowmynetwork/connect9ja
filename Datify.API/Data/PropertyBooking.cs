using Datify.Shared.Models;

namespace Datify.API.Data;

public class PropertyBooking : BaseProperties
{
    public string? BookingUserId { get; set; }
    public long PropertyId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTimeOffset AvailableStartDate { get; set; }
    public DateTimeOffset AvailableEndDate { get; set; }
    public string? MobileNo { get; set; }
    public string? OccupantsNumber { get; set; }
    public decimal Price{get;set;}
    public bool IsPaid { get; set; }
    public DateTimeOffset DatePaid { get; set; }
}