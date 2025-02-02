using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Requires authentication
public class CryptoQuoteController : ControllerBase
{
    private readonly CryptoQuoteService _cryptoQuoteService;

    public CryptoQuoteController(CryptoQuoteService cryptoQuoteService)
    {
        _cryptoQuoteService = cryptoQuoteService;
    }

    [HttpGet("{cryptoSymbol}")]
    public async Task<IActionResult> GetQuote(string cryptoSymbol)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        var result = await _cryptoQuoteService.GetCryptoQuote(cryptoSymbol.ToUpper());
        return Ok(new { requestedBy = userEmail, quote = result });
    }

     [HttpGet]
    public async Task<IActionResult> Ping()
    {
        return Ok();
    }
}