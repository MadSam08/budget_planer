using System.Reflection;
using BudgetPlaner.Api.DatabaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<IdentityContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
