using api_joyeria.Api.AutoMapper;
using api_joyeria.Api.Middlewares;
using api_joyeria.Application;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Application.Services;
using api_joyeria.Application.Validators;
using api_joyeria.Infrastructure.Payments.Izipay;
using api_joyeria.Infrastructure.Persistence;
using api_joyeria.Infrastructure.Repositories;
using api_joyeria.Infrastructure.Services;
using api_joyeria.Infrastructure.Services.Cloudinary;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = new[] { "http://localhost:3000", "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.Configure<IzipayOptions>(
    builder.Configuration.GetSection("Izipay")
);

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(AssemblyReference).Assembly
    );
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateGuestOrderCommandValidator>();

builder.Services.AddHttpClient<IPaymentGateway, IzipayPaymentGateway>((sp, client) =>
{
    var opts = sp.GetRequiredService<IOptions<IzipayOptions>>().Value;

    if (!string.IsNullOrWhiteSpace(opts.BaseUrl))
    {
        client.BaseAddress = new Uri(opts.BaseUrl);
    }

    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IInventoryService, DatabaseInventoryService>();

builder.Services.Configure<CloudinaryOptions>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton<IStorageService, CloudinaryStorageService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors("AllowLocalFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();