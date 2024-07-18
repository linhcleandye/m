using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.HttpRepository;

public class BasketHttpRepository : IBasketHttpRepository
{
    private readonly HttpClient _client;
    private const string BaseUrl = "baskets";

    public BasketHttpRepository(HttpClient client)
    {
        _client = client;
    }

    public async Task<CartDto> GetBasket(string username)
    {
        var cart = await _client.GetFromJsonAsync<CartDto>($"${BaseUrl}/{username}");
        if (cart == null || !cart.Items.Any()) return null;

        return cart;
    }

    public async Task<bool> DeleteBasket(string username)
    {
        var response = await _client.DeleteAsync($"{BaseUrl}/{username}");
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            throw new Exception($"Delete basket for Username: {username} not success");

        var result = response.IsSuccessStatusCode;
        return result;
    }
}