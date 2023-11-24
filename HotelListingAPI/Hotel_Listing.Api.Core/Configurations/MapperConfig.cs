using AutoMapper;
using Hotel_Listing.api.Dtos;
using Hotel_Listing.api.Dtos.Country;
using Hotel_Listing.api.Dtos.Hotel;
using Hotel_Listing.api.Dtos.Users;
using Hotel_Listing.api.Models;

namespace Hotel_Listing.api.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Country, UpdateCountryDto>().ReverseMap();

            CreateMap<Hotel, HotelDto>().ReverseMap();
            CreateMap<Hotel, CreateHotelDto>().ReverseMap();

            CreateMap<ApiUserDto, ApiUser>().ReverseMap();
        }
    } 
}
