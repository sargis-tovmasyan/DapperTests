using DapperSqlLearning.Api.Data;
using DapperSqlLearning.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IAppDbConnectionFactory, AppDbConnectionFactory>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.InitializeDatabaseAsync();
app.MapApiEndpoints();

app.Run();
