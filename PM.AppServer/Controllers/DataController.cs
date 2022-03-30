using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PM.AppServer.Models.Data;
using PM.AppServer.Services;

namespace PM.AppServer.Controllers
{

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly IEnumerable<PlagueDataType> _plagueDataTypes;
    private readonly IPlagueDataService _dataService;

    public DataController(IOptions<List<PlagueDataType>> plagueDataTypes, IPlagueDataService dataService)
    {
        _plagueDataTypes = plagueDataTypes.Value;
        _dataService = dataService;
    }

    [HttpGet("types")]
    public ActionResult<IEnumerable<PlagueDataType>> ListDataTypes()
    {
        return Ok(_plagueDataTypes);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlagueData>>> ListData([FromQuery] string tokenPath)
    {
        return Ok(await _dataService.List(tokenPath));
    }
}

}