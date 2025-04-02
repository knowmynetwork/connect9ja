using Datify.Shared.Models.Enum;

namespace Datify.Shared.Models;

public class PropertyDocumentDto : BaseProperties
{
    public long ItemId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string DocumentPath { get; set; } = default!; // Path to the document file
    public string DocumentName { get; set; } = default!; // Path to the document file
    public DateTime UploadedDate { get; set; }
}