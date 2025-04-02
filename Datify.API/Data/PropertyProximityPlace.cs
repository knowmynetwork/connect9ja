using System.ComponentModel.DataAnnotations.Schema;
using Datify.Shared.Models;

namespace Datify.API.Data;

public class PropertyProximityPlace : BaseProperties
{
    public string? ProximityPlaceName { get; set; }
    public string? ProximityPlaceDescription { get; set; }
    public int Distance { get; set; }

    [ForeignKey(nameof(Property))]
    public long PropertyId { get; set; } 
    public Property Property { get; set; } = default!;
}