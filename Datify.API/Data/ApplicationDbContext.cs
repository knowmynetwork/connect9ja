using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Datify.API.Data;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyDocument> PropertyDocuments { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<PropertyBooking> PropertyBookings { get; set; }
    public DbSet<FlutterWaveVerificationResponse> FlutterWaveVerificationResponses { get; set; }
    public DbSet<OtpVerification> OtpVerifications { get; set; }
}