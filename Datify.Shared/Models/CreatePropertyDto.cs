using System.ComponentModel.DataAnnotations;
using Datify.Shared.Models.Enum;
using Microsoft.AspNetCore.Http;

namespace Datify.Shared.Models;

public class CreatePropertyDto : BaseProperties
{
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Category is required.")]
    public string Category { get; set; } = null!;

    public string Condition { get; set; } = "NA";

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    [StringLength(3, ErrorMessage = "Currency should be a valid 3-letter code.")]
    public string Currency { get; set; } = "NGN";

    [Required(ErrorMessage = "Town is required.")]
    public string Town { get; set; } = "NA";

    [Required(ErrorMessage = "State is required.")]
    public string State { get; set; } = "NA";

    [Range(0, int.MaxValue, ErrorMessage = "Bathrooms count cannot be negative.")]
    public int Bathrooms { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Bedrooms count cannot be negative.")]
    public int Bedrooms { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Living rooms count cannot be negative.")]
    public int LivingRooms { get; set; }

    [Required(ErrorMessage = "Payment frequency is required.")]
    public string PaymentFrequency { get; set; } = "per night";

    [Required(ErrorMessage = "Market type is required.")]
    public PropertyMarketType MarketType { get; set; }

    [Required(ErrorMessage = "Location is required.")]
    public string Location { get; set; } = null!;

    [Required(ErrorMessage = "Collection option is required.")]
    public string CollectionOption { get; set; } = "NA";

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "User ID is required.")]
    public string UserId { get; set; } = null!;

    public UserDto? User { get; set; }

    public List<IFormFile> DocumentsToUpload { get; set; } = [];

    public List<PropertyDocumentDto> PropertyDocuments { get; set; } = [];

    public List<PropertyFeatureDto> Features { get; set; } = [];

    public List<PropertyProximityPlaceDto> ProximityPlaces { get; set; } = [];

    [Required(ErrorMessage = "Property type is required.")]
    public string PropertyType { get; set; } = null!;

    [Required(ErrorMessage = "Property measurement is required.")]
    public string PropertyMeasurement { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Max guests must be at least 1.")]
    public int MaxGuests { get; set; }
    [Required(ErrorMessage = "Landmark is required.")]

    public string LandMark { get; set; }

    //[Required(ErrorMessage = "Nightly rate is required.")]
    public string NightlyRate { get; set; }

    [Required(ErrorMessage = "Security deposit is required.")]
    public string SecurityDeposit { get; set; }

    [Required(ErrorMessage = "Final pricing is required.")]
    public string FinalPricing { get; set; } = "250,000";

    public string HouseRules { get; set; }

    public bool EventsAllowed { get; set; }

    public List<PropertyFeesDto> PropertyFees { get; set; } = [];

    public List<PropertyRatingDto> Ratings { get; set; } = [];

    public List<PropertyRulesDto> PropertyRules { get; set; } = [];

    public List<PropertyAllowedEventsDto> PropertyAllowedEvents { get; set; } = [];

    public bool HasParking { get; set; }

    public bool HasFreeWifi { get; set; }

    public bool HasSwimmingPool { get; set; }

    public bool HasAirConditioning { get; set; }

    [Required(ErrorMessage = "Check-in time is required.")]
    public string CheckInTime { get; set; }

    [Required(ErrorMessage = "Check-out time is required.")]
    public string CheckOutTime { get; set; }

    [Required(ErrorMessage = "Security deposit condition is required.")]
    public string SecurityDepositCondition { get; set; }

    public string BuildingName { get; set; }
    public string BuildingNumber { get; set; }
    public string HouseNumber { get; set; }
}