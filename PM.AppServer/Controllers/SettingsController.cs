using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PM.AppServer.Models;

namespace PM.AppServer.Controllers
{

[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly AppSettings _appSettings;

    public SettingsController(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public ActionResult<AppSettings> Get()
    {
        return Ok(_appSettings);
    }
}

}