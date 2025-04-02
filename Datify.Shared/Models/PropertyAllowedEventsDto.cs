namespace Datify.Shared.Models;

public class PropertyAllowedEventsDto : BaseProperties
{
    public long PropertyId { get; set; } // FK to Property
    public string? EventDescription { get; set; }
    public decimal Amount { get; set; }

}