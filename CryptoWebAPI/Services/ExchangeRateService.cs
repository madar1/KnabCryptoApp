using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

public class ExchangeRateService
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;

    public ExchangeRateService(HttpClient httpClient, IOptions<AppSettings> appSettings)
    {
        _httpClient = httpClient;
        _appSettings = appSettings.Value;
    }

    public async Task<Dictionary<string, decimal>> GetExchangeRates()
    {
        var requestUri = $"https://api.exchangeratesapi.io/latest?base=USD&apikey={_apiKey}";
        var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        var jsonResponse = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

        return new Dictionary<string, decimal>
        {
            { "EUR", jsonResponse.GetProperty("rates").GetProperty("EUR").GetDecimal() },
            { "BRL", jsonResponse.GetProperty("rates").GetProperty("BRL").GetDecimal() },
            { "GBP", jsonResponse.GetProperty("rates").GetProperty("GBP").GetDecimal() },
            { "AUD", jsonResponse.GetProperty("rates").GetProperty("AUD").GetDecimal() }
        };
    }
}