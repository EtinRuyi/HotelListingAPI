using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hotel_Listing.api.Data;
using Hotel_Listing.api.Dtos.Country;
using Hotel_Listing.api.Exceptions;
using Hotel_Listing.api.Models;
using Hotel_Listing.api.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Listing.api.Services.Repository
{
    public class CountryRepo : GenericRepository<Country>, ICountryRepo
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        public CountryRepo(HotelListingDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            this._mapper = mapper;
        }

        public async Task<CountryDto> GetDetails(int id)
        {
            var country =  await _context.Countries.Include(d => d.Hotels)
                .ProjectTo<CountryDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(d => d.Id == id);

            if(country == null)
            {
                throw new NotFoundException(nameof(GetDetails), id);
            }

            return country;
        }
    }
}
