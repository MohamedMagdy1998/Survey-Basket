
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SurveyBasketAPI.Mapping;
using SurveyBasketAPI.Persistence;
using SurveyBasketAPI.Services;
using SurveyBasketAPI.Services_Abstraction;
using System.Reflection;

namespace SurveyBasketAPI;

public static class DependenyInjection
{
    public static IServiceCollection AddDepenencies(this IServiceCollection service)
    {
        service.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        service.AddSwagger();
        service.AddScoped<IPollService, PollService>();

        service.AddMapsterConfiguration();

        service.AddFluentValidation();

        return service;
    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<SurveyBasketDbContext>(options =>
            options.UseSqlServer(connectionString));
    }


    public static IServiceCollection AddSwagger(this IServiceCollection service)
    {
        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen();
        return service;
    }

    public static IServiceCollection AddMapsterConfiguration(this IServiceCollection service)
    {
        var MappingConfiguration = TypeAdapterConfig.GlobalSettings;
        MappingConfiguration.Scan(Assembly.GetExecutingAssembly());

        service.AddSingleton<IMapper>
             (new Mapper(MappingConfiguration));
        return service;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection service)
    {
        service.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        service.AddFluentValidationAutoValidation();
        return service;
    }


}
