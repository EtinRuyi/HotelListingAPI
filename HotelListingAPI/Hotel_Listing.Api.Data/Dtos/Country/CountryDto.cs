using Hotel_Listing.api.Dtos.Hotel;

namespace Hotel_Listing.api.Dtos.Country
{
    public class CountryDto : BaseCountryDto
    {
        public int Id { get; set; }
        public IList<HotelDto> Hotels { get; set; }
    }
}
