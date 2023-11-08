using TazaFood_API.Helpers;
using TazaFood_Core.IRepositories;
using TazaFood_Repository.Repository;

namespace TazaFood_API.Extenssions
{
    public static class ApplicationServicesExtenssions
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            //Generic Repository Injection
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //add mapper services
            services.AddAutoMapper(typeof(MappingProfiles));

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
