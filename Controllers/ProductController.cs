using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TazaFood_API.DTO;
using TazaFood_Core.IRepositories;
using TazaFood_Core.ISpecifications;
using TazaFood_Core.Models;

namespace TazaFood_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IGenericRepository<Product> productRepo;
        private IMapper mapper;

        public ProductController(IGenericRepository<Product> _productRepo,IMapper _mapper)
        {
            productRepo = _productRepo;
            mapper = _mapper;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //useing sepcification pattern to return products
            var spec = new ProductWithCategorySpecification();
            var products = await productRepo.GetAllWithSpec(spec);

            return Ok(mapper.Map<IEnumerable<Product>,IEnumerable<ProductReturnToDto>>(products));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //using specification pattern to return product by id
            var spec = new ProductWithCategorySpecification(id);
            var product = await productRepo.GetByIdWithSpec(spec);

            return Ok(mapper.Map<Product,ProductReturnToDto>(product));
        }

        [HttpPost]
        [Route("AddProduct")]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                await productRepo.Add(product);
                return Ok($"{product} \n is added successfully");
            }
            return BadRequest("there is some thing is invalide");
        }

    }
}
