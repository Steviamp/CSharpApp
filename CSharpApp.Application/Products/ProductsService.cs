using System.Text;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(
        IPlatziApiClient apiClient,
        ILogger<ProductsService> logger)
    {
        _httpClient = new HttpClient();
        _restApiSettings = apiClient.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        _httpClient.BaseAddress = new Uri(_restApiSettings.BaseUrl!);
        var response = await _httpClient.GetAsync(_restApiSettings.Products);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Product>>(content);
        
        return res.AsReadOnly();
    }
    public async Task<Product> GetProductById(int id)
    {
        _httpClient.BaseAddress = new Uri(_restApiSettings.BaseUrl!);
        var response = await _httpClient.GetAsync($"{_restApiSettings.Products}/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<Product>(content);

        return res;
    }

    public async Task<Product> CreateProduct(CreateProduct productDto)
    {
        _httpClient.BaseAddress = new Uri(_restApiSettings.BaseUrl!);
        var jsonContent = new StringContent(JsonSerializer.Serialize(productDto), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_restApiSettings.Products, jsonContent);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<Product>(content);

        return res;
    }
}