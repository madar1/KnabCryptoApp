public class AppSettings
{
    public string ApplicationName { get; set; }
    public string Version { get; set; }
    public CoinMarketCap CoinMarketCap { get; set; }
}

public partial class CoinMarketCap
{
    public string ApiKey{ get; set; }
}