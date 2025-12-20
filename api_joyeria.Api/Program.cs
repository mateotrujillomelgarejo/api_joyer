using api_joyeria.Api.Middleware;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Application.Services;
using api_joyeria.Infrastructure.Payments.Izipay;
using api_joyeria.Infrastructure.Persistence;
using api_joyeria.Infrastructure.Repositories;
using api_joyeria.Infrastructure.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuration & Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar HttpClient para izipay adapter
builder.Services.AddHttpClient<IPaymentGateway, IzipayPaymentGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Izipay:BaseUrl"] ?? "https://api.izipay.com/");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// Registrar implementeaciones y services necesarios
builder.Services.AddScoped<IInventoryService, DatabaseInventoryService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IGuestTokenGenerator, GuestTokenGenerator>();

// AutoMapper
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddSingleton<IGuestTokenGenerator, GuestTokenGenerator>();

// FluentValidation, CORS, Authentication etc. can be added here.

var app = builder.Build();

// Middlewares
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();