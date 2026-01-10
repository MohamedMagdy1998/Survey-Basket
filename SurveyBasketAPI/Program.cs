using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Services;
using SurveyBasketAPI.Services_Abstraction;
using SurveyBasketAPI.Validations;
using System.Reflection;

namespace SurveyBasketAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            
            builder.AddDatabase();
           builder.AddIdentityConfiguration();
            builder.Services.AddDepenencies();



            var app = builder.Build();

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
