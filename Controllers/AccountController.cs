using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TazaFood_API.DTO;
using TazaFood_API.Extenssions;
using TazaFood_Core.IdentityModels;
using TazaFood_Core.Models;
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
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<UserAccountDto>> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user is null) return Unauthorized("There Is No Account With This Email.");

                var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!result.Succeeded) return Unauthorized("The Password Is Incorrect.");

                string message = "Succeeded";
                var account = new UserAccountDto()
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Token = await tokenService.CreateTokenAsync(user, userManager)
                };

                // Check user roles
                var roles = await userManager.GetRolesAsync(user);
                string accountRole = roles.Contains("Admin") ? "Admin" : "User";

                return Ok(new
                {
                    Message = message,
                    user = account,
                    AccountRole = accountRole
                });
            }
            return BadRequest("Invalid data...");
        }


        [HttpPost("Register")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<UserAccountDto>> Register([FromForm] Register model)
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
                    Address = new Address() { City = model.City, Country = model.Country, Street = model.Street }
                };

                var resualt = await userManager.CreateAsync(newuser, model.Password);
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
                    Token = await tokenService.CreateTokenAsync(newuser, userManager)
                };
                return Ok(new
                {
                    messege = Messege,
                    user = useraccount
                });

            }
            return BadRequest("data is invalid check your data");
        }

        //[Authorize (Roles = "Admin")]
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

        [Authorize (Roles = "Admin")]
        [HttpGet("GetUser")]
        public async Task<ActionResult<UserAccountDto>> GetUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var currentuser = await userManager.FindByEmailAsync(email);


            return Ok(new UserAccountDto()
            {
                DisplayName = currentuser.DisplayName,
                Email = currentuser.Email,
                Token = await tokenService.CreateTokenAsync(currentuser, userManager)

            });
        }

        [Authorize]
        [HttpGet("GetUserAddress")]
        public async Task<ActionResult<UserAddressDto>> GetUserAddress()
        {
            var user = await userManager.GetUserAddressByEmail(User);
            var adress = autoMapper.Map<Address, UserAddressDto>(user.Address);
            return Ok(adress);
        }

        [Authorize]
        [HttpPut("UpdateUserAdress")]
        public async Task<ActionResult<UserAddressDto>> UpdateUserAdress(UserAddressDto updatedAddres)
        {
            var address = autoMapper.Map<UserAddressDto, Address>(updatedAddres);
            var user = await userManager.GetUserAddressByEmail(User);

            address.Id = user.Address.Id;

            user.Address = address;
            var resualt = await userManager.UpdateAsync(user);
            if (!resualt.Succeeded) return BadRequest("can't updated user adress");
            return Ok(updatedAddres);

        }

        [HttpPost("AddAdmin")]
       /* [Authorize(Roles = "Admin")] */ // admin@taza.com   admin123
        public async Task<ActionResult<UserAccountDto>> AddAdmin([FromForm] Register model)
        {
            if (ModelState.IsValid)
            {
                //first check if the user already exist
                var adminExisted = await userManager.FindByEmailAsync(model.Email);
                if (adminExisted is not null) return Ok("there is already account with this email...");

                var newAdmin = new AppUser()
                {
                    DisplayName = model.FirstName + "_" + model.LastName,
                    UserName = model.Email.Split("@")[0],
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    Address = new Address() { City = model.City, Country = model.Country, Street = model.Street }
                };

                var resualt = await userManager.CreateAsync(newAdmin, model.Password);
                if (!resualt.Succeeded)
                {
                    foreach (var error in resualt.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return BadRequest(ModelState);
                }
                string Messege = "Added Succeeded";

                await userManager.AddToRoleAsync(newAdmin, "Admin");

                var adminAccount = new UserAccountDto
                {
                    DisplayName = newAdmin.DisplayName,
                    Email = newAdmin.Email,
                    Token = await tokenService.CreateTokenAsync(newAdmin, userManager)
                };
                return Ok(new
                {
                    messege = Messege,
                    user = adminAccount
                });

            }
            return BadRequest("data is invalid check your data");
        }


        [HttpPut("UpdateAccount")]
        [Authorize]
        public async Task<ActionResult<UserAccountDto>> PutAccount(string id , [FromForm] Register updatedAccountmodel)
        {

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id);
                if (user is null)
                {
                    return NotFound("User not found");
                }


                user.DisplayName = updatedAccountmodel.FirstName + "_" + updatedAccountmodel.LastName;
                user.UserName = updatedAccountmodel.Email.Split("@")[0];
                user.PhoneNumber = updatedAccountmodel.PhoneNumber;
                user.Email = updatedAccountmodel.Email;

                var result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return BadRequest(ModelState);
                }

                string Messege = "Updated Succeeded";
                var updatedAccount = new UserAccountDto
                {
                    DisplayName = user.DisplayName,
                    Email = updatedAccountmodel.Email,
                    Token = await tokenService.CreateTokenAsync(user, userManager)
                };
                return Ok(new
                {
                    messege = Messege,
                    user = updatedAccount
                });
            }
            return BadRequest("data is invalid check your data");
        }


        [HttpDelete("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound("User not found");
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            return StatusCode(StatusCodes.Status204NoContent, "Deleted Successed");
        }




    }   
}
