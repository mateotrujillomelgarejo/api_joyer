using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using api_joyeria.Api.Middlewares;
using api_joyeria.Api.AutoMapper;
using AutoMapper;
using MediatR;
using api_joyeria.Application.Interfaces;
using api_joyeria.Infrastructure.Payments.Izipay;
using api_joyeria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Infrastructure.Repositories;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Application.Services;
using api_joyeria.Infrastructure.Services;
using api_joyeria.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(opts => {
        opts.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// AutoMapper - register profiles in this assembly
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// MediatR - will discover handlers in Application assembly if referenced
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(AssemblyReference).Assembly
    );
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


// Register API-specific services or adapters here (implementations live in Infrastructure)
// Example registrations (uncomment and adjust when you add Infrastructure project references):
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICheckoutService,CheckoutService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IInventoryService, DatabaseInventoryService>();
builder.Services.AddScoped<IPaymentGateway,IzipayPaymentGateway>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

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