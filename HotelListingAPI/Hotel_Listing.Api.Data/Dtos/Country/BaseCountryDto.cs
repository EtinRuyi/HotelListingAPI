using System.ComponentModel.DataAnnotations;

namespace Hotel_Listing.api.Dtos.Country
{
    public abstract class BaseCountryDto
    {
        [Required]
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}
