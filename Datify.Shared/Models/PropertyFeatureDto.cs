namespace Datify.Shared.Models;

public class PropertyFeatureDto : BaseProperties
{
    public string? FeatureName { get; set; }
    public string? FeatureDescription { get; set; }
    public int NumberOfFeatures { get; set; }
    public int PropertyItemId { get; set; }
    public bool IsAvailable { get; set; }
    public string? IsAvailableString { get; set; }
}