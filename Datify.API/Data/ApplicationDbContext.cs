using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Datify.API.Data;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<DatifyProfile> Profiles { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<FlutterWaveVerificationResponse> FlutterWaveVerificationResponses { get; set; }
    public DbSet<OtpVerification> OtpVerifications { get; set; }
}