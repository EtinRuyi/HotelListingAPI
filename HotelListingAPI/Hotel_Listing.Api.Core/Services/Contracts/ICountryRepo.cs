using Hotel_Listing.api.Dtos.Country;
using Hotel_Listing.api.Models;

namespace Hotel_Listing.api.Services.Contracts
{
    public interface ICountryRepo : IGenericRepository<Country>
    {
        Task<CountryDto> GetDetails(int id);
    }
}
