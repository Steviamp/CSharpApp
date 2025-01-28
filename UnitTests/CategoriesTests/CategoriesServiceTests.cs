
using CSharpApp.Application.Categories;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace UnitTests.CategoriesTests;

public class CategoryServiceTests
{
    private readonly Mock<ILogger<CategoriesService>> _loggerMock;
    private readonly Mock<IPlatziApiClient> _apiClientMock;
    private readonly CategoriesService _categoryService;
    private const string BaseUrl = "https://api.test.com/";
    private const string CategoriesEndpoint = "categories";

    public CategoryServiceTests()
    {
        _loggerMock = new Mock<ILogger<CategoriesService>>();
        _apiClientMock = new Mock<IPlatziApiClient>();

        _apiClientMock.Setup(x => x.BaseUrl).Returns(BaseUrl);
        _apiClientMock.Setup(x => x.Categories).Returns(CategoriesEndpoint);

        _categoryService = new CategoriesService(_apiClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ShouldReturnCategoriesWhenApiCallIsSuccessful()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Category 1" },
            new() { Id = 2, Name = "Category 2" }
        };

        var httpClient = TestHelpers.CreateMockHttpClient(categories, HttpStatusCode.OK);
        _categoryService.SetHttpClient(httpClient);

        // Act
        var result = await _categoryService.GetCategories();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Category 1", result.First().Name);
    }

    [Fact]
    public async Task ShouldReturnCategory()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Category 1" };
        var httpClient = TestHelpers.CreateMockHttpClient(category, HttpStatusCode.OK);
        _categoryService.SetHttpClient(httpClient);

        // Act
        var result = await _categoryService.GetCategoryById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Category 1", result.Name);
    }

    [Fact]
    public async Task ShouldReturnCreatedCategory()
    {
        // Arrange
        var newCategory = new Category
        {
            Name = "New Category",
            Image = "http://example.com/image.jpg"
        };

        var createdCategory = new Category
        {
            Id = 3,
            Name = newCategory.Name,
            Image = newCategory.Image
        };

        var httpClient = TestHelpers.CreateMockHttpClient(createdCategory, HttpStatusCode.Created);
        _categoryService.SetHttpClient(httpClient);

        // Act
        var result = await _categoryService.CreateCategory(newCategory);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("New Category", result.Name);
    }
}
