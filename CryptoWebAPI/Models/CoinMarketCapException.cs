using System.Net;

namespace CryptoWebAPI.Models;

public class CoinMarketCapException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public CoinMarketCapException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public CoinMarketCapException(string message, HttpStatusCode statusCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
