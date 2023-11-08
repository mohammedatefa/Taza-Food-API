using AutoMapper;
using TazaFood_API.DTO;
using TazaFood_Core.Models;
using static System.Net.WebRequestMethods;

namespace TazaFood_API.Helpers
{
    public class ProductImageUrlResolver : IValueResolver<Product, ProductReturnToDto,string>
    {
        private readonly IConfiguration configration;

        public ProductImageUrlResolver(IConfiguration configration)
        {
            this.configration = configration;
        }
        public string Resolve(Product source, ProductReturnToDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImageUrl))
            {
                return $"{configration["ApiBaseUrl"]}/{source.ImageUrl}";
            }
            else
                return string.Empty;
        }
    }
}
