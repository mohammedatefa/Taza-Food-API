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
        private readonly ICategoryRepository categoryRepo;

        public CategoryController(IGenericRepository<Category> _CategoryRepo, ICategoryRepository categoryRepo)
        {
            CategoryRepo = _CategoryRepo;
            this.categoryRepo = categoryRepo;
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> getCategories()
        {
            var categories = await CategoryRepo.GetAll();
            return Ok(categories);
        }

        [HttpGet("{id:int}", Name = "CategoryDetailsRoute")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var spec = new CategorySpecification(id);
            var category = await CategoryRepo.GetByIdWithSpec(spec);
            return Ok(category);
        }

        [HttpGet("{name:alpha}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var category = await categoryRepo.GetByName(name);
            if(category == null) // worset case
            {
                return NotFound("Category not found");
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> PostCategory(Category newCategory)
        {
            if(!ModelState.IsValid)    // worst case
            {
                return BadRequest(ModelState);
            }
            await CategoryRepo.Add(newCategory);
            string url = Url.Link("CategoryDetailsRoute", new {id = newCategory.Id});
            return Created(url, newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id ,  Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool isUpdated = await CategoryRepo.Update(id, newCategory);
            if (!isUpdated)
            {
                return NotFound("Category not found");
            }
            return StatusCode(StatusCodes.Status204NoContent, "Saved Successed");
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            bool isDeleted = await CategoryRepo.Delete(id);
            if (!isDeleted)
            {
                return NotFound("Category not found");
            }
            return StatusCode(StatusCodes.Status204NoContent, "Deleted Successed");
        }
    }
}
