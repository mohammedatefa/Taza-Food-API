using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TazaFood_API.DTO;
using TazaFood_API.Helpers;
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
        private readonly IWebHostEnvironment environment;

        public ProductController(IGenericRepository<Product> _productRepo,IMapper _mapper,IWebHostEnvironment _enviroment)
        {
            productRepo = _productRepo;
            mapper = _mapper;
            environment = _enviroment;
          
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //useing sepcification pattern to return products
            var spec = new ProductWithCategorySpecification();
            var products = await productRepo.GetAllWithSpec(spec);

            return Ok(mapper.Map<IEnumerable<Product>,IEnumerable<ProductReturnToDto>>(products));
        }

        [HttpGet("GetProductsOrderBy")]
        public async Task<IActionResult> GetProductsOrderBy(string sort)
        {
            //useing sepcification pattern to return products
            var spec = new ProductWithCategorySpecification(sort);
            var products = await productRepo.GetAllWithSpec(spec);

            return Ok(mapper.Map<IEnumerable<Product>, IEnumerable<ProductReturnToDto>>(products));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //using specification pattern to return product by id
            var spec = new ProductWithCategorySpecification(id);
            var product = await productRepo.GetByIdWithSpec(spec);

            return Ok(mapper.Map<Product,ProductReturnToDto>(product));
        }


        [HttpPost("AddProduct")]
        public async Task<IActionResult> CreateProduct(string name,string description,int rate,int price,int categoryid,IFormFile formfile)
        {
            //upload images first and save it on the database 
            string ImagePath = await UploadeImage.SaveImage(formfile, this.environment.WebRootPath, name);

            Product product = new Product();
            product.Name = name;
            product.Description = description;
            product.Rate = rate;
            product.Price = price;
            product.CategoryId = categoryid;
            product.ImageUrl = ImageUrlResolve.ResolveUrl(ImagePath);
            await productRepo.Add(product);
            return Ok();
      
        }


        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> updateProduct(int id, string name, string description, int rate, int price, int categoryid, IFormFile formfile)
        {
            // Retrieve existing product from the database
            Product product = await productRepo.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            // Upload a new image if provided
            string ImagePath = formfile != null
                ? await UploadeImage.SaveImage(formfile, this.environment.WebRootPath, name)
                : product.ImageUrl;

            // Update the product properties
            product.Name = name;
            product.Description = description;
            product.Rate = rate;
            product.Price = price;
            product.CategoryId = categoryid;
            product.ImageUrl = ImageUrlResolve.ResolveUrl(ImagePath);

            // Update the product in the database
            await productRepo.Update(id, product);

            return Ok(product);
        }


        [HttpDelete("DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            // Retrieve the existing product from the database
            Product product = await productRepo.GetById(productId);

            if (product == null)
            {
                return NotFound(); // Product not found
            }

            // Delete the product image if it exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string imagePath = Path.Combine(environment.WebRootPath, product.ImageUrl);

                // Ensure that the file exists before attempting to delete it
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Delete the product from the database
            await productRepo.Delete(productId);

            return NoContent(); // Return 204 No Content upon successful deletion
        }




    }
}
