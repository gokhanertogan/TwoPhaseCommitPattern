using System.Net;
using System.Text.Json;

namespace COORDINATOR.API.Clients;


public class StockApiClient(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(StockApiClient));

    public async Task<(bool IsSuccess, bool result, HttpStatusCode StatusCode)> CheckStockAsync()
    {
        var response = await _httpClient.GetAsync("/check-stock");
        var content = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, JsonSerializer.Deserialize<bool>(content), response.StatusCode);
    }

    public async Task<(bool IsSuccess, bool Response, HttpStatusCode StatusCode)> DecreaseStockAsync(long productId)
    {
        if (productId == default)
        {
            return (false, default, HttpStatusCode.BadRequest);
        }

        var response = await _httpClient.PostAsync($"/decrease-stock/{productId}", null);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = response.IsSuccessStatusCode && JsonSerializer.Deserialize<bool>(responseContent);

        return (response.IsSuccessStatusCode, result, response.StatusCode);
    }

    public async Task<(bool IsSuccess, bool Response, HttpStatusCode StatusCode)> IncreaseStockAsync(long productId)
    {
        if (productId == default)
        {
            return (false, default, HttpStatusCode.BadRequest);
        }

        var response = await _httpClient.PostAsync($"/increase-stock/{productId}", null);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = response.IsSuccessStatusCode && JsonSerializer.Deserialize<bool>(responseContent);

        return (response.IsSuccessStatusCode, result, response.StatusCode);
    }    
}

