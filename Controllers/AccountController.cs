using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TazaFood_API.DTO;
using TazaFood_API.Extenssions;
using TazaFood_Core.IdentityModels;
using TazaFood_Core.Services;

namespace TazaFood_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper autoMapper;

        public AccountController(
            UserManager<AppUser> UserManager,
            SignInManager<AppUser> SignInManager,
            ITokenService TokenService,
            IMapper AutoMapper
            )
        {
            this.userManager = UserManager;
            this.signInManager = SignInManager;
            this.tokenService = TokenService;
            this.autoMapper = AutoMapper;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserAccountDto>> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user is null) return Unauthorized();
                var resualt = await signInManager.CheckPasswordSignInAsync(user,model.Password,false);
                if (!resualt.Succeeded) return Unauthorized();

                string Messege = "Succeeded";
                var account = new UserAccountDto()
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Token=await tokenService.CreateTokenAsync(user,userManager)
                };

                return Ok(new
                {
                    messege = Messege,
                    user = account

                });

            }
            return BadRequest("invalid data...");
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserAccountDto>> Register([FromForm]Register model)
        {
            if (ModelState.IsValid)
            {
                //first check if the user already exist
                var userExisted = await userManager.FindByEmailAsync(model.Email);
                if (userExisted is not null) return Ok("there is already account with this email...");

                var newuser = new AppUser()
                {
                    DisplayName = model.FirstName + "_" + model.LastName,
                    UserName = model.Email.Split("@")[0],
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    Address = new Address() { City=model.City,Country=model.Country,Street=model.Street}
                };

                var resualt=await userManager.CreateAsync(newuser, model.Password);
                if (!resualt.Succeeded)
                {
                    foreach (var error in resualt.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return BadRequest(ModelState);
                }
                string Messege = "Added Succeeded";
                var useraccount = new UserAccountDto
                {
                    DisplayName = newuser.DisplayName,
                    Email = newuser.Email,
                    Token = await tokenService.CreateTokenAsync(newuser,userManager)
                };
                return Ok(new
                {
                  messege=Messege,
                  user = useraccount
                });

            }
            return BadRequest("data is invalid check your data");
        }
        
        [Authorize]
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IReadOnlyList<UserAccountDto>>> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();

            var userList = users.Select(user => new UserAccountDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                
                
            }).ToList();

            return Ok(userList);
        }

        [Authorize]
        [HttpGet("GetUser")]
        public async Task<ActionResult<UserAccountDto>> GetUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var currentuser=await userManager.FindByEmailAsync(email);
            return Ok(new UserAccountDto()
            {
                DisplayName = currentuser.DisplayName,
                Email=currentuser.Email,
                Token= await tokenService.CreateTokenAsync(currentuser,userManager)

            });
        }

        [Authorize]
        [HttpGet("GetUserAddress")]
        public async Task<ActionResult<UserAddressDto>> GetUserAddress()
        {

            var user = await userManager.GetUserAddressByEmail(User);
            var adress =  autoMapper.Map<Address, UserAddressDto>(user.Address);
            return Ok(adress);
        }

        [Authorize]
        [HttpPut("UpdateUserAdress")]
        public async Task<ActionResult<UserAddressDto>> UpdateUserAdress(UserAddressDto updatedAddres)
        {
            var address = autoMapper.Map<UserAddressDto, Address>(updatedAddres);
            var user =await userManager.GetUserAddressByEmail(User);

            address.Id = user.Address.Id;

            user.Address = address;
            var resualt = await userManager.UpdateAsync(user);
            if (!resualt.Succeeded) return BadRequest("can't updated user adress");
            return Ok(updatedAddres);

        }
    }
}
