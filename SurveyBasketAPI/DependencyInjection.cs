using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurveyBasketAPI.Entities;
using SurveyBasketAPI.Middleware;
using SurveyBasketAPI.Option_Pattern;
using SurveyBasketAPI.Persistence;
using SurveyBasketAPI.Services;
using SurveyBasketAPI.Services_Abstraction;
using System.Reflection;
using System.Text;
using SurveyBasketAPI.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using Hangfire;

namespace SurveyBasketAPI;

public static class DependenyInjection
{
    public static IServiceCollection AddDepenencies(this IServiceCollection service,IConfiguration configuration=default!)
    {
        service.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        service._AddCors();

        service.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        service.AddDatabase(configuration);
        service.AddIdentityConfiguration(configuration);
        service.AddHybridCache();

        service.AddScoped<IAuthService, AuthService>();

        service.AddScoped<IPollService, PollService>();
        service.AddScoped<IQuestionService, QuestionService>(); 
        service.AddScoped<IVoteService, VoteService>();
        service.AddScoped<IResultService, ResultService>();
        service.AddScoped<IEmailSender, EmailService>();
        service.AddScoped<INotificationService, NotificationService>();
        service.AddScoped<IUserService, UserService>();



        service.AddHealthChecks().AddDbContextCheck<SurveyBasketDbContext>(name: "Database")
             .AddHangfire(options => { options.MinimumAvailableServers = 1; });
        service.AddHttpContextAccessor();

        service.AddExceptionHandler<GlobalExceptionHandler>();
        service.AddProblemDetails();

        service.AddSwagger();
        service.AddMapsterConfiguration();
        service.AddFluentValidation();


        service.AddBackgroundJobsConfiguration(configuration);

        return service;
    }


    public static IServiceCollection _AddCors(this IServiceCollection service)
    {
        service.AddCors(options =>
        {
            options.AddDefaultPolicy( builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
        return service;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection service, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        service.AddDbContext<SurveyBasketDbContext>(options =>
            options.UseSqlServer(connectionString));
        return service;
    }

    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddSingleton<IJwtProvider, JwtProvider>();
        service.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<SurveyBasketDbContext>()
            .AddDefaultTokenProviders();

        //service.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        service.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations().ValidateOnStart();

        var JWTsettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();


        service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTsettings!.Key)),
                ValidIssuer = JWTsettings.Issuer,
                ValidAudience = JWTsettings.Audience
            };
        });


        service.Configure<IdentityOptions>(options =>
        {

            //options.SignIn.RequireConfirmedEmail = true;

            options.User.RequireUniqueEmail = true ;

            options.Password.RequiredLength = 8;
        });
        return service;
    }


    public static IServiceCollection AddSwagger(this IServiceCollection service)
    {
        service.AddEndpointsApiExplorer();
       
        service.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                Description = "Please enter a word 'Bearer' followed by Space and your Token.'"

            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference=new OpenApiReference()
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });

        });
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

    public static IServiceCollection AddBackgroundJobsConfiguration(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddHangfire(config => config
       .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));


        service.AddHangfireServer();

        return service;
    }
}
