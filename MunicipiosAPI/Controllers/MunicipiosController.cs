using Microsoft.AspNetCore.Mvc;
using MunicipiosAPI.Service.Interfaces;
using MunicipiosAPI.Domain.Models;
using System.Net;

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

    /// <summary>
    /// Retorna uma lista paginada de municípios de uma UF específica.
    /// </summary>
    /// <param name="uf">Sigla da unidade federativa (ex: SP, RJ, RS).</param>
    /// <param name="page">Número da página (padrão: 1).</param>
    /// <param name="pageSize">Quantidade de itens por página (padrão: 20).</param>
    /// <returns>Lista paginada de municípios.</returns>
    /// <response code="200">Retorna os municípios paginados</response>
    /// <response code="400">UF inválida</response>
    /// <response code="500">Erro interno no servidor</response>
    [HttpGet("{uf}")]
    [ProducesResponseType(typeof(PagedResult<MunicipioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<MunicipioResponse>>> Get(
        string uf,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _service.GetMunicipiosAsync(uf, page, pageSize);
        return Ok(result);
    }
}
