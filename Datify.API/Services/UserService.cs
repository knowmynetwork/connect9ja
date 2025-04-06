using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Datify.Shared.Models;
using Datify.Shared.Utilities;
using System.Security.Claims;
using System.Text;
using Datify.API.Contracts;
using Datify.API.Data;
using AutoMapper;

namespace Datify.API.Services;

public class UserService(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IMapper mapper) : IUserService
{
    public async ValueTask<List<UserDto>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await dbContext.Users.ToListAsync();  

        if (users.Count == 0)
            return [];

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
            query = query.Where(u => u.UserName != null && u.UserName.Contains(userName));  
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

        if (filteredUsers.Count == 0)
            return [];

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
        if (validUser.Email.StartsWith("admin@"))
        {
            roles.Add("Admin");
        }
        else
        {
            roles.Add("User");
        }

        var claims = new UserClaimsDto(
            validUser.Id,
            validUser.UserName ?? "",
            validUser.Email,
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

    public async ValueTask<bool> RegisterUser(RegisterModelDto model, UserManager<ApplicationUser> userManager2,
        HttpContext httpContext)
    {
        // check for existing user
        var existingUser = await userManager2.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            throw new Exception("Email already exists");
        }

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            Gender = model.Gender,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            PhoneNumber = model.PhoneNumber,
        };

        var result = await userManager2.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new Exception($"Registration failed: {errors}");
        }

        // Add user claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new("Gender", user.Gender) // Custom claim
        };

        // Add email confirmation
        var token = await userManager2.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);
        var request = httpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var confirmationLink = $"{baseUrl}/api/users/confirm-email?userId={user.Id}&token={encodedToken}";

        var emailHtml = $"""
                             <html>
                             <body>
                                 <h2>Welcome to Datify 👋</h2>
                                 <p>Click the button below to confirm your email and activate your account:</p>
                                 <p><a href="{confirmationLink}" style="padding: 10px 15px; background-color: #4CAF50; color: white; text-decoration: none;">Confirm Email</a></p>
                                 <p>If you did not request this, please ignore this email.</p>
                             </body>
                             </html>
                         """;

        await emailService.SendEmailAsync(user.Email, "Confirm your email", emailHtml);

        return true;
    }

    
}
