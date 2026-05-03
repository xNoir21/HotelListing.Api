using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Api.Domain.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKeys>
{
    public void Configure(EntityTypeBuilder<ApiKeys> builder)
    {
        builder.HasIndex(e => e.KeyId).IsUnique();
        builder.HasData(
            //Just an example
            new ApiKeys
            {
                Id = new Guid("a4d3988c-5b82-40ce-8aca-d90d590b1387"),
                AppName = "app",
                CreatedOnUtc = DateTime.UtcNow,
                KeyId = "ba0bd8fd6a88",
                KeyHash = "36F27D144462317E37C5F364A9657A667A76C56C38896EF2E70031391A69B2B2"
            }
        );
    }
}