using AspireStack.Application;
using AspireStack.Application.AppService;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Localization;
using AspireStack.Domain.Services;
using AspireStack.Domain.Shared.UserManagement;
using AspireStack.Infrastructure;
using AspireStack.Infrastructure.EntityFrameworkCore;
using AspireStack.WebApi.DynamicRouteMapping;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using AspireStack.WebApi.Host.Authentication;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
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
                BearerFormat = "token",
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
        builder.AddAuthentication();
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
}
