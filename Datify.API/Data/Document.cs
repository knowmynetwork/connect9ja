using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Datify.Shared.Models;
using Datify.Shared.Models.Enum;

namespace Datify.API.Data;

public class Document : BaseProperties
{
    public string UserForeignKeyId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string DocumentPath { get; set; } = default!;
    public string DocumentName { get; set; } = default!;
    public DateTime UploadedDate { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(UserForeignKeyId))]
    public ApplicationUser? User { get; set; }
}
