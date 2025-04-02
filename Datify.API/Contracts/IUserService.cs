using Microsoft.AspNetCore.Identity;
using Datify.Shared.Models;
using System.Security.Claims;
using Datify.API.Data;

namespace Datify.API.Contracts;
public interface IUserService : IServices
{
    ValueTask<bool> DeleteById(string id, CancellationToken cancellationToken);
    ValueTask<ApplicationUser[]> GetAll(CancellationToken cancellationToken);
    ValueTask<ApplicationUser?> GetById(string id, CancellationToken cancellationToken);
    ValueTask<string?> GetUserClaims(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken);
    ValueTask<bool> UpdateById(string id, UserDto userDto, CancellationToken cancellationToken);
}