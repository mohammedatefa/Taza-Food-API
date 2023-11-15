using System.ComponentModel.DataAnnotations;

namespace TazaFood_API.DTO
{
    public class Login
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
