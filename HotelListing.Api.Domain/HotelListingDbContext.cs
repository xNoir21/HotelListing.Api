using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Domain;

public class HotelListingDbContext(DbContextOptions<HotelListingDbContext> options) :
    IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<ApiKeys> ApiKeys { get; set; }
    public DbSet<HotelAdmin> HotelAdmins { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Country>()
            .HasIndex(c => c.Name);

        modelBuilder.Entity<Country>()
            .HasIndex(c => c.ShortName);

        modelBuilder.Entity<Hotel>()
            .HasIndex(h => h.Name);

        modelBuilder.Entity<Hotel>()
            .HasIndex(h => h.CountryId);

        modelBuilder.Entity<Hotel>()
            .HasIndex(h => new { h.CountryId, h.Rating });
    }
}