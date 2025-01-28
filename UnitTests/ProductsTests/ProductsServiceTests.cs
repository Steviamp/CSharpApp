using CSharpApp.Application.Products;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace UnitTests.ProductsTests;

public class ProductServiceTests
{
    private readonly Mock<ILogger<ProductsService>> _loggerMock;
    private readonly Mock<IPlatziApiClient> _apiClientMock;
    private readonly ProductsService _productService;
    private const string BaseUrl = "https://api.test.com/";
    private const string ProductsEndpoint = "products";

    public ProductServiceTests()
    {
        _loggerMock = new Mock<ILogger<ProductsService>>();
        _apiClientMock = new Mock<IPlatziApiClient>();

        _apiClientMock.Setup(x => x.BaseUrl).Returns(BaseUrl);
        _apiClientMock.Setup(x => x.Products).Returns(ProductsEndpoint);

        _productService = new ProductsService(_apiClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ShouldReturnProductsWhenApiCallIsSuccessful()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Title = "Product 1", Price = 100 },
            new() { Id = 2, Title = "Product 2", Price = 200 }
        };

        var httpClient = TestHelpers.CreateMockHttpClient(products, HttpStatusCode.OK);
        _productService.SetHttpClient(httpClient);

        // Act
        var result = await _productService.GetProducts();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Product 1", result.First().Title);
        Assert.Equal(200, result.Last().Price);
    }

    [Fact]
    public async Task ShouldReturnProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Title = "Product 1", Price = 100 };
        var httpClient = TestHelpers.CreateMockHttpClient(product, HttpStatusCode.OK);
        _productService.SetHttpClient(httpClient);

        // Act
        var result = await _productService.GetProductById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Product 1", result.Title);
    }

    [Fact]
    public async Task ShouldReturnCreatedProduct()
    {
        // Arrange
        var createProduct = new CreateProduct
        {
            Title = "New Product",
            Price = 150,
            Description = "Test Description",
            CategoryId = 1
        };

        var createdProduct = new Product
        {
            Id = 3,
            Title = createProduct.Title,
            Price = createProduct.Price,
            Description = createProduct.Description
        };

        var httpClient = TestHelpers.CreateMockHttpClient(createdProduct, HttpStatusCode.Created);
        _productService.SetHttpClient(httpClient);

        // Act
        var result = await _productService.CreateProduct(createProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("New Product", result.Title);
    }
}