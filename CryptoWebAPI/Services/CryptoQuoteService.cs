using System.Collections.Generic;
using System.Threading.Tasks;

public class CryptoQuoteService
{
    private readonly CryptoPriceService _cryptoPriceService;
    private readonly ExchangeRateService _exchangeRateService;

    public CryptoQuoteService(CryptoPriceService cryptoPriceService, ExchangeRateService exchangeRateService)
    {
        _cryptoPriceService = cryptoPriceService;
        _exchangeRateService = exchangeRateService;
    }

    public async Task<Dictionary<string, decimal>> GetCryptoQuote(string cryptoSymbol)
    {
        decimal priceInUSD = await _cryptoPriceService.GetCryptoPriceInUSD(cryptoSymbol);
        var exchangeRates = await _exchangeRateService.GetExchangeRates();

        return new Dictionary<string, decimal>
        {
            { "USD", priceInUSD },
            { "EUR", priceInUSD * exchangeRates["EUR"] },
            { "BRL", priceInUSD * exchangeRates["BRL"] },
            { "GBP", priceInUSD * exchangeRates["GBP"] },
            { "AUD", priceInUSD * exchangeRates["AUD"] }
        };
    }
}