using MandalaStore.Application.Interfaces;
using MandalaStore.Application.Services;
using MandalaStore.Infrastructure.Context;
using MandalaStore.Infrastructure.Repositories;
using MandalaStore.Infrastructure.Services;
using MandalaStore.Infrastructure.Settings;
using MandalaStore.Infrastructure.Interfaces;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<MercadoPagoSettings>(
    builder.Configuration.GetSection("MercadoPago"));



builder.Services.AddSingleton<MongoContext>();
builder.Services.AddScoped<ProdutoRepository>();

builder.Services.AddScoped<PedidoRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<TemaRepository>();
builder.Services.AddScoped<IFreteService,FreteService>();
builder.Services.AddScoped<MercadoPagoService>();
builder.Services.AddScoped<ProdutoService>();




//builder.Services.AddSingleton<IMongoDatabase>(sp =>
//{
//    var client = sp.GetRequiredService<IMongoClient>();

//    var databaseName =
//        builder.Configuration["MongoDbSettings:DatabaseName"];

//    return client.GetDatabase(databaseName);
//});

builder.Services.AddControllers();


builder.Services.Configure<MelhorEnvioSettings>(
    builder.Configuration.GetSection("MelhorEnvio"));

builder.Services.AddHttpClient<IMelhorEnvioService, MelhorEnvioService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddScoped<PedidoService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();
