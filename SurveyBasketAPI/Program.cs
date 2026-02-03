using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Serilog;
using SurveyBasketAPI.DataSeeding;
using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Middleware;
using SurveyBasketAPI.Services;
using SurveyBasketAPI.Services_Abstraction;
using SurveyBasketAPI.Validations;
using System.Reflection;
using System.Threading.Tasks;

namespace SurveyBasketAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDepenencies(builder.Configuration);

            builder.Host.UseSerilog((Context, Configuration) =>
            Configuration.ReadFrom.Configuration(Context.Configuration));
            
            builder.Services.AddDistributedMemoryCache();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                await CreateUser.SeedUserAsync(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
