using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using TazaFood_API.Extenssions;
using TazaFood_API.Helpers;
using TazaFood_Core.IRepositories;
using TazaFood_Core.Models;
using TazaFood_Repository.Context;
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

            //add connection string 
            builder.Services.AddDbContext<TazaDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefualtConnection"));
            });

            //add application services 
            builder.Services.ApplicationServices();
            #endregion 


            var app = builder.Build();
            #region Auto Migration

            //Add Auto Migration

            var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var ILoggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var dbcontext = services.GetRequiredService<TazaDbContext>();
               await dbcontext.Database.MigrateAsync();

                //seeding the initial-data to database
               await TazaContextSeed.Dataseeding(dbcontext);
            }
            catch (Exception ex)
            {
                var logger = ILoggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "there is som thing wrong.....");
            } 
            #endregion 

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            #region Middlewares Pipe
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("mypolicy");
            app.MapControllers(); 
            #endregion 

            app.Run();
        }
    }
}