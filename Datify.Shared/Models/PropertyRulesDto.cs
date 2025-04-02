namespace Datify.Shared.Models;

public class PropertyRulesDto : BaseProperties
{
    public long PropertyId { get; set; } // FK to Property
    public string? RuleDescription { get; set; }
}