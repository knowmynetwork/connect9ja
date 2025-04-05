using Datify.API.Contracts;
using Datify.Shared.Models;

namespace Datify.API.Services
{
    public interface IDatifyProfileService : IServices
    {
        Task<(bool Success, string Message)> AddProfile(DatifyProfileDto profileDto);
        Task<(bool Success, string Message)> EditProfile(DatifyProfileDto profileDto, string id);
        Task<(bool Success, string Message)> DeleteProfile(string id);
        Task<DatifyProfileDto> GetUserProfileById(string id);
    }
}
