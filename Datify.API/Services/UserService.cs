using System.Diagnostics;
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

public class UserService : IUserService
{
    private readonly ApplicationDbContext dbContext;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IEmailService emailService;
    private readonly IMapper mapper;

    public UserService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IEmailService emailService, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.emailService = emailService;
        this.mapper = mapper;
    }

    public async ValueTask<List<UserProfileDto>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .Include(u => u.Interests) // Include related Interests
            .Include(u => u.Photos)   // Include related Photos
            .ToListAsync(cancellationToken);

        if (users.Count == 0)
            return new List<UserProfileDto>();

        // Map the users to UserProfileDto
        var userProfileDtos = mapper.Map<List<UserProfileDto>>(users);

        return userProfileDtos;
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

    public async ValueTask<UserProfileDto?> GetUserProfile(string userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Include(u => u.Interests) // Include related Interests
            .Include(u => u.Photos)   // Include related Photos
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return null;

        // Map the user to UserProfileDto
        var userProfileDto = mapper.Map<UserProfileDto>(user);

        return userProfileDto;
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

    public async ValueTask<UserProfileDto> RegisterUser(RegisterModelDto model, UserManager<ApplicationUser> userManager2, HttpContext httpContext)
    {
        // Check for existing user
        var existingUser = await userManager2.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            throw new Exception("Email already exists");
        }

        // Create a new user
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

        // Map the newly created user to UserProfileDto
        var userProfileDto = mapper.Map<UserProfileDto>(user);

        return userProfileDto;
    }

    public async ValueTask<bool> UpdateFirstName(string userId, string firstName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(firstName)) throw new ArgumentException("First name cannot be null or empty");

        var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) throw new Exception("User not found");

        user.FirstName = firstName;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async ValueTask<bool> UpdateLastName(string userId, string lastName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(lastName)) throw new ArgumentException("Last name cannot be null or empty");

        var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) throw new Exception("User not found");

        user.LastName = lastName;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async ValueTask<bool> UpdateGender(string userId, string gender, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(gender)) throw new ArgumentException("Gender cannot be null or empty");

        var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) throw new Exception("User not found");

        user.Gender = gender;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async ValueTask<bool> UpdateDateOfBirth(string userId, DateTime dateOfBirth, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) throw new Exception("User not found");

        user.DateOfBirth = dateOfBirth;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async ValueTask<bool> UpdateNickname(string userId, string nickname, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(nickname)) throw new ArgumentException("Nickname cannot be null or empty");

        var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) throw new Exception("User not found");

        user.Nickname = nickname;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async ValueTask<bool> UpdateRelationshipGoals(string userId, string relationshipGoals, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(relationshipGoals)) throw new ArgumentException("Relationship goals cannot be null or empty");

        var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) throw new Exception("User not found");

        user.RelationshipGoals = relationshipGoals;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async ValueTask<bool> UpdateDistancePreference(string userId, double distancePreference, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) throw new Exception("User not found");

        user.DistancePreference = distancePreference;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async ValueTask<bool> UpdateLocation(string userId, string location, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(location)) throw new ArgumentException("Location cannot be null or empty");

        var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) throw new Exception("User not found");

        user.Location = location;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
