using Datify.API.Contracts;
using Datify.API.Data;
using Datify.Shared.Models;
using Datify.Shared.Models.Enum;

namespace Datify.API.Services
{
    public interface IDocumentService : IServices
    {
        Task<(bool Success, string Message)> DeleteDocumentAsync(DocumentTwoDto documentx);
        Task<(bool Success, string Message, DocumentDto Document)> UploadDocumentAsync(ApplicationUser user, IFormFile file, string documentType, string base64String = "", string base64FileName = "");
        Task<(bool Success, string Message, DocumentDto Document)> UploadDocumentAsync(long userId, IFormFile file, string documentType, string base64String = "", string base64FileName = "");
        Task<List<DocumentDto>> GetUserDocumentsAsync(string userId);

        public Task<(bool Success, string Message, PropertyDocumentDto Document)> UploadPropertyDocumentAsync(long itemId, IFormFile file, string documentType);
        public Task<(bool Success, string Message)> DeleteItemDocumentAsync(long documentId);
        public Task<(bool Success, string Message)> DeleteDocumentAsync(long documentId);
        DocumentType ConvertToDocumentType(string documentTypeString);
        Task<(bool Success, string Message, PropertyDocumentDto Document)> ReplacePropertyDocumentAsync(long documentId,
            IFormFile file, string documentType);
    }


}
