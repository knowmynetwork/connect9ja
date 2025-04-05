using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Datify.Shared.Models;
using Datify.Shared.Utilities;
using System.Security.Claims;
using System.Text;
using Datify.API.Contracts;
using Datify.API.Data;
using Microsoft.Win32;
using AutoMapper;

namespace Datify.API.Services;

public class UserService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager,IEmailService emailService, IMapper mapper) : IUserService
{
    public async ValueTask<List<UserDto>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await dbContext.Users.ToListAsync();  

        if (users == null || !users.Any())
            return new List<UserDto>();

        // Map the users to DTO
        var userDtos = mapper.Map<List<UserDto>>(users);

        return userDtos;
    }


    public async ValueTask<List<UserDto>> FilterUsersAsync(string? userName, string? email, string? gender)
    {
        var query = dbContext.Users.AsQueryable();  // Start with a queryable list of users

        // Apply filters dynamically based on the query parameters
        if (!string.IsNullOrEmpty(userName))
        {
            query = query.Where(u => u.UserName.Contains(userName));  
        }

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(u => u.Email.Contains(email));  
        }

        if (!string.IsNullOrEmpty(gender))
        {
            query = query.Where(u => u.Gender.Contains(gender));  
        }

        // Execute the query and get the filtered users
        var filteredUsers = await query.ToListAsync();

        if (filteredUsers == null || !filteredUsers.Any())
            return new List<UserDto>();

        // Map the filtered users to DTO
        var userDtos = mapper.Map<List<UserDto>>(filteredUsers);

        return userDtos;
    
    }



    public async ValueTask<ApplicationUser?> GetById(string id, CancellationToken cancellationToken)
        => await dbContext.Users.FindAsync([id], cancellationToken: cancellationToken);

    public async ValueTask<string?> GetUserClaims(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var validUser = await userManager.GetUserAsync(claimsPrincipal);
        if (validUser is null) return null;

        // ✅ Ensure roles are stored as a List<string>
        var roles = new List<string>();
        if (validUser.Email!.StartsWith("admin@"))
        {
            roles.Add("Admin");
        }
        else
        {
            roles.Add("User");
        }

        var claims = new UserClaimsDto(
            validUser.Id,
            validUser.UserName!,
            validUser.Email!,
            roles // ✅ Now passing a List<string> instead of a string
        );

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(claims.ToJson()));
    }
    public async ValueTask<bool> UpdateById(string id, UserDto userDto, CancellationToken cancellationToken)
    {
        var existingUser = await dbContext.Users.FindAsync([id], cancellationToken: cancellationToken);
        if (existingUser is null) return false;
        existingUser.EmailConfirmed = userDto.EmailConfirmed;
        existingUser.PhoneNumberConfirmed = userDto.PhoneNumberConfirmed;
        existingUser.TwoFactorEnabled = userDto.TwoFactorEnabled;
        existingUser.LockoutEnabled = userDto.LockoutEnabled;
        dbContext.Update(existingUser);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async ValueTask<bool> DeleteById(string id, CancellationToken cancellationToken)
    {
        var existingUser = await dbContext.Users.FindAsync([id], cancellationToken: cancellationToken);
        if (existingUser is null) return false;
        dbContext.Remove(existingUser);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async ValueTask<bool> RegisterUser(RegisterModelDto model, UserManager<ApplicationUser> userManager)
    {
        // check for existing user
        var existingUser = await userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            throw new Exception("Email already exists");
        }
        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            Gender = model.Gender ?? "NA",
            FirstName = model.FirstName,
            LastName = model.LastName,            
            DateOfBirth = model.DateOfBirth,
            PhoneNumber = model.PhoneNumber,
            PasswordHash = model.Password,
            
           
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
        // Add user claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new("Gender", user.Gender) // Custom claim
        };
        var subject = "Welcome onboard";
        var body = "Congratulations! Your account has been registered successfully.";
        await userManager.AddClaimsAsync(user, claims);
        await emailService.SendEmailAsync(model.Email, subject, body);
        }
        return result.Succeeded;

    }
}
