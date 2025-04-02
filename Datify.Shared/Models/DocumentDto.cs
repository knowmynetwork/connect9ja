using System.ComponentModel.DataAnnotations.Schema;
using Datify.Shared.Models.Enum;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace Datify.Shared.Models;

public class DocumentDto : BaseProperties
{
    public DocumentType DocumentType { get; set; }
    public string? DocumentPath { get; set; } = default!; // Path to the document file
    public string DocumentName { get; set; } = default!;

    public DateTime UploadedDate { get; set; }
    [NotMapped]
    public IBrowserFile? File { get; set; }
    public CrudOperation CrudOperation { get; set; } = CrudOperation.Read;

    public string UserForeignKeyId { get; set; }
    public UserDto User { get; set; } = default!;
}

public class DocumentTwoDto : BaseProperties
{
    public DocumentType DocumentType { get; set; }
    public string? DocumentPath { get; set; } = default!; // Path to the document file
    public string DocumentName { get; set; } = default!;
    public CrudOperation CrudOperation { get; set; } = CrudOperation.Read;
    public DateTime UploadedDate { get; set; }
    public IFormFile? File { get; set; }

    public string UserForeignKeyId { get; set; }
    public UserDto? User { get; set; }
}

public enum CrudOperation
{
    Create,
    Update,
    Delete,
    Read
}
