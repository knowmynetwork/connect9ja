using System.ComponentModel.DataAnnotations.Schema;
using Datify.Shared.Models;
using Datify.Shared.Models.Enum;

namespace Datify.API.Data;

public class PropertyDocument : BaseProperties
{
    public long ItemId { get; set; }
    public Property Property { get; set; } = default!;
    [ForeignKey(nameof(Property))]
    public long PropertyId { get; set; }

    public DocumentType DocumentType { get; set; }
    public string DocumentPath { get; set; } = default!; // Path to the document file
    public string DocumentName { get; set; } = default!; // Path to the document file
    public DateTime UploadedDate { get; set; }
}