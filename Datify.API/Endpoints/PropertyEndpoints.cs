using System.Security.Claims;
using System.Text.Json;
using Datify.API.Contracts;
using Datify.API.Services;
using Microsoft.AspNetCore.Mvc;
using Datify.Shared.Models;
using Datify.Shared.Utilities;
using Microsoft.AspNetCore.Antiforgery;

namespace Datify.API.Endpoints;

[IgnoreAntiforgeryToken] 

public sealed class PropertyEndpoints(IPropertyService propertyService) : IEndpoints
{
    public void Register(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/properties").WithTags("Properties").RequireAuthorization();

        group.MapGet("/getallproperties", GetProperties).AllowAnonymous();
        group.MapPost("/search", SearchProperties).AllowAnonymous();
        group.MapGet("/{id:int}", GetProperty).AllowAnonymous();
        group.MapGet("/booking/history/{id}", GetBookingUserList).AllowAnonymous();
        group.MapGet("/user/{userId}", GetPropertiesByUserId);
        group.MapPost("/create-property/{id:int}", CreateProperty).DisableAntiforgery();
        group.MapPost("/save-property-booking", SavePropertyBooking);
        group.MapPost("/verify-property-transaction", VerifyPropertyTransaction);
        group.MapPut("/{id:int}", UpdateProperty);
        group.MapDelete("/{id:int}", DeleteProperty);
        group.MapGet("/test-token", TestToken);
    }

    private async Task<IResult> GetProperties(CancellationToken cancellationToken)
    {
        var items = await propertyService.GetPropertiesAsync();
        return Results.Ok(Response.CreateSuccessResult(items, "Properties retrieved successfully"));
    }
    
    private async Task<IResult> GetBookingUserList(string id, CancellationToken cancellationToken)
    {
        var bookingList = await propertyService.GetBookingListByUserIdAsync(id);
        if (bookingList?.Count == 0) return Results.NotFound(Response.CreateFailureResult<bool>("No Booking"));
        return Results.Ok(Response.CreateSuccessResult(bookingList, "Booking History"));
    }

    private async Task<IResult> SearchProperties([FromBody] SearchPropertyDto propertyDto, CancellationToken cancellationToken)
    {
        var items = await propertyService.SearchPropertiesAsync(propertyDto);
        return Results.Ok(Response.CreateSuccessResult(items, "Search results retrieved successfully"));
    }

    private async Task<IResult> GetProperty(int id, CancellationToken cancellationToken)
    {
        var item = await propertyService.GetPropertyByIdAsync(id);
        if (item?.Id == 0) return Results.NotFound(Response.CreateFailureResult<bool>("Property not found"));
        return Results.Ok(Response.CreateSuccessResult(item, "Property retrieved successfully"));
    }

    private async Task<IResult> GetPropertiesByUserId(string userId, CancellationToken cancellationToken)
    {
        var items = await propertyService.GetPropertiesByUserIdAsync(userId);
        if (!items.Any())
        {
            return Results.NotFound(Response.CreateFailureResult<bool>("No properties found for this user"));
        }
        return Results.Ok(Response.CreateSuccessResult(items, "User's properties retrieved successfully"));
    }

    private async Task<IResult> SavePropertyBooking(
        [FromBody] BookingCreationDto model, CancellationToken cancellationToken)
    {
        var result = await propertyService.SavePropertyBooking(model, cancellationToken);
        return Results.Ok(Response.CreateSuccessResult(result, "Booking created successfully"));
    }
    
    private async Task<IResult> VerifyPropertyTransaction(
        [FromBody] VerifyTransactionDto model, CancellationToken cancellationToken)
    {
        var result = await propertyService.VerifyPropertyPayment(model, cancellationToken);
        return Results.Ok(Response.CreateSuccessResult(result, "Payment for Booking verified successfully"));
    }

    private async Task<IResult> CreateProperty(
        [FromForm] string model,
        [FromForm] IFormFileCollection files,
        int id,
        ClaimsPrincipal user)
    {
        var jsonType = JsonSerializer.Deserialize<CreatePropertyDto>(model) ?? new CreatePropertyDto();
    
        jsonType.DocumentsToUpload = files.ToList();
    
        await propertyService.CreateOrUpdatePropertyAsync(jsonType, id);

        return Results.Ok(Response.CreateSuccessResult("Property created successfully"));
    }

    private async Task<IResult> UpdateProperty(int id, PropertyDto propertyDto, CancellationToken cancellationToken)
    {
        var updatedItem = await propertyService.UpdatePropertyAsync(id, propertyDto);
        if (updatedItem?.Id == 0) return Results.NotFound(Response.CreateFailureResult<bool>("Property not found"));
        return Results.Ok(Response.CreateSuccessResult(updatedItem, "Property updated successfully"));
    }

    private async Task<IResult> DeleteProperty(int id, CancellationToken cancellationToken)
    {
        var result = await propertyService.DeletePropertyAsync(id);
        if (!result) return Results.NotFound(Response.CreateFailureResult<bool>("Failed to delete property"));
        return Results.Ok(Response.CreateSuccessResult(true, "Property deleted successfully"));
    }

    private IResult TestToken(HttpContext httpContext)
    {
        var headers = httpContext.Request.Headers.Select(h => $"{h.Key}: {h.Value}").ToList();
        Console.WriteLine("🔹 Request Headers: " + string.Join(", ", headers));

        var userClaims = httpContext.User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
        Console.WriteLine("🔹 User Claims: " + string.Join(", ", userClaims));

        Console.WriteLine("🔹 Checking User Authentication...");
        Console.WriteLine($"🔹 IsAuthenticated: {httpContext.User.Identity?.IsAuthenticated}");

        return Results.Ok(Response.CreateSuccessResult("TestToken", "Token validated successfully"));
    }
}