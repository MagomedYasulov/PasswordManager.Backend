
using FluentValidation.AspNetCore;
using FluentValidation;
using PasswordManager.Backend.Extententions;
using PasswordManager.Backend.Models;
using PasswordManager.Backend.Services;
using PasswordManager.Backend.Validators;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using PasswordManager.Backend.Middlewares;
using PasswordManager.Backend.Data;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Backend.Data.Entities;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Localization;
using PasswordManager.Backend.Controllers;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace PasswordManager.Backend
{
    public class Program
    {
        private static readonly CultureInfo[] supportedCultures = [new CultureInfo("en"), new CultureInfo("ru")];

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region DB
            builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IRepository, EFRepository<ApplicationContext>>();
            builder.Services.AddIdentity<User, IdentityRole<int>>()
                            .AddEntityFrameworkStores<ApplicationContext>()
                            .AddUserManager<UserManager<User>>()
                            .AddSignInManager<SignInManager<User>>();
            #endregion

            #region Cors
            builder.Services.AddCors(c => c.AddPolicy("cors", opt =>
            {
                opt.AllowAnyHeader();
                opt.AllowCredentials();
                opt.AllowAnyMethod();
                opt.SetIsOriginAllowed(_ => true);
                //opt.AllowAnyOrigin(); //opt.WithOrigins(builder.Configuration.GetSection("Cors:Urls").Get<string[]>()!);

            }));
            #endregion

            #region Authorization
            builder.Services.AddAuthorization(options => options.DefaultPolicy =
            new AuthorizationPolicyBuilder
                    (JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());
            #endregion

            #region Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "JWT_OR_COOKIE";
                options.DefaultChallengeScheme = "JWT_OR_COOKIE";
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Events = new JwtBearerEvents()
                {
                    OnForbidden = JwtBearerEventsImplimintation.OnForbidden,
                    OnChallenge = JwtBearerEventsImplimintation.OnChallenge,
                    OnTokenValidated = JwtBearerEventsImplimintation.OnTokenValidated,
                    OnMessageReceived = JwtBearerEventsImplimintation.OnMessageReceived
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtToken:ISSUER"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtToken:AUDIENCE"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = builder.Configuration.CreateSymmetricSecurityKey(),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(5),
                };
            })
            .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
            {
                options.ForwardDefaultSelector = context => JwtBearerDefaults.AuthenticationScheme;
            });
            #endregion

            #region Controllers
            builder.Services.AddRouting(r => r.LowercaseUrls = true);
            builder.Services.AddControllers().AddNewtonsoftJson();
            #endregion

            #region Swagger
           
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "PasswordManager", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            #endregion

            #region AutoMapper

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            
            #endregion

            #region Fluent validation
            
            builder.Services.AddFluentValidationRulesToSwagger();
            builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
            builder.Services.AddFluentValidationAutoValidation(config =>
            {
                config.DisableDataAnnotationsValidation = true;
            });

            #endregion

            #region Localization 
            
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            #endregion

            #region Exception Handler
           
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            
            #endregion

            #region TokenService

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddHostedService<TokenHostedService>();

            #endregion

            #region Invalid ModelState response

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var localizer = actionContext.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<BaseController>>();
                    var problemDetails = new ValidationProblemDetails(actionContext.ModelState)
                    {
                        Type = options.ClientErrorMapping[400].Link,
                        Title = localizer["Title400"],
                        Status = StatusCodes.Status400BadRequest,
                        Detail = localizer["Detail"],
                        Instance = actionContext.HttpContext.Request.Path
                    };
                    problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? actionContext.HttpContext.TraceIdentifier);
                    return new BadRequestObjectResult(problemDetails);
                };
            });

            #endregion

            var app = builder.Build();

           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseExceptionHandler();

            //app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("cors");

            app.MapControllers();

            app.Run();
        }
    }
}
