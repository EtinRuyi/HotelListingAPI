using Hotel_Listing.api.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Listing.api.Data.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "San Hoeshi francuas",
                    Address = "San teigo",
                    CountryId = 1,
                    Rating = 4.5
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Confideration Le Chalee",
                    Address = "Chile",
                    CountryId = 3,
                    Rating = 4.2
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Lumberato Es Satao",
                    Address = "Spain",
                    CountryId = 2,
                    Rating = 4
                }
            );
        }
    }
}
