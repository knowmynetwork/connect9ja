using Datify.Shared.Models;

namespace Datify.API.Data;

public class PropertyFees : BaseProperties
{
    public long PropertyId { get; set; } // FK to Property
    public string? FeesDescription { get; set; }
    public string FeesName { get; set; }
    public decimal FeesAmount { get; set; }
}