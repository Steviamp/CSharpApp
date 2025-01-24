using Autofac.Core;
using CSharpApp.Application.Categories;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using CSharpApp.Infrastructure.Clients;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDefaultConfiguration();
builder.Services.AddHttpConfiguration(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

var versionedEndpointRouteBuilder = app.NewVersionedApi();

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/products", async (IProductsService productsService) =>
    {
        var products = await productsService.GetProducts();
        return products;
    })
    .WithName("GetProducts")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/products/{id}", async (int id, IProductsService productsService) =>
{
    var product = await productsService.GetProductById(id);
    return product != null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProductById")
.HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/products", async (CreateProduct productDto, IProductsService productsService) =>
{
    if (productDto == null)
    {
        return Results.BadRequest("Product data is required.");
    }

    var createdProduct = await productsService.CreateProduct(productDto);

    return Results.Created($"api/v1/product/{createdProduct.Id}", createdProduct);
})
.WithName("CreateProduct")
.HasApiVersion(1.0);


versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/categories", async (ICategoriesService categoriesService) =>
{
    var categories = await categoriesService.GetCategories();
    return categories;
})
    .WithName("GetCategories")
.HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/categories/{id}", async (int id, ICategoriesService categoriesService) =>
{
    var category = await categoriesService.GetCategoryById(id);
    return category != null ? Results.Ok(category) : Results.NotFound();
})
.WithName("GetCategoryById")
.HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/categories", async (Category categoryDto, ICategoriesService categoriesService) =>
{
    if (categoryDto == null)
    {
        return Results.BadRequest("Product data is required.");
    }

    var createdCategory = await categoriesService.CreateCategory(categoryDto);

    return Results.Created($"api/v1/categories/{createdCategory.Id}", createdCategory);
})
.WithName("CreateCategory")
.HasApiVersion(1.0);

app.Run();