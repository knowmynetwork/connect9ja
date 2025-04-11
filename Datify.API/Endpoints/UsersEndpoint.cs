using System.Security.Claims;
using System.Text.Encodings.Web;
using Datify.API.Contracts;
using Datify.API.Data;
using Datify.API.Services;
using Datify.Shared.Models;
using Datify.Shared.Models.Enum;
using Datify.Shared.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Datify.API.Endpoints;

public sealed class UsersEndpoint(IUserService service, IOtpService otpService, IEmailService emailService)
    : IEndpoints
{
    public void Register(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/users").WithTags("Users").RequireAuthorization();
        group.MapGet("/{id}", GetUser);
        group.MapGet("/", GetAllUsers);
        group.MapGet("/claims", GetUserClaims);
        group.MapGet("/logout", LogoutUser);

        group.MapPost("/", CreateUser);
        group.MapPost("/sendOtp", SendOtp);
        group.MapPost("/verifyotp", VerifyUserOtp);
        group.MapPost("/forgotPassword", ForgotPassword);
        group.MapPost("/resetPassword", ResetPassword);

        group.MapPut("/{id}", UpdateUser);
        group.MapPut("/changePassword/{id}", UpdatePassword);

        group.MapDelete("/{id}", DeleteUser);

        // ✅ Authentication Endpoints
        group.MapPost("/auth/login", LoginUser).AllowAnonymous();
        group.MapPost("/auth/register", RegisterUser).AllowAnonymous();
        group.MapGet("/confirm-email", ConfirmEmail).AllowAnonymous();

        group.MapPut("/{id}/firstName", UpdateFirstName);
        group.MapPut("/{id}/lastName", UpdateLastName);
        group.MapPut("/{id}/gender", UpdateGender);
        group.MapPut("/{id}/dateOfBirth", UpdateDateOfBirth);
        group.MapPut("/{id}/nickname", UpdateNickname);
        group.MapPut("/{id}/relationshipGoals", UpdateRelationshipGoals);
        group.MapPut("/{id}/distancePreference", UpdateDistancePreference);
        group.MapPut("/{id}/location", UpdateLocation);

        group.MapGet("/{id}/profile", GetUserProfile);
    }

    private async Task<IResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await service.GetAllUsers(cancellationToken);
        return users.Count == 0 ? Results.NotFound(Response.CreateFailureResult<List<UserProfileDto>>("No users found")) : Results.Ok(Response.CreateSuccessResult(users, "Users retrieved successfully"));
    }

    private async Task<IResult> GetUser(string id, CancellationToken cancellationToken)
    {
        var user = await service.GetById(id, cancellationToken);
        return user is null ? Results.NotFound(Response.CreateFailureResult<bool>("User not found")) : Results.Ok(Response.CreateSuccessResult(user, "User retrieved successfully"));
    }

    private async Task<IResult> GetUserClaims(ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        var claims = await service.GetUserClaims(user, cancellationToken);
        return string.IsNullOrEmpty(claims) ? Results.Unauthorized() : Results.Ok(Response.CreateSuccessResult(claims, "Claims retrieved successfully"));
    }

    private static async Task<IResult> LogoutUser(SignInManager<ApplicationUser> signInManager, CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();
        return Results.Ok(Response.CreateSuccessResult(true, "User logged out successfully"));
    }

    private static async Task CreateUser(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    private async Task<IResult> UpdateUser(string id, UserDto model, CancellationToken cancellationToken)
    {
        var result = await service.UpdateById(id, model, cancellationToken);
        return !result ? Results.BadRequest(Response.CreateFailureResult("Failed to update user")) : Results.Ok(Response.CreateSuccessResult(result, "User updated successfully"));
    }

    private static async Task<IResult> UpdatePassword(string id, ChangePasswordRequestDto request, ApplicationDbContext adbc, UserManager<ApplicationUser> userManager, CancellationToken cancellationToken)
    {
        var user = await adbc.Users.FindAsync([id], cancellationToken: cancellationToken);
        if (user is null) return Results.NotFound(Response.CreateFailureResult("User not found"));

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        return result.Errors.Any() ? Results.Problem(Response.CreateFailureResult(result.Errors.First().Description).Message, statusCode: 400, title: "Change password failed") : Results.Ok(Response.CreateSuccessResult(true, "Password updated successfully"));
    }
    
    private async Task<IResult> ForgotPassword([FromBody] ForgotPasswordRequest request, [FromServices] UserManager<ApplicationUser> userManager)
    {
        if (string.IsNullOrEmpty(request.Email))
            return Results.BadRequest(new { message = "Email is required." });

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Results.NotFound(new { message = "User not found." });

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"https://localhost:7035/api/users/\r\n{user.Email}&token={Uri.EscapeDataString(token)}";

        if (user.Email != null)
            await emailService.SendEmailAsync(user.Email, "Reset Your Password",
                $"Click <a href='{HtmlEncoder.Default.Encode(resetLink)}'>here</a> to reset your password.");

        return Results.Ok(Response.CreateSuccessResult(true, "Password reset link has been sent to your email."));
    }


    private static async Task<IResult> ResetPassword([FromBody] ResetPasswordRequestDto request, [FromServices] UserManager<ApplicationUser> userManager)
    {
        if (string.IsNullOrEmpty(request.Email) ||
            string.IsNullOrEmpty(request.ResetToken) ||
            string.IsNullOrEmpty(request.NewPassword))
        {
            return Results.BadRequest(new { message = "Email, token, and new password are required." });
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Results.NotFound(new { message = "User not found." });
        }

        var resetPassResult = await userManager.ResetPasswordAsync(user, request.ResetToken, request.NewPassword);

        if (resetPassResult.Succeeded)
            return Results.Ok(Response.CreateSuccessResult(true, "Password reset successfully!"));
        var errors = string.Join(", ", resetPassResult.Errors.Select(e => e.Description));
        return Results.BadRequest(new { message = $"Password reset failed: {errors}" });

    }

    private async Task<IResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteById(id, cancellationToken);
        return !result ? Results.BadRequest(Response.CreateFailureResult("Failed to delete user")) : Results.Ok(Response.CreateSuccessResult(result, "User deleted successfully"));
    }

    private static async Task<IResult> LoginUser(
        [FromBody] LoginRequestDto model,
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] SignInManager<ApplicationUser> signInManager,
        [FromServices] IConfiguration config,
        [FromServices] IUserService userService,
        HttpContext httpContext)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user is null) return Results.BadRequest(Response.CreateFailureResult("Invalid email or password"));

        if (!user.EmailConfirmed)
        {
            throw new Exception("Email is not confirmed. Please verify your email before logging in.");
        }

        var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (!result.Succeeded) return Results.BadRequest(Response.CreateFailureResult("Invalid login attempt"));

        // ✅ Retrieve user roles
        var roles = await userManager.GetRolesAsync(user);

        var jwtToken = userService.GenerateJwtToken(user, roles, httpContext, config);

        var response = new LoginResponseDto("Bearer", jwtToken, 3600, "refresh_token_placeholder")
        {
            Claims = ""
        };

        return Results.Ok(Response.CreateSuccessResult(response, "Login successful"));
    }

    private async Task<IResult> RegisterUser([FromBody] RegisterModelDto model, [FromServices] UserManager<ApplicationUser> userManager, HttpContext httpContext, [FromServices] IConfiguration config)
    {
        var userProfile = await service.RegisterUser(model, userManager, httpContext, config);
        return Results.Ok(Response.CreateSuccessResult(userProfile, "User registered successfully"));
    }

    private static async Task<IResult> ConfirmEmail(
        [FromQuery] string userId,
        [FromQuery] string token,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Results.BadRequest("Invalid user.");

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            return Results.Content(
                "<html><body><h2>Email confirmed successfully!</h2><p>You can now log in.</p></body></html>",
                "text/html");
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        return Results.BadRequest($"Email confirmation failed: {errors}");

    }

    private async Task<IResult> SendOtp(string userEmail, ContactType contactType)
    {
        // generate otp
        otpService.GenerateOtp();
        // save and send otp
        await otpService.SaveOtpAsync(userEmail, contactType);
        return Results.Ok(Response.CreateSuccessResult(true, "Otp sent successfully"));
    }


    private async Task<IResult> VerifyUserOtp(string userEmail, string verificationOtpCode)
    {
        await otpService.VerifyOtpAsync(userEmail, verificationOtpCode);
        return Results.Ok(Response.CreateSuccessResult(true, "Otp verification is successful"));
    }

    private async Task<IResult> UpdateFirstName(string id, [FromBody] UserProfileDto request, CancellationToken cancellationToken)
    {
        if (request.FirstName != null) await service.UpdateFirstName(id, request.FirstName, cancellationToken);
        return Results.Ok(Response.CreateSuccessResult(true, "First name updated successfully"));
    }

    private async Task<IResult> UpdateLastName(string id, [FromBody] UserProfileDto request, CancellationToken cancellationToken)
    {
        if (request.LastName != null) await service.UpdateLastName(id, request.LastName, cancellationToken);
        return Results.Ok(Response.CreateSuccessResult(true, "Last name updated successfully"));
    }

    private async Task<IResult> UpdateGender(string id, [FromBody] UserProfileDto request, CancellationToken cancellationToken)
    {
        if (request.Gender != null) await service.UpdateGender(id, request.Gender, cancellationToken);
        return Results.Ok(Response.CreateSuccessResult(true, "Gender updated successfully"));
    }

    private async Task<IResult> UpdateDateOfBirth(string id, [FromBody] UserProfileDto request, CancellationToken cancellationToken)
    {
        if (request.DateOfBirth != null)
            await service.UpdateDateOfBirth(id, request.DateOfBirth.Value, cancellationToken);
        return Results.Ok(Response.CreateSuccessResult(true, "Date of birth updated successfully"));
    }

    private async Task<IResult> UpdateNickname(string id, [FromBody] UserProfileDto request, CancellationToken cancellationToken)
    {
        if (request.NickName != null) await service.UpdateNickname(id, request.NickName, cancellationToken);
        return Results.Ok(Response.CreateSuccessResult(true, "Nickname updated successfully"));
    }
    
    private async Task<IResult> UpdateRelationshipGoals(string id, [FromBody] UserProfileDto request, CancellationToken cancellationToken)
    {
        if (request.RelationshipGoals != null)
            await service.UpdateRelationshipGoals(id, request.RelationshipGoals, cancellationToken);
        return Results.Ok(Response.CreateSuccessResult(true, "Relationship goals updated successfully"));
    }

    private async Task<IResult> UpdateDistancePreference(string id, [FromBody] UserProfileDto request, CancellationToken cancellationToken)
    {
        if (request.DistancePreference != null)
            await service.UpdateDistancePreference(id, request.DistancePreference.Value, cancellationToken);
        return Results.Ok(Response.CreateSuccessResult(true, "Distance preference updated successfully"));
    }

    private async Task<IResult> UpdateLocation(string id, [FromBody] UserProfileDto request, CancellationToken cancellationToken)
    {
        if (request.Location != null) await service.UpdateLocation(id, request.Location, cancellationToken);
        return Results.Ok(Response.CreateSuccessResult(true, "Location updated successfully"));
    }

    private async Task<IResult> GetUserProfile(string id, CancellationToken cancellationToken)
    {
        var userProfile = await service.GetUserProfile(id, cancellationToken);
        if (userProfile == null)
            return Results.NotFound(Response.CreateFailureResult<UserProfileDto>("User not found"));

        return Results.Ok(Response.CreateSuccessResult(userProfile, "User profile retrieved successfully"));
    }
}