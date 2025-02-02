using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class CryptoPriceService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public CryptoPriceService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["CoinMarketCap:ApiKey"];
    }

    public async Task<decimal> GetCryptoPriceInUSD(string cryptoSymbol)
    {
        var requestUri = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={cryptoSymbol}&convert=USD";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("X-CMC_PRO_API_KEY", _apiKey);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        var jsonResponse = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

        return jsonResponse.GetProperty("data").GetProperty(cryptoSymbol).GetProperty("quote").GetProperty("USD").GetProperty("price").GetDecimal();
    }
}