using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Extensions;
using BudgetPlaner.Application;
using BudgetPlaner.Infrastructure;
using BudgetPlaner.Infrastructure.DatabaseContext;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Sqids;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
    options.IgnoreObsoleteActions();

    // Set this flag to omit schema property descriptions for any type properties decorated with the
    options.IgnoreObsoleteProperties();

    // Set custom schema name 
    options.CustomSchemaIds(x => x.FullName);
    options.DescribeAllParametersInCamelCase();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer {...}\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            []
        }
    });
});

builder.Services.AddEndpointDefinitions(typeof(IAssemblyMarker));

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssembly(typeof(IAssemblyMarker).Assembly);

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme, options =>
    {
        // Set access token expiration to 30 minutes
        options.BearerTokenExpiration = TimeSpan.FromMinutes(30);

        // Set refresh token expiration to 7 days
        options.RefreshTokenExpiration = TimeSpan.FromDays(7);
    });
    // .AddGoogle(options =>
    // {
    //     options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    //     options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    //     options.SignInScheme = IdentityConstants.ExternalScheme;
    // })
    // .AddFacebook(options =>
    // {
    //     options.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
    //     options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
    //     options.SignInScheme = IdentityConstants.ExternalScheme;
    // });

builder.Services.AddAuthorizationBuilder();

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddApiEndpoints();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(cachePolicyBuilder => cachePolicyBuilder.Expire(TimeSpan.FromSeconds(30)));
});

builder.Services.AddSingleton(new SqidsEncoder<int>(new SqidsOptions
{
    Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
    MinLength = 5,
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpointDefinitions();
app.ApplyMigration();

app.Run();