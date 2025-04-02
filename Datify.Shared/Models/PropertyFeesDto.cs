namespace Datify.Shared.Models;

public class PropertyFeesDto : BaseProperties
{
    public long PropertyId { get; set; } // FK to Property
    public string? FeesDescription { get; set; }
    public string FeesName { get; set; }
    public decimal FeesAmount { get; set; }
}