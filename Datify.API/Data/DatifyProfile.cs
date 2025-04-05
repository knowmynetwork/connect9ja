using Datify.Shared.Models;

namespace Datify.API.Data
{
    public class DatifyProfile : BaseProperties
    {

        public string UserId { get; set; }
        public string NickName { get; set; }
        public string Bio { get; set; }
        public string Hobbies { get; set; }
        public string ProfilePicture { get; set; }
        public string CoverPhoto { get; set; }
        public string Featured { get; set; }
        public string Work { get; set; }
        public string Education { get; set; }
        public string Location { get; set; }
        public string RelationshipStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
