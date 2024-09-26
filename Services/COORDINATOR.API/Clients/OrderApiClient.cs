using System.Net;
using System.Text;
using System.Text.Json;
using COORDINATOR.API.Models;

namespace COORDINATOR.API.Clients;

public class OrderApiClient(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(OrderApiClient));

    public async Task<(bool IsSuccess, bool result, HttpStatusCode StatusCode)> CheckOrderAsync()
    {
        var response = await _httpClient.GetAsync("/check-order");
        var content = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, JsonSerializer.Deserialize<bool>(content), response.StatusCode);
    }

    public async Task<(bool IsSuccess, CreateOrderResponseModel? Response, HttpStatusCode StatusCode)> CreateOrderAsync(CreateOrderDto requestModel)
    {
        if (requestModel == null)
        {
            return (false, default, HttpStatusCode.BadRequest);
        }

        var content = new StringContent(JsonSerializer.Serialize(requestModel), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/create-order", content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdOrder = response.IsSuccessStatusCode
            ? JsonSerializer.Deserialize<CreateOrderResponseModel>(responseContent)
            : default;

        return (response.IsSuccessStatusCode, createdOrder, response.StatusCode);
    }

    public async Task<bool> RollbackOrderAsync(Guid transactionId)
    {
        var response = await _httpClient.PostAsJsonAsync("/rollback-order", transactionId);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<bool> CompleteOrderAsync(long orderId)
    {
        var response = await _httpClient.PutAsJsonAsync("/complete-order", orderId);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        return await response.Content.ReadFromJsonAsync<bool>();
    }
}