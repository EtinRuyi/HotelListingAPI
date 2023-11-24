using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Listing.api.Dtos.Country
{
    public class GetCountryDto : BaseCountryDto
    {
        public int Id { get; set; }
    }
}
