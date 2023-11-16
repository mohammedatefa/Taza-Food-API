using System.ComponentModel.DataAnnotations;

namespace TazaFood_API.DTO
{
    public class Register
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string  LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

      
    }
}
