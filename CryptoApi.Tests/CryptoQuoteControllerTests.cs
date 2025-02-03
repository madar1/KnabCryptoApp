using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using CryptoWebAPI.Controllers;
using CryptoWebAPI.Models;
using CryptoWebAPI.Services;
using System;
using System.Net;
using CryptoWebAPI.Services.Interfaces;

namespace CryptoApi.Tests;
public class CryptoQuoteControllerTests
{
    private readonly Mock<ICryptoQuoteService> _mockCryptoQuoteService;
    private readonly CryptoQuoteController _controller;
    private readonly Mock<IOptions<AppSettings>> _mockAppSettings;

    public CryptoQuoteControllerTests()
    {
        _mockCryptoQuoteService = new Mock<ICryptoQuoteService>();
        _mockAppSettings = new Mock<IOptions<AppSettings>>();

        _mockAppSettings.Setup(x => x.Value).Returns(new AppSettings
        {
            JWToken = new JWToken
            {
                ConfigurationKey = "test_secret_key_test_secret_key_12345678901234567890",
                Issuer = "test_issuer",
                Audience = "test_audience"
            }
        });

        _controller = new CryptoQuoteController(_mockCryptoQuoteService.Object, _mockAppSettings.Object);
    }

  
    [Fact]
    public void Login_ReturnsJwtToken()
    {
        // Act
        var result = _controller.Login();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<string>(okResult.Value);

        Assert.NotNull(response);
    }


    [Fact]
    public async Task Ping_ReturnsOk()
    {
        // Act
        var result = await _controller.Ping();

        // Assert
        Assert.IsType<OkResult>(result);
    }


    [Fact]
    public async Task GetQuote_ValidCryptoSymbol_ReturnsCryptoQuoteResponse()
    {
        // Arrange
        var cryptoSymbol = "BTC";
        var expectedQuote = new Dictionary<string, decimal>
        {
            { "USD", 42000m },
            { "EUR", 38000m },
            { "BRL", 210000m },
            { "GBP", 36000m },
            { "AUD", 58000m }
        };

        _mockCryptoQuoteService.Setup(s => s.GetCryptoQuote(cryptoSymbol))
                               .ReturnsAsync(expectedQuote);

        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "testuser@email.com")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = mockUser }
        };

        // Act
        var result = await _controller.GetQuote(cryptoSymbol);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CryptoQuoteResponse>(okResult.Value);

        Assert.Equal("testuser@email.com", response.RequestedBy);
        Assert.Equal(expectedQuote, response.Quote);
    }


    [Fact]
    public async Task GetQuote_CoinMarketCapException_ReturnsBadRequest()
    {
        // Arrange
        var cryptoSymbol = "BTC";
        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "testuser@email.com")
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = mockUser }
        };
        _mockCryptoQuoteService.Setup(s => s.GetCryptoQuote(cryptoSymbol))
                               .ThrowsAsync(new CoinMarketCapException("API error", HttpStatusCode.BadRequest));

        // Act
        var result = await _controller.GetQuote(cryptoSymbol);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        Assert.Contains("Request to get crypto prices failed", objectResult.Value.ToString());
    }


    [Fact]
    public async Task GetQuote_ExchangeRateException_ReturnsBadGateway()
    {
        // Arrange
        var cryptoSymbol = "BTC";
        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "testuser@email.com")
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = mockUser }
        };
        _mockCryptoQuoteService.Setup(s => s.GetCryptoQuote(cryptoSymbol))
                               .ThrowsAsync(new ExchangeRateException("Exchange API error", HttpStatusCode.BadGateway));

        // Act
        var result = await _controller.GetQuote(cryptoSymbol);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.BadGateway, objectResult.StatusCode);
        Assert.Contains("Exchange API error", objectResult.Value.ToString());
    }


    [Fact]
    public async Task GetQuote_UnhandledException_ReturnsInternalServerError()
    {
        // Arrange
        var cryptoSymbol = "BTC";
        _mockCryptoQuoteService.Setup(s => s.GetCryptoQuote(cryptoSymbol))
                               .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _controller.GetQuote(cryptoSymbol);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        Assert.Contains("An unexpected error occurred", objectResult.Value.ToString());
    }
}
