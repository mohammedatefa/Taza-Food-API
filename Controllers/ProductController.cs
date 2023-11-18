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
        private IUnitOfWork productRepo;
        private IMapper mapper;
        private readonly IWebHostEnvironment environment;

        public ProductController(IUnitOfWork _productRepo,IMapper _mapper,IWebHostEnvironment _enviroment)
        {
            productRepo = _productRepo;
            mapper = _mapper;
            environment = _enviroment;
          
        }

        //get all products
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAll()
        {
            //useing sepcification pattern to return products
            var spec = new ProductWithCategorySpecification();
            var products = await productRepo.Repository<Product>().GetAllWithSpec(spec);

            return Ok(mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductReturnToDto>>(products));
        }


        //get productby Id
        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetById(int id)
        {
            //using specification pattern to return product by id
            var spec = new ProductWithCategorySpecification(id);
            var product = await productRepo.Repository<Product>().GetByIdWithSpec(spec);

            return Ok(mapper.Map<Product, ProductReturnToDto>(product));
        }


        //sorting products by specification type 
        [HttpGet("GetProductsOrderBy")]
        public async Task<IActionResult> GetProductsOrderBy(string sort)
        {
            //useing sepcification pattern to return products
            var spec = new ProductWithCategorySpecification(sort);
            var products = await productRepo.Repository<Product>().GetAllWithSpec(spec);

            return Ok(mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductReturnToDto>>(products));
        }


        //filtteration products by price or rate or category name or by all 
        [HttpGet("GetProductsFillterBy")]
        public async Task<IActionResult> GetProductsFillterBy(int?price,int? rate,string?category)
        {
            //useing sepcification fillter to return products 
            var spec = new ProductWithCategorySpecification(price,rate,category);
            var products = await productRepo.Repository<Product>().GetAllWithSpec(spec);

            return Ok(mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductReturnToDto>>(products));
        }

        //pagination 
        [HttpGet("ProductsPagination")]
        public async Task<IActionResult> ProductsPagination([FromQuery]ProductPaginationParams prams)
        {
            //useing sepcification fillter to return products 
            var spec = new ProductWithCategorySpecification(prams);
            var products = await productRepo.Repository<Product>().GetAllWithSpec(spec);
            var data = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductReturnToDto>>(products);
            var countspec = new ProductWithFillterationForPaginationSpecification(prams);

            var count = await productRepo.Repository<Product>().GetCountWithSpec(countspec);

            return Ok(new ProductPagination<ProductReturnToDto>(prams.Pagesize, prams.pageIndex, count,data));
        }


        //add new product
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
            await productRepo.Repository<Product>().Add(product);
            await productRepo.complete();
            return Ok();
      
        }

        //updaute product
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> updateProduct(int id, string name, string description, int rate, int price, int categoryid, IFormFile formfile)
        {
            // Retrieve existing product from the database
            Product product = await productRepo.Repository<Product>().GetById(id);

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
            await productRepo.Repository<Product>().Update(id, product);
            await productRepo.complete();


            return Ok(product);
        }

        //delete product
        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            // Retrieve the existing product from the database
            Product product = await productRepo.Repository<Product>().GetById(productId);

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
                    await productRepo.complete();

                }
            }

            // Delete the product from the database
            await productRepo.Repository<Product>().Delete(productId);
            await productRepo.complete();


            return NoContent(); // Return 204 No Content upon successful deletion
        }




    }
}
