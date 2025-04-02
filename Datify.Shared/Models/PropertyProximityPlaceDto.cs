namespace Datify.Shared.Models;

public class PropertyProximityPlaceDto : BaseProperties
{
    public string? ProximityPlaceName { get; set; }
    public string? ProximityPlaceDescription { get; set; }
    public int Distance { get; set; }
    public int PropertyItemId { get; set; }
}