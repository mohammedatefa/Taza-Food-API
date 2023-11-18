using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;
using TazaFood_API.Extenssions;
using TazaFood_API.Helpers;
using TazaFood_Core.IdentityModels;
using TazaFood_Core.IRepositories;
using TazaFood_Core.Models;
using TazaFood_Repository.Context;
using TazaFood_Repository.IdentityContext;
using TazaFood_Repository.Repository;

namespace TazaFood_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configrution services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //add session to save the cart item 
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(72);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            //add sql-database connection 
            builder.Services.AddDbContext<TazaDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefualtConnection"));
            });

            //add Identity connection
            builder.Services.AddDbContext<IdentityContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
     

            //add Redis Connection 
            builder.Services.AddSingleton<IConnectionMultiplexer>(s =>
            {
                var connection = builder.Configuration.GetConnectionString("RedisConnection");

                return ConnectionMultiplexer.Connect(connection);

            });

            //add application services 
            builder.Services.ApplicationServices();

            //add identity services 
            builder.Services.AddIdentityServices(builder.Configuration);

            #endregion


            var app = builder.Build();


            #region Auto Migration

            //Add Auto Migration

            using(var scope = app.Services.CreateScope())
            { 
                var services = scope.ServiceProvider;

                var ILoggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
               {


                    var usermanger = services.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var Identitycontext = services.GetRequiredService<IdentityContext>();
                    await Identitycontext.Database.MigrateAsync();
                    await IdentityDbContextSeed.AppUserAsync(usermanger);
                    await IdentityDbContextSeed.AppRoleAsync(roleManager);

                    var dbcontext = services.GetRequiredService<TazaDbContext>();
                    await dbcontext.Database.MigrateAsync();
                    //seeding the initial-data to database
                    await TazaContextSeed.Dataseeding(dbcontext);

                  
                }
                catch (Exception ex)
                {
                    var logger = ILoggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "there is some thing wrong.....");
                }
            }
            #endregion 

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
               
            }

            #region Middlewares Pipe
            
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseSession();
            app.UseAuthentication();
            app.UseCors("mypolicy");
            app.UseAuthorization();

            
            app.MapControllers(); 
            #endregion 

            app.Run();
        }
    }
}