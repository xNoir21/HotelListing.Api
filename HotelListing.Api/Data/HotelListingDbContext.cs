using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Data;

public class HotelListingDbContext(DbContextOptions<HotelListingDbContext> options) : DbContext(options)
{
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }
    
}

