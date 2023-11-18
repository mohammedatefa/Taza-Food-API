using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;
using TazaFood_Core.IRepositories;
using TazaFood_Core.Models;
using TazaFood_Repository.SpecificationEvaluter;

namespace TazaFood_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _reviewRepo;
        public ReviewController(IUnitOfWork ReviewRepo) 
        {
            _reviewRepo = ReviewRepo;
        }

        [HttpPost("AddReview")]
        public async Task<ActionResult<Review>> AddReview(string content)
        {
            var email = User.FindFirstValue(ClaimTypes.Email.Split("@")[0]);
            var userName = "";

            if (string.IsNullOrEmpty(email))
            {
                userName = "Unknown";
            }
            else
            {
                userName = email;
            }

            var review = new Review();
            review.UserName = userName;
            review.Content = content;
            var newreview = _reviewRepo.Repository<Review>().Add(review);
            await _reviewRepo.complete();

            return Ok(review);
        }


        [HttpGet("GetReviews")]
        [Authorize]
        public async Task<ActionResult<Review>> GetReviews()
        {
            var reviews = await _reviewRepo.Repository<Review>().GetAll();
            return Ok(reviews);
        }

    }
}
