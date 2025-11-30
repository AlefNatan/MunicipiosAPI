using Microsoft.AspNetCore.Mvc;
using MunicipiosAPI.Service.Interfaces;
using MunicipiosAPI.Domain.Models;

namespace MunicipiosAPI.Controllers;

[ApiController]
[Route("municipios")]
public class MunicipiosController : ControllerBase
{
    private readonly IMunicipiosService _service;

    public MunicipiosController(IMunicipiosService service)
    {
        _service = service;
    }

    [HttpGet("{uf}")]
    public async Task<ActionResult<PagedResult<MunicipioResponse>>> Get(
        string uf,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _service.GetMunicipiosAsync(uf, page, pageSize);
        return Ok(result);
    }

}
