using System.Net;

namespace CryptoWebAPI.Models;

public class ExchangeRateException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ExchangeRateException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public ExchangeRateException(string message, HttpStatusCode statusCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
