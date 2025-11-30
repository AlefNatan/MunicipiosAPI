using MunicipiosAPI.Domain.Interfaces;
using MunicipiosAPI.Domain.Models;
using MunicipiosAPI.Providers.Http;

namespace MunicipiosAPI.Providers.Providers;

public class BrasilApiMunicipiosProvider : HttpProviderBase, IProviderMunicipios
{
    public BrasilApiMunicipiosProvider(HttpClient httpClient) : base(httpClient)
    {
        _httpClient.BaseAddress = new Uri("https://brasilapi.com.br/api/ibge/municipios/v1/");
    }

    public async Task<List<MunicipioResponse>> GetMunicipiosAsync(string uf)
    {
        var data = await GetAsync<List<BrasilApiMunicipioDto>>(uf);

        return data.Select(m => new MunicipioResponse
        {
            Name = m.nome,
            IbgeCode = m.codigo_ibge
        }).ToList();
    }

    private class BrasilApiMunicipioDto
    {
        public string? nome { get; set; }
        public string? codigo_ibge { get; set; }
    }
}
