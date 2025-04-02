using Datify.Shared.Models.Enum;

namespace Datify.Shared.Models;

public class SearchPropertyDto
{
    public string Location { get; set; }= null!;
    public int Bathrooms { get; set; }
    public DateTimeOffset? AvailableStartDate { get; set; }
    public DateTimeOffset? AvailableEndDate { get; set; }
    public int Bedrooms { get; set; }
    public int LivingRooms { get; set; }
    public int NoOfRooms { get; set; }
    public string Town { get; set; }= "NA";
    public string State { get; set; }= "NA";
    public PropertyMarketType MarketType { get; set; }
}