using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using HangfireBasicAuthenticationFilter;
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
using Swashbuckle.AspNetCore.SwaggerUI;
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

            using (var myscope = app.Services.CreateScope())
            {
                await CreateUser.SeedUserAsync(myscope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.ConfigObject = new ConfigObject()
                    {
                        DisplayRequestDuration = true,
                    };
                    options.DocumentTitle = "Survey Basket App";
                    options.DocExpansion(DocExpansion.None);
                    options.EnableFilter();
                    options.EnablePersistAuthorization();
                });

                app.UseHangfireDashboard("/jobs", new DashboardOptions
                {
                    Authorization = new[] {
                        new HangfireCustomBasicAuthenticationFilter
                    {
                        User = app.Configuration.GetValue<string>("HangfireSettings:UserName"),
                        Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
                    }
                    },
                    DashboardTitle = "Survey Basket Dashboard"
                });

            }

            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            RecurringJob.AddOrUpdate("SendNewPollsNotification", () => notificationService.SendNewPollsNotification(null), Cron.Daily);


            app.UseSerilogRequestLogging();

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors();
            app.UseAuthorization();
            app.MapHealthChecks("health");


            app.MapControllers();

            app.Run();
        }
    }
}
