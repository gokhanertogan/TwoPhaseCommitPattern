using System.Net;
using System.Text;
using System.Text.Json;
using COORDINATOR.API.Models;

namespace COORDINATOR.API.Clients;

public class PaymentApiClient(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(OrderApiClient));

    public async Task<(bool IsSuccess, bool result, HttpStatusCode StatusCode)> CheckPaymentAsync()
    {
        var response = await _httpClient.GetAsync("/check-payment");
        var content = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, JsonSerializer.Deserialize<bool>(content), response.StatusCode);
    }

    public async Task<(bool IsSuccess, bool Response, HttpStatusCode StatusCode)> CreatePaymentAsync(CreatePaymentDto requestModel)
    {
        if (requestModel == null)
        {
            return (false, default, HttpStatusCode.BadRequest);
        }

        var content = new StringContent(JsonSerializer.Serialize(requestModel), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/create-payment", content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = response.IsSuccessStatusCode && JsonSerializer.Deserialize<bool>(responseContent);

        return (response.IsSuccessStatusCode, result, response.StatusCode);
    }

    public async Task<bool> RollbackPaymentAsync(Guid transactionId)
    {
        var response = await _httpClient.PostAsJsonAsync("/rollback-payment", transactionId);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        return await response.Content.ReadFromJsonAsync<bool>();
    }
}