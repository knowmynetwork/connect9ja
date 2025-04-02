using Datify.Shared.Models;
using Datify.Shared.Models.Enum;

namespace Datify.API.Data;

public class Property : BaseProperties
{
    public string Title { get; set; }= null!;
    public string Category { get; set; }= null!;
    public string? Condition { get; set; }
    public string Location { get; set; }= null!;
    public string Town { get; set; }= "NA";
    public string State { get; set; }= "NA";
    public string CollectionOption { get; set; }= null!;
    public string Description { get; set; }= null!;
    public string? PaymentFrequency { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "NGN";
    public int Bathrooms { get; set; }
    public int Bedrooms { get; set; }
    public int LivingRooms { get; set; } = 0;
    public string PropertyMeasurement { get; set; } = null!;
    public int MaxGuests { get; set; }
    public string? LandMark { get; set; }
    public string PropertyType { get; set; }= "NA";
    public PropertyMarketType MarketType { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public ICollection<PropertyDocument> PropertyItemDocuments { get; set; } = [];
    public ICollection<PropertyFeature> Features { get; set; } = [];
    public ICollection<PropertyProximityPlace> ProximityPlaces { get; set; } = [];
    public ICollection<PropertyRatings> Ratings { get; set; } = [];
    public ICollection<PropertyRules> PropertyRules { get; set; } = [];
    public ICollection<PropertyAllowedEvents> PropertyAllowedEvents { get; set; } = [];
    
}

