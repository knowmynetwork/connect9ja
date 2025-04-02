using Datify.Shared.Models.Enum;
using Datify.Shared.Models;

namespace Datify.API.Data
{
    public class OtpVerification : BaseProperties
    {
        public string ContactNumberOrEmailValue { get; set; } = default!;
        public ContactType ContactType { get; set; }
        public string OtpCode { get; set; } = default!;
        public DateTime ExpirationTime { get; set; }
        public bool IsVerified { get; set; }
        public DateTime DateVerified { get; set; } = DateTime.UtcNow;
    }
}
