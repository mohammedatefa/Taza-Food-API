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

            //mapping to order 
            CreateMap<Order, OrderReturnToDto>()
                .ForMember(d => d.DeliveryMethod, d => d.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.Cost, d => d.MapFrom(s => s.DeliveryMethod.Cost))
                .ForMember(d => d.DeliveryTime, d => d.MapFrom(s => s.DeliveryMethod.DeliveryTime));
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.Id, d => d.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, d => d.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.ImageUrl, d => d.MapFrom(s => s.Product.ImageUrl))
                .ForMember(d => d.ImageUrl, d => d.MapFrom<OrderItemImageResolve>());
            
        }
    }
}
