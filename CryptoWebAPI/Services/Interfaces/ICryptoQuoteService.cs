namespace CryptoWebAPI.Services.Interfaces;
public interface ICryptoQuoteService
{
    Task<Dictionary<string, decimal>> GetCryptoQuote(string cryptoSymbol);
}

