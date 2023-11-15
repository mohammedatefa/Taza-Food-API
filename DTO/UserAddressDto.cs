using System.ComponentModel.DataAnnotations;

namespace TazaFood_API.DTO
{
    public class UserAddressDto
    {
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
    }
}
