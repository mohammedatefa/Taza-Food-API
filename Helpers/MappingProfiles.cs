using AutoMapper;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using TazaFood_API.DTO;
using TazaFood_Core.IdentityModels;
using TazaFood_Core.Models;
using TazaFood_Core.Models.Order_Aggregate;

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


            //mapping to user adress
            CreateMap<TazaFood_Core.IdentityModels.Address, UserAddressDto>().ReverseMap();

            CreateMap<UserAddressDto, TazaFood_Core.Models.Order_Aggregate.Address>();
        }
    }
}
