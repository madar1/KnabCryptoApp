using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CryptoWebAPI.Helpers;
using CryptoWebAPI.Models;
using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace CryptoWebAPI.Services;

public class ExchangeRateService
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;

    public ExchangeRateService(HttpClient httpClient, IOptions<AppSettings> settings)
    {
        _httpClient = httpClient;
        _appSettings = settings.Value;
        _retryPolicy = RetryPolicyProvider.GetRetryPolicy();
        _circuitBreakerPolicy = RetryPolicyProvider.GetCircuitBreakerPolicy();
    }

    public async Task<Dictionary<string, decimal>> GetExchangeRates()
    {
        try
        {
            var apiKey = _appSettings.ExchangeRates.ApiKey;
            var requestUri = $"http://api.exchangeratesapi.io/v1/latest?access_key={apiKey}&symbols=USD,BRL,GBP,AUD";

            HttpResponseMessage response = await _retryPolicy.ExecuteAsync(async () =>
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri);

                    return await _httpClient.SendAsync(request);
                }));

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var jsonResponse = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

            return new Dictionary<string, decimal>
            {
                { "USD", jsonResponse.GetProperty("rates").GetProperty("USD").GetDecimal() },
                { "BRL", jsonResponse.GetProperty("rates").GetProperty("BRL").GetDecimal() },
                { "GBP", jsonResponse.GetProperty("rates").GetProperty("GBP").GetDecimal() },
                { "AUD", jsonResponse.GetProperty("rates").GetProperty("AUD").GetDecimal() }
            };
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request to get exhange rates failed: {ex.Message}");
            throw new ExchangeRateException(
                    $"Exchange rates API returned an error: {ex.Message}", 
                    ex.StatusCode ?? System.Net.HttpStatusCode.NotImplemented
                );
        }
    }
}