namespace CSharpApp.Core.Interfaces;

public interface IProductsService
{
    Task<IReadOnlyCollection<Product>> GetProducts();
    Task<Product> GetProductById(int id);
    Task<Product> CreateProduct(Product productDto);
}