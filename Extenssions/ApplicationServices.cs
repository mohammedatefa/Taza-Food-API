using TazaFood_API.Helpers;
using TazaFood_Core.IRepositories;
using TazaFood_Core.Services;
using TazaFood_Repository.Repository;
using TazaFood_Services.Order;
using TazaFood_Services.PaymentService;

namespace TazaFood_API.Extenssions
{
    public static class ApplicationServicesExtenssions
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services)
        {

            //add Unitofwork Service 
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWorkRepository));
            //Generic Repository Injection
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //Category Repository Service
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            //add cartitem services
            services.AddScoped(typeof(ICartItemsRepository), typeof(CartItemRepository));

            //add mapper services
            services.AddAutoMapper(typeof(MappingProfiles));


            //add cartitem services
            services.AddScoped(typeof(ICartItemsRepository), typeof(CartItemRepository));

            //add order service
            services.AddScoped(typeof(IOrderService), typeof(OrderService));

            //add payments service
            services.AddScoped(typeof(IPaymentService),typeof(PaymentService));

            

            //add cors policy
            services.AddCors(options =>
            {
                options.AddPolicy("mypolicy", corsoptions =>
                {
                    corsoptions.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            return services;
        }
    }
}
