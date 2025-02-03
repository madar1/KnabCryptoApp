
namespace CryptoWebAPI.Models;

public class AppSettings
{
    public string ApplicationName { get; set; }
    public string Version { get; set; }
    public CoinMarketCap CoinMarketCap { get; set; }
    public ExchangeRates ExchangeRates { get; set; }
    public JWToken JWToken { get; set; }
}

public partial class CoinMarketCap
{
    public string ApiKey{ get; set; }
}

public partial class ExchangeRates
{
    public string ApiKey{ get; set; }
}

public partial class JWToken
{
    public string ConfigurationKey{ get; set; }
    public string Issuer{ get; set; }
    public string Audience{ get; set; }
}