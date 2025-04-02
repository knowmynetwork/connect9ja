using Datify.Shared.Models.Enum;

namespace Datify.Shared.Models;

public class PropertyDto : BaseProperties
{
    public string Title { get; set; }= null!;
    public string Category { get; set; } = null!;
    public string Condition { get; set; } = null!;
    public decimal Price { get; set; }
    public string Location { get; set; }= null!;
    public string CollectionOption { get; set; }= null!;
    public string Description { get; set; }= null!;
    public int Bathrooms { get; set; }
    public string PaymentFrequency { get; set; } = default!;
    public string Currency { get; set; } = default!;
    public int Bedrooms { get; set; }
    public int LivingRooms { get; set; }
    public string Town { get; set; }= "NA";
    public string State { get; set; }= "NA";
    public string PropertyType { get; set; }= null!;
    public string PropertyMeasurement { get; set; } = null!;
    public int MaxGuests { get; set; }
    public string LandMark { get; set; }
    public PropertyMarketType MarketType { get; set; }
    public UserDto User { get; set; } = default!;
    
    public string NightlyRate { get; set; }
    public string SecurityDeposit { get; set; }
    public string FinalPricing { get; set; } = "250,000";
    public string HouseRules { get; set; }
    
    public bool EventsAllowed { get; set; }
    public List<PropertyDocumentDto> PropertyDocuments { get; set; } = [];
    public List<PropertyFeesDto> PropertyFees { get; set; } = [];
    public List<PropertyFeatureDto> Features { get; set; } = [];
    public List<PropertyProximityPlaceDto> ProximityPlaces { get; set; } = [];
    public List<PropertyRatingDto> Ratings { get; set; } = [];
    public List<PropertyRulesDto> PropertyRules { get; set; } = [];
    public List<PropertyAllowedEventsDto> PropertyAllowedEvents { get; set; } = [];
    public bool HasFreeWifi { get; set; }
    public bool HasSwimmingPool { get; set; }
    public bool HasAirConditioning { get; set; }
    public string CheckInTime { get; set; }
    public string CheckOutTime { get; set; }
    public string SecurityDepositCondition { get; set; }
}