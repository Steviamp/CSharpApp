namespace CSharpApp.Core.Interfaces;

public interface IPlatziApiClient
{
    string BaseUrl { get; }
    string Products { get; }
    string Categories {  get; }
    Task<string> GetDataAsync(string endpoint);
}