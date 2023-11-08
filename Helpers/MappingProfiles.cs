using AutoMapper;
using TazaFood_API.DTO;
using TazaFood_Core.Models;

namespace TazaFood_API.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {

            //mapping the category name to display it 
            CreateMap<Product, ProductReturnToDto>()
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d=>d.ImageUrl,o=>o.MapFrom<ProductImageUrlResolver>());
        }
    }
}
