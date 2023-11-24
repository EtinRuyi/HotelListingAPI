using AutoMapper;
using Hotel_Listing.api.Data;
using Hotel_Listing.api.Models;
using Hotel_Listing.api.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Listing.api.Services.Repository
{
    public class HotelRepo : GenericRepository<Hotel>, IHotelRepo
    {
        public HotelRepo(HotelListingDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
