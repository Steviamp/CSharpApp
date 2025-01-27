using CSharpApp.Application.Products;
using CSharpApp.Core.Interfaces;
using System.Text;

namespace CSharpApp.Application.Categories;

public class CategoriesService : ICategoriesService
{
    private readonly HttpClient _httpClient;
    private readonly IPlatziApiClient _platziApiClient;
    private readonly ILogger<CategoriesService> _logger;

    public CategoriesService(
        IPlatziApiClient platziApiClient,
        ILogger<CategoriesService> logger)
    {
        _httpClient = new HttpClient();
        _platziApiClient = platziApiClient ?? throw new ArgumentNullException(nameof(platziApiClient));
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        _httpClient.BaseAddress = new Uri(_platziApiClient.BaseUrl!);
        var response = await _httpClient.GetAsync(_platziApiClient.Categories);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Category>>(content);
        
        return res.AsReadOnly();
    }
    public async Task<Category> GetCategoryById(int id)
    {
        _httpClient.BaseAddress = new Uri(_platziApiClient.BaseUrl!);
        var response = await _httpClient.GetAsync($"{_platziApiClient.Categories}/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<Category>(content);

        return res;
    }

    public async Task<Category> CreateCategory(Category categoryDto)
    {
        _httpClient.BaseAddress = new Uri(_platziApiClient.BaseUrl!);
        var jsonContent = new StringContent(JsonSerializer.Serialize(categoryDto), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_platziApiClient.Categories, jsonContent);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<Category>(content);

        return res;
    }
}