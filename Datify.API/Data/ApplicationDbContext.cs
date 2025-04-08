using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Datify.API.Data;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<DatifyProfile> Profiles { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<FlutterWaveVerificationResponse> FlutterWaveVerificationResponses { get; set; }
    public DbSet<OtpVerification> OtpVerifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.Value.ToUniversalTime(), // Convert to UTC before saving
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // Convert to UTC when reading;

        });
        base.OnModelCreating(modelBuilder);
    }
}