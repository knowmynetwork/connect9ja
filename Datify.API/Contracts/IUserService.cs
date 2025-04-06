using Microsoft.AspNetCore.Identity;
using Datify.Shared.Models;
using System.Security.Claims;
using Datify.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace Datify.API.Contracts;
public interface IUserService : IServices
{
    ValueTask<bool> RegisterUser(RegisterModelDto model, UserManager<ApplicationUser> userManager2,
        HttpContext httpContext);
    ValueTask<bool> DeleteById(string id, CancellationToken cancellationToken);
    //ValueTask<List<UserDto>> GetAllUsers2(CancellationToken cancellationToken);
    ValueTask<ApplicationUser?> GetById(string id, CancellationToken cancellationToken);
    ValueTask<string?> GetUserClaims(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken);
    ValueTask<bool> UpdateById(string id, UserDto userDto, CancellationToken cancellationToken);
    ValueTask<List<UserDto>> FilterUsersAsync(string? fullName, string? email, string? gender);
    ValueTask<bool> UpdateFirstName(string userId, string firstName, CancellationToken cancellationToken);
    ValueTask<bool> UpdateLastName(string userId, string lastName, CancellationToken cancellationToken);
    ValueTask<bool> UpdateGender(string userId, string gender, CancellationToken cancellationToken);
    ValueTask<bool> UpdateDateOfBirth(string userId, DateTime dateOfBirth, CancellationToken cancellationToken);
    ValueTask<bool> UpdateNickname(string userId, string nickname, CancellationToken cancellationToken);
    ValueTask<bool> UpdateRelationshipGoals(string userId, string relationshipGoals, CancellationToken cancellationToken);
    ValueTask<bool> UpdateDistancePreference(string userId, double distancePreference, CancellationToken cancellationToken);
    ValueTask<bool> UpdateLocation(string userId, string location, CancellationToken cancellationToken);
    ValueTask<List<UserProfileDto>> GetAllUsers(CancellationToken cancellationToken);
    ValueTask<UserProfileDto?> GetUserProfile(string userId, CancellationToken cancellationToken);
}