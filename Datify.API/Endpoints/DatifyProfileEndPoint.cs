using Datify.API.Contracts;
using Datify.API.Services;
using Datify.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Datify.API.Endpoints
{
    public sealed class DatifyProfileEndPoint(IDatifyProfileService datifyProfileService) : IEndpoints
    {
        public void Register(IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/profiles").WithTags("Profiles").RequireAuthorization();
            group.MapGet("/{id}", GetUserProfileById);
            group.MapPost("/", AddProfile);
            group.MapPut("/{id}", EditProfile);
            group.MapDelete("/{id}", DeleteProfile);
        }


        private async Task<IResult> AddProfile([FromForm]DatifyProfileDto profileDto)
        {
            var addProfile = await datifyProfileService.AddProfile(profileDto);
            return Results.Ok();
        }


        private async Task<IResult> EditProfile([FromForm]DatifyProfileDto profileDto, string id)
        {
            var editProfile = await datifyProfileService.EditProfile(profileDto, id);            
            return Results.Ok();
        }


        private async Task<IResult> DeleteProfile(string id)
        {           
            var deleteProfile = await datifyProfileService.DeleteProfile(id);
            return Results.Ok();
        }

        private async Task<IResult> GetUserProfileById(string id)
        {
            var userId = await datifyProfileService.GetUserProfileById(id);
            if(userId is null)
            {
                return Results.NotFound("user not found");
            }
            return Results.Ok();
 
        }

    }


}