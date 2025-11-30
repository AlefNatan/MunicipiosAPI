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
    public async Task<ActionResult<List<MunicipioResponse>>> Get(string uf)
    {
        var result = await _service.GetMunicipiosAsync(uf);
        return Ok(result);
    }
}
