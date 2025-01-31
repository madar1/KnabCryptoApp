using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[Route("api/config")]
[ApiController]
public class QuoteController : ControllerBase
{
    private readonly AppSettings _appSettings;

    public QuoteController(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult GetConfig()
    {
        return Ok(_appSettings);
    }
}