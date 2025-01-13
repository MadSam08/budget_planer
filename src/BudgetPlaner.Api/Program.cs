using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Extensions;
using BudgetPlaner.Application;
using BudgetPlaner.Infrastructure;
using BudgetPlaner.Infrastructure.DatabaseContext;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sqids;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.Services
    .AddAuthentication();

builder.Services.AddOptions<BearerTokenOptions>(IdentityConstants.BearerScheme)
    .Configure(options =>
{
    options.BearerTokenExpiration = TimeSpan.FromMinutes(10);
});

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
            []
        }
    });
});


builder.Services.AddEndpointDefinitions(typeof(IAssemblyMarker));

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssembly(typeof(IAssemblyMarker).Assembly);

builder.Services.AddIdentityApiEndpoints<IdentityUser>(options => { })
    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(cachePolicyBuilder => cachePolicyBuilder.Expire(TimeSpan.FromSeconds(5)));
});

builder.Services.AddSingleton(new SqidsEncoder<int>(new SqidsOptions
{
    Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
    MinLength = 5,
}));

var key = "budgetplanerCs3q.@WBaTi#34P"u8.ToArray();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(60) // Remove delay when token expires
        };
    });


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