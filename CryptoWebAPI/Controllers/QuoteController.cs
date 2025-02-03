using CryptoWebAPI.Models;
using CryptoWebAPI.Services;
using CryptoWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace CryptoWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CryptoQuoteController : ControllerBase
{
    private readonly ICryptoQuoteService _cryptoQuoteService;
    private readonly AppSettings _appSettings;


    public CryptoQuoteController(ICryptoQuoteService cryptoQuoteService, IOptions<AppSettings> appSettings)
    {
        _cryptoQuoteService = cryptoQuoteService;
        _appSettings = appSettings.Value;
    }

/// <summary>
/// Used for requesting a token to mock an authorized front-end app
/// </summary>
/// <returns>
/// A JWT token to be used in the GET crypto quote endpoint
/// </returns>
    [HttpPost("requestToken")]
    public IActionResult Login()
    {
        var token = GenerateJwtToken();
        return Ok(token);
    }


/// <summary>
/// Used for liveness probe
/// </summary>
/// <returns>
/// An OKResult if the app is alive 
/// </returns>
    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        return Ok();
    }

/// <summary>
/// Used for getting crypto quotes
/// </summary>
/// <returns>
/// The quotes in different currencies for the user input crypto symbol and the user email.
/// </returns>
    [HttpGet("{cryptoSymbol}")]
    [Authorize(Policy = "UserOnly")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CryptoQuoteResponse), StatusCodes.Status200OK)]

    public async Task<IActionResult> GetQuote(string cryptoSymbol)
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var quote = await _cryptoQuoteService.GetCryptoQuote(cryptoSymbol.ToUpper());

            var response = new CryptoQuoteResponse(userEmail, quote);
            return Ok(response);
        }
        catch (CoinMarketCapException ex)
        {
            return StatusCode((int)ex.StatusCode, new { error = $"Request to get crypto prices failed with the following error: {ex.Message}" });
        }
        catch (ExchangeRateException ex)
        {
            return StatusCode((int)ex.StatusCode, new { error = $"Request to get exhange rates failed with the following error: {ex.Message}" });
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new { error = "An unexpected error occurred", details = ex.Message });
        }
    }


    #region  PrivateMethods
    private string GenerateJwtToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWToken.ConfigurationKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, "testuser@email.com")
            };

        var token = new JwtSecurityToken(
            issuer: _appSettings.JWToken.Issuer,
            audience: _appSettings.JWToken.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    #endregion
}


