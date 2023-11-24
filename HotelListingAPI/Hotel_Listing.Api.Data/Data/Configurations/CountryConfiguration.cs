using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Hotel_Listing.api.Models;

namespace Hotel_Listing.api.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
                new Country
                {
                    Id = 1,
                    Name = "Jamaica",
                    ShortName = "JM",
                },
                new Country
                {
                    Id = 2,
                    Name = "Nigeria",
                    ShortName = "NG",
                }, new Country
                {
                    Id = 3,
                    Name = "Egypt",
                    ShortName = "EG",
                }, new Country
                {
                    Id = 4,
                    Name = "Cameron",
                    ShortName = "CM",
                }
            );
        }
    }
}
