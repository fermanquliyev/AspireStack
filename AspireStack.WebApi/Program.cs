using AspireStack.Application;
using AspireStack.Application.AppService;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Localization;
using AspireStack.Domain.Services;
using AspireStack.Domain.Shared.UserManagement;
using AspireStack.Infrastructure;
using AspireStack.WebApi.DynamicRouteMapping;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Security.Claims;
using System.Text;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.OperationFilter<DynamicRouteOperationFilter>(); // Register the filter
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
            });
        });

        builder.Services.AddCors();
        builder.RegisterInfrastructureModule("AspireStackDb");
        var tokenParameters = builder.Configuration.GetSection("Jwt").Get<TokenParameters>();
        ArgumentNullException.ThrowIfNull(tokenParameters, "Jwt parameters are not set.");

        builder.Services.AddOptions<TokenParameters>().Bind(builder.Configuration.GetSection("Jwt"));
        builder.Services.AddAuthorization(AddPolicies);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.IncludeErrorDetails = true;
                o.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = tokenParameters.Issuer,
                    ValidAudience = tokenParameters.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenParameters.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.RegisterAppServices();
        builder.AddRedisDistributedCache("cache");
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = CultureHelper.GetSupportedCultures();
            options.DefaultRequestCulture = new RequestCulture(CultureHelper.DefaultCulture);
            options.SupportedCultures = supportedCultures.ToList();
            options.SupportedUICultures = supportedCultures.ToList();
        });

        var app = builder.Build();
        app.RegisterDynamicRoutes();
        app.MapDefaultEndpoints();
        app.MapControllers();
        app.SetupLocalization();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors(static builder =>
            builder.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin());

        app.Run();
    }

    private static void SetupLocalization(this WebApplication app)
    {
        app.UseRequestLocalization();
        var localizationProvider = app.Services.GetRequiredService<ILocalizationProvider>();
        LocalizationInitializer.Initialize(localizationProvider);
    }

    /// <summary>
    /// Add policies for authorization. Each policy is based on a permission.
    /// </summary>
    /// <param name="authentication"></param>
    static void AddPolicies(AuthorizationOptions authentication)
    {
        var permissionNames = PermissionNames.PermissionStrings;
        foreach (var permission in permissionNames)
        {
            authentication.AddPolicy(permission.Value, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    var permisionEnum = (int)permission.Key;
                    return context.User.Claims.Where(x => x.Type == CustomClaimTypes.Permission).Any(x => x.Value == permisionEnum.ToString());
                });
            });
        }
    }
}