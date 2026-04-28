using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Api.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "a4d3988c-5b82-40ce-8aca-d90d590b1387",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "a740b281-d7d6-458f-9c25-8294e7de3fa6",
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole
            {
                Id = "a740b281-5b82-458f-9c25-d90d590b1387",
                Name = "Hotel Admin",
                NormalizedName = "HOTEL ADMIN"
            }
        );
    }
}