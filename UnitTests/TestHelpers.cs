using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using CSharpApp.Application.Products;
using CSharpApp.Application.Categories;

namespace UnitTests;

public static class TestHelpers
{
    public static HttpClient CreateMockHttpClient<T>(T response, HttpStatusCode statusCode)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(JsonSerializer.Serialize(response))
        };

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        return new HttpClient(handlerMock.Object);
    }

    public static void SetHttpClient(this ProductsService service, HttpClient client)
    {
        var fieldInfo = typeof(ProductsService).GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        fieldInfo?.SetValue(service, client);
    }

    public static void SetHttpClient(this CategoriesService service, HttpClient client)
    {
        var fieldInfo = typeof(CategoriesService).GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        fieldInfo?.SetValue(service, client);
    }
}
