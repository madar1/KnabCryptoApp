public class AppSettings
{
    public string ApplicationName { get; set; }
    public string Version { get; set; }
    public CoinMarketCap CoinMarketCap { get; set; }
    public JWToken JWToken { get; set; }
    public GoogleOAuth GoogleOAuth { get; set; }
}

public partial class CoinMarketCap
{
    public string ApiKey{ get; set; }
}

public partial class JWToken
{
    public string Authority{ get; set; }
    public string Audience{ get; set; }
}

public partial class GoogleOAuth
{
    public string ClientId{ get; set; }
    public string ClientSecret{ get; set; }
    public string CallbackPath{ get; set; }
}