using EBooking.API.Middleware;
using EBooking.Application;
using EBooking.Infrastructure;
using EBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwaggerGen();
        
builder.Services.AddCors(options =>
{
    var corsUrls = builder.Configuration["CORSAllowedOrigins"].ToString()
              .Split(",", StringSplitOptions.RemoveEmptyEntries)
                     .ToArray();
    options.AddPolicy("CorsPolicy",
    builder =>
    {
        builder.WithOrigins(corsUrls)
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.SeedDatabaseAsync();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
