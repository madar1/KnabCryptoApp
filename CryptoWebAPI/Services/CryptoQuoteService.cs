using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoWebAPI.Services.Interfaces;

namespace CryptoWebAPI.Services;

public class CryptoQuoteService: ICryptoQuoteService
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
        decimal priceInEUR = await _cryptoPriceService.GetCryptoPriceInEUR(cryptoSymbol);         
        var exchangeRates = await _exchangeRateService.GetExchangeRates();

        return new Dictionary<string, decimal>
        {
            { "EUR", priceInEUR },
            { "USD", priceInEUR * exchangeRates["USD"] },
            { "BRL", priceInEUR * exchangeRates["BRL"] },
            { "GBP", priceInEUR * exchangeRates["GBP"] },
            { "AUD", priceInEUR * exchangeRates["AUD"] }
        };
    }
}