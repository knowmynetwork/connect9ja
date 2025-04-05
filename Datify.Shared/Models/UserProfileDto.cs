using Microsoft.AspNetCore.Http;

namespace Datify.Shared.Models;

public class UserProfileDto
{
    public string? NickName { get; set; }
    public string? Bio { get; set; }
    public String? Hobies { get; set; }
    public String? Location { get; set; }
    public IFormFile? ProfilePicture { get; set; }
  }