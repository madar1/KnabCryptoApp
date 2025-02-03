using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CryptoWebAPI.Helpers;
using CryptoWebAPI.Models;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace CryptoWebAPI.Services;

public class CryptoPriceService
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;

    public CryptoPriceService(HttpClient httpClient, IOptions<AppSettings> settings)
    {
        _httpClient = httpClient;
        _appSettings = settings.Value;
        _retryPolicy = RetryPolicyProvider.GetRetryPolicy();
        _circuitBreakerPolicy = RetryPolicyProvider.GetCircuitBreakerPolicy();
    }

    public async Task<decimal> GetCryptoPriceInEUR(string cryptoSymbol)
    {
        try
        {
            var apiKey = _appSettings.CoinMarketCap.ApiKey;
            var requestUri = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={cryptoSymbol}&convert=EUR";

           HttpResponseMessage response = await _retryPolicy.ExecuteAsync(async () =>
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                    request.Headers.Add("X-CMC_PRO_API_KEY", apiKey);
                    request.Headers.Add("Accepts", "application/json");

                    return await _httpClient.SendAsync(request);
                }));

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var jsonResponse = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

            return jsonResponse.GetProperty("data").GetProperty(cryptoSymbol).GetProperty("quote").GetProperty("EUR").GetProperty("price").GetDecimal();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request to get crypto prices failed: {ex.Message}");
            throw new CoinMarketCapException(
                    $"CoinMarketCap API returned an error: {ex.Message}", 
                    ex.StatusCode ?? System.Net.HttpStatusCode.NotImplemented
                );
        }
    }
}