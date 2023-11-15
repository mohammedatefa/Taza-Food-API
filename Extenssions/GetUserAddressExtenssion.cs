using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TazaFood_Core.IdentityModels;

namespace TazaFood_API.Extenssions
{
    public static class GetUserAddressExtenssion
    {
        public static async Task<AppUser> GetUserAddressByEmail(this UserManager<AppUser> userManger,ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManger.Users.Include(U => U.Address).FirstOrDefaultAsync(U => U.Email == email);
            return user;
        }
    }
}
