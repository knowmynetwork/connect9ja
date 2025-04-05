using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datify.Shared.Models
{
   public class DatifyProfileDto
    {
        public string UserId { get; set; }
        public string NickName { get; set; }
        public string Bio { get; set; }
        public string Hobbies { get; set; }
        public IFormFile ProfilePicture { get; set; }
        public IFormFile CoverPhoto { get; set; }
        public string Featured { get; set; }
        public string Work { get; set; }
        public string Education { get; set; }
        public string Location { get; set; }
        public string RelationshipStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
