using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PM.AppServer.Services.Base;
using PM.Model.Data;

namespace PM.AppServer.Controllers
{

[ApiController]
[Route("api/plague_data")]
public class PlagueDataController : ControllerBase
{
    private readonly IEnumerable<PlagueDataType> _plagueDataTypes;
    private readonly IPlagueDataService _plagueDataService;

    public PlagueDataController(IOptions<List<PlagueDataType>> plagueDataTypes, IPlagueDataService plagueDataService)
    {
        _plagueDataTypes = plagueDataTypes.Value;
        _plagueDataService = plagueDataService;
    }

    [HttpGet("types")]
    public ActionResult<IEnumerable<PlagueDataType>> ListDataTypes()
    {
        return Ok(_plagueDataTypes);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlagueData>>> ListData([FromQuery] string tokenPath)
    {
        return Ok(await _plagueDataService.ListData(tokenPath));
    }
}

}