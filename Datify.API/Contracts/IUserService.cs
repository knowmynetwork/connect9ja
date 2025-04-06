using Microsoft.AspNetCore.Identity;
using Datify.Shared.Models;
using System.Security.Claims;
using Datify.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace Datify.API.Contracts;
public interface IUserService : IServices
{
    ValueTask<bool> RegisterUser(RegisterModelDto model, UserManager<ApplicationUser> userManager2);
    ValueTask<bool> DeleteById(string id, CancellationToken cancellationToken);
    ValueTask<List<UserDto>> GetAllUsers(CancellationToken cancellationToken);
    ValueTask<ApplicationUser?> GetById(string id, CancellationToken cancellationToken);
    ValueTask<string?> GetUserClaims(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken);
    ValueTask<bool> UpdateById(string id, UserDto userDto, CancellationToken cancellationToken);
    ValueTask<List<UserDto>> FilterUsersAsync(string? fullName, string? email, string? gender);

}