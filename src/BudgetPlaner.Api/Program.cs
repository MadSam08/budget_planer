using System.Reflection;
using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.DatabaseContext;
using BudgetPlaner.Api.Extensions;
using BudgetPlaner.Api.Repository.UnitOfWork;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sqids;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
            new string[] { }
        }
    });
});

var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
builder.Services.AddDbContext<IdentityContext>(
    opts =>
    {
        opts.UseNpgsql(builder.Configuration["IdentityDb:DbConnection"],
            optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly(assemblyName);
                optionsBuilder.EnableRetryOnFailure();
            });
    });

builder.Services.AddDbContext<BudgetPlanerContext>(
    opts =>
    {
        opts.UseNpgsql(builder.Configuration["BudgetPlanerDb:DbConnection"],
            optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly(assemblyName);
                optionsBuilder.EnableRetryOnFailure();
            });
    });

builder.Services.AddScoped<IUnitOfWork<BudgetPlanerContext>, UnitOfWork<BudgetPlanerContext>>();

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddEndpointDefinitions(typeof(IAssemblyMarker));

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssembly(typeof(IAssemblyMarker).Assembly);

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(cachePolicyBuilder => cachePolicyBuilder.Expire(TimeSpan.FromSeconds(5)));
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
app.UseEndpointDefinitions();
app.UseAuthentication();
app.UseAuthorization();
app.ApplyMigration();

app.Run();
