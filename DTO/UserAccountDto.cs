using TazaFood_Core.IdentityModels;

namespace TazaFood_API.DTO
{
    public class UserAccountDto
    {
        public string DisplayName {  get; set; }
        public string Email { get; set; }
        public string  Token { get; set; }
    }
}
