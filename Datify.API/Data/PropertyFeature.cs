using System.ComponentModel.DataAnnotations.Schema;
using Datify.Shared.Models;

namespace Datify.API.Data;

public class PropertyFeature : BaseProperties
{
    public string? FeatureName { get; set; }
    public string? FeatureDescription { get; set; }
    public int NumberOfFeatures { get; set; }
    public bool IsAvailable { get; set; }

    // Foreign Key to PropertyItem
    [ForeignKey(nameof(Property))]
    public long PropertyId { get; set; }
    public Property Property { get; set; } = default!;
}