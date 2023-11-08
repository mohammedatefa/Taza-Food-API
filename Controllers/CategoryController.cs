using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TazaFood_Core.IRepositories;
using TazaFood_Core.ISpecifications;
using TazaFood_Core.Models;

namespace TazaFood_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IGenericRepository<Category> CategoryRepo;

        public CategoryController(IGenericRepository<Category> _CategoryRepo)
        {
            CategoryRepo = _CategoryRepo;
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> getCategories()
        {
            var categories = await CategoryRepo.GetAll();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var spec = new CategorySpecification(id);
            var category = await CategoryRepo.GetByIdWithSpec(spec);
            return Ok(category);
        }
    }
}
