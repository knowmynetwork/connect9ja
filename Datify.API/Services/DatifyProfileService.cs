using AutoMapper;
using Datify.API.Data;
using Datify.Shared.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Datify.API.Services
{
    public class DatifyProfileService : IDatifyProfileService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        public DatifyProfileService(ApplicationDbContext applicationDbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            dbContext = applicationDbContext;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(bool Success, string Message)> AddProfile(DatifyProfileDto profileDto)
        {
            var profileEntity = _mapper.Map<DatifyProfile>(profileDto);

            // Handle Profile Picture Upload
            if (profileDto.ProfilePicture != null)
            {
                profileEntity.ProfilePicture = await SaveFileAsync(profileDto.ProfilePicture, "profile_pictures");
            }

            // Handle Cover Photo Upload
            if (profileDto.CoverPhoto != null)
            {
                profileEntity.CoverPhoto = await SaveFileAsync(profileDto.CoverPhoto, "cover_photos");
            }

            await dbContext.AddAsync(profileEntity);
            var isSaved = await dbContext.SaveChangesAsync();
            if (isSaved > 0)
            {
                return (true, "Profile created successfuly");
            }
            return (false, "Profile not successfuly created");

        }

        public async Task<(bool Success, string Message)> DeleteProfile(string id)
        {
            var profileEntity = await dbContext.Profiles.FindAsync(id);
            if (profileEntity == null)
            {
                return (false, "Profile can not be found");
            }
            dbContext.Remove(profileEntity);
            await dbContext.SaveChangesAsync();
            return (true, "Profile deleted successfully");
        }

        public async Task<(bool Success, string Message)> EditProfile(DatifyProfileDto profileDto, string id)
        {
            var profiileEntity = await dbContext.Profiles.FindAsync(id);
            if (profiileEntity == null)
            {
                return (false, "Profile can not be found");
            }
            // map text fields only
            _mapper.Map(profileDto, profiileEntity);

            //  Handle Profile Picture Upload
            if (profileDto.ProfilePicture != null)
            {
                profiileEntity.ProfilePicture = await SaveFileAsync(profileDto.ProfilePicture, "profile_pictures");
            }
            //  Handle Cover Photo Upload
            if (profileDto.CoverPhoto != null)
            {
                profiileEntity.CoverPhoto = await SaveFileAsync(profileDto.CoverPhoto, "cover_photos");
            }
            // Save changes to the database
            dbContext.Profiles.Update(profiileEntity);
            await dbContext.SaveChangesAsync();
            return (true, "profile updated");

        }

        public async Task<DatifyProfileDto> GetUserProfileById(string id)
        {
            var profileEntity = await dbContext.Profiles.FindAsync(id);
            if (profileEntity == null)
            {
                return null;
            }
            // map model to dto
            var mappedProfile = _mapper.Map<DatifyProfileDto>(profileEntity);
            return mappedProfile;
        }  



        // Helper method to handle file uploads
        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folderName);
            Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folderName}/{uniqueFileName}";
        }
    }
}
