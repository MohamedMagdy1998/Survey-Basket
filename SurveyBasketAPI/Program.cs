using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using SurveyBasketAPI.DataSeeding;
using SurveyBasketAPI.DTOs;
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

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
