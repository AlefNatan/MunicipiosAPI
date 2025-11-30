using MunicipiosAPI.Domain.Interfaces;
using MunicipiosAPI.Domain.Models;
using MunicipiosAPI.Providers.Http;

namespace MunicipiosAPI.Providers.Providers;

public class IbgeMunicipiosProvider : HttpProviderBase, IProviderMunicipios
{
    public IbgeMunicipiosProvider(HttpClient httpClient) : base(httpClient)
    {
        _httpClient.BaseAddress = new Uri("https://servicodados.ibge.gov.br/api/v1/localidades/estados/");
    }

    public async Task<List<MunicipioResponse>> GetMunicipiosAsync(string uf)
    {
        var endpoint = $"{uf}/municipios";
        var data = await GetAsync<List<IbgeMunicipioDto>>(endpoint);

        return data.Select(m => new MunicipioResponse
        {
            Name = m.nome,
            IbgeCode = m.id.ToString()
        }).ToList();
    }

    private class IbgeMunicipioDto
    {
        public int id { get; set; }
        public string? nome { get; set; }
    }
}
