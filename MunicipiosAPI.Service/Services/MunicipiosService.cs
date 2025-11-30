using Microsoft.Extensions.Caching.Memory;
using MunicipiosAPI.Domain.Interfaces;
using MunicipiosAPI.Domain.Models;
using MunicipiosAPI.Service.Interfaces;

namespace MunicipiosAPI.Service.Services;

public class MunicipiosService : IMunicipiosService
{
    private readonly IProviderMunicipios _provider;
    private readonly IMemoryCache _cache;

    public MunicipiosService(IProviderMunicipios provider, IMemoryCache cache)
    {
        _provider = provider;
        _cache = cache;
    }

    public async Task<List<MunicipioResponse>> GetMunicipiosAsync(string uf)
    {
        if (string.IsNullOrWhiteSpace(uf))
            throw new ArgumentException("UF não pode ser vazia.");

        uf = uf.Trim().ToUpper();

        string cacheKey = $"municipios_{uf}";

        // Tenta pegar do cache
        if (_cache.TryGetValue(cacheKey, out List<MunicipioResponse>? cached) && cached != null)
            return cached;

        // Chama o provider configurado
        var municipios = await _provider.GetMunicipiosAsync(uf);

        // Armazena no cache por 10 minutos
        _cache.Set(cacheKey, municipios, TimeSpan.FromMinutes(10));

        return municipios;
    }
}
