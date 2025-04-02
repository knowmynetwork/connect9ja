using AutoMapper;
using Azure.Storage.Blobs;
using Datify.API.Data;
using Microsoft.EntityFrameworkCore;
using Datify.Shared.Models;
using Datify.Shared.Models.Enum;

namespace Datify.API.Services;

public class DocumentService(ApplicationDbContext context, BlobServiceClient blobServiceClient, IMapper mapper)
    : IDocumentService
{
    private readonly string? _containerName = GetContainerName();

    private static string GetContainerName()
    {
        var name = "";
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            name = "testdevcontainer";
        }
        else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            name = "homxlycontainer";
        }
        return name;
    }

    public async Task<(bool Success, string Message, DocumentDto Document)> UploadDocumentAsync(long userId, IFormFile file, string documentType, string base64String = "", string base64FileName = "")
    {
        BlobClient? blobClient1 = null;
        string fileName = "";

        if (!string.IsNullOrEmpty(base64String))
        {
            fileName = $"{Guid.NewGuid()}_{base64FileName}";

            // Get the container client
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Get the blob client
            blobClient1 = containerClient.GetBlobClient(fileName);

            // Convert the base64 string to a byte array
            byte[] fileBytes = Convert.FromBase64String(base64String);

            // Upload the file
            using var ms = new MemoryStream(fileBytes);
            await blobClient1.UploadAsync(ms, true);
        }
        else if (file != null)
        {
            fileName = file.FileName;
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            _ = $"{Guid.NewGuid()}_{file.FileName}";
            blobClient1 = containerClient.GetBlobClient($"{Guid.NewGuid()}_{file.FileName}");
            using var stream = file.OpenReadStream();
            await blobClient1.UploadAsync(stream, true);
        }

        var documentDto = new DocumentDto
        {
            DocumentPath = blobClient1?.Uri.ToString() ?? "",
            UploadedDate = DateTime.Now,
            DocumentName = fileName
        };

        return (true, "Document uploaded successfully", documentDto);
    }

    public async Task<(bool Success, string Message, DocumentDto Document)> UploadDocumentAsync(ApplicationUser user, IFormFile file, string documentType, string base64String = "", string base64FileName = "")
    {
        BlobClient? blobClient1 = null;
        string fileName = "";
        var dateNow = DateTime.UtcNow;
        var uniqueKey = $"{user.Email}_{dateNow:yyyyMMddHHmmsstt}_{ dateNow.Ticks}";
        if (!string.IsNullOrEmpty(base64String))
        {
            fileName = base64FileName;

            // Get the container client
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Get the blob client
            blobClient1 = containerClient.GetBlobClient($"{uniqueKey}_fileName");

            // Convert the base64 string to a byte array
            byte[] fileBytes = Convert.FromBase64String(base64String);

            // Upload the file
            using var ms = new MemoryStream(fileBytes);
            await blobClient1.UploadAsync(ms, true);
        }
        else if (file != null)
        {
            fileName = file.FileName;
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            blobClient1 = containerClient.GetBlobClient($"{uniqueKey}_{file.FileName}");
            using var stream = file.OpenReadStream();
            await blobClient1.UploadAsync(stream, true);
        }

        var documentDto = new DocumentDto
        {
            DocumentPath = blobClient1?.Uri.ToString() ?? "",
            UploadedDate = DateTime.Now,
            DocumentName = fileName
        };

        return (true, "Document uploaded successfully", documentDto);
    }


    public async Task<(bool Success, string Message, PropertyDocumentDto Document)> UploadPropertyDocumentAsync(long itemId, IFormFile file, string documentType)
    {
        if (file == null || file.Length == 0)
            return (false, "File is empty", new PropertyDocumentDto());

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        await using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }

        var document = new PropertyDocument
        {
            ItemId = itemId,
            DocumentPath = blobClient.Uri.ToString(),
            DocumentType = ConvertToDocumentType(documentType),
            UploadedDate = DateTime.UtcNow,
            DocumentName = file.FileName,
            PropertyId = itemId
        };

        context.PropertyDocuments.Add(document);
        await context.SaveChangesAsync();

        var documentDto = new PropertyDocumentDto
        {
            Id = document.Id,
            DocumentType = document.DocumentType,
            DocumentPath = document.DocumentPath,
            UploadedDate = document.UploadedDate,
            DocumentName = document.DocumentName
        };

        return (true, "Document uploaded successfully", documentDto);
    }


    public async Task<(bool Success, string Message)> DeleteItemDocumentAsync(long documentId)
    {

        // Find the document in the database
        var document = await context.PropertyDocuments.FindAsync(documentId);
        if (document == null)
        {
            return (false, "Document not found");
        }

        // Delete from blob storage
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var blobName = Path.GetFileName(new Uri(document.DocumentPath).LocalPath);
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();

        // Delete from database
        context.PropertyDocuments.Remove(document);
        await context.SaveChangesAsync();

        return (true, "Document deleted successfully");
    }

    public async Task<(bool Success, string Message)> DeleteDocumentAsync(long documentId)
    {

        // Find the document in the database
        var document = await context.Documents.FindAsync(documentId);
        if (document == null)
        {
            return (false, "Document not found");
        }

        // Delete from blob storage
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var blobName = Path.GetFileName(new Uri(document.DocumentPath).LocalPath);
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();

        // Delete from database
        context.Entry(document).State = EntityState.Deleted;
        await context.SaveChangesAsync();

        return (true, "Document deleted successfully");
    }

    public async Task<(bool Success, string Message)> DeleteDocumentAsync(DocumentTwoDto documentx)
    {

        // // Find the document in the database
        Document document = new()
        {
            UserForeignKeyId = documentx.UserForeignKeyId,
            DocumentPath = documentx.DocumentPath ?? "",
            DocumentType = documentx.DocumentType,
            UploadedDate = DateTime.UtcNow,
            DocumentName = documentx.DocumentName,
            Id = documentx.Id,
            WasActionDoneBySomeoneElse = documentx.WasActionDoneBySomeoneElse,
            WhichCommunityWasActionDoneFor = documentx.WhichCommunityWasActionDoneFor,
            WhenWasActionDone = documentx.WhenWasActionDone,
            WhoDidTheAction = documentx.WhoDidTheAction,
            
        };
  

        // Delete from blob storage
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var blobName = Path.GetFileName(new Uri(document.DocumentPath).LocalPath);
        var blobClient = containerClient.GetBlobClient(blobName);

        var delete = await blobClient.DeleteIfExistsAsync();

        return (delete, "Document deleted successfully");


    }

    // Helper method to extract blob name from URL

    public async Task<List<DocumentDto>> GetUserDocumentsAsync(string userId)
    {
        return await context.Documents
            .Where(d => d.UserForeignKeyId == userId)
            .Select(d => new DocumentDto
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                DocumentPath = d.DocumentPath,
                UploadedDate = d.UploadedDate
            })
            .ToListAsync();
    }

    public DocumentType ConvertToDocumentType(string documentTypeString)
    {
        if (Enum.TryParse(documentTypeString, true, out DocumentType result))
        {
            return result;
        }
        else
        {
            // Handle the case where the string doesn't match any enum value
            // You could return a default value or throw an exception
            return DocumentType.MarketItem; // or whatever your default should be
        }
    }

    public async Task<(bool Success, string Message, PropertyDocumentDto Document)> ReplacePropertyDocumentAsync(long documentId, IFormFile file, string documentType)
    {
        var existingDocument = await context.PropertyDocuments.FindAsync(documentId);
        if (existingDocument == null)
        {
            return (false, "Document not found", null);
        }

        // Delete the old file from blob storage
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var oldBlobName = Path.GetFileName(new Uri(existingDocument.DocumentPath).LocalPath);
        var oldBlobClient = containerClient.GetBlobClient(oldBlobName);
        await oldBlobClient.DeleteIfExistsAsync();

        // Upload the new file
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var newBlobClient = containerClient.GetBlobClient(fileName);

        await using (var stream = file.OpenReadStream())
        {
            await newBlobClient.UploadAsync(stream, true);
        }

        // Update the document in the database
        existingDocument.DocumentPath = newBlobClient.Uri.ToString();
        existingDocument.DocumentName = file.FileName;
        existingDocument.DocumentType = ConvertToDocumentType(documentType);
        existingDocument.UploadedDate = DateTime.UtcNow;

        await context.SaveChangesAsync();

        var documentDto = mapper.Map<PropertyDocumentDto>(existingDocument);
        return (true, "Document replaced successfully", documentDto);
    }

}