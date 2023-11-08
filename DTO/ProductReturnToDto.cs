using TazaFood_Core.Models;

namespace TazaFood_API.DTO
{
    public class ProductReturnToDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int? Rate { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}
