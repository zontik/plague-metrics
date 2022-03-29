using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PM.AppServer.Models.Data;
using PM.AppServer.Services;

namespace PM.AppServer.Controllers
{

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly IPlagueDataTypesService _typesService;
    private readonly IPlagueDataService _dataService;

    public DataController(IPlagueDataTypesService typesService, IPlagueDataService dataService)
    {
        _typesService = typesService;
        _dataService = dataService;
    }

    [HttpGet("types")]
    public ActionResult<IEnumerable<PlagueDataType>> ListDataTypes()
    {
        return Ok(_typesService.List());
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlagueData>> ListData([FromQuery] string typeKey)
    {
        return Ok(_dataService.List(typeKey));
    }
}

}