namespace CSharpApp.Core.Interfaces;

public interface IPlatziApiClient
{
    string BaseUrl { get; }
    string Products { get; }
    Task<string> GetDataAsync(string endpoint);
}