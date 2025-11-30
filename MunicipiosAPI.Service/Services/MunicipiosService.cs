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

    public async Task<PagedResult<MunicipioResponse>> GetMunicipiosAsync(string uf, int page, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(uf))
            throw new ArgumentException("UF não pode ser vazia.");

        uf = uf.Trim().ToUpper();

        string cacheKey = $"municipios_{uf}";

        // Cache
        if (!_cache.TryGetValue(cacheKey, out List<MunicipioResponse>? municipios))
        {
            municipios = await _provider.GetMunicipiosAsync(uf);
            _cache.Set(cacheKey, municipios, TimeSpan.FromMinutes(10));
        }

        int totalItems = municipios.Count;
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        page = Math.Max(1, page);
        page = Math.Min(page, totalPages);

        var items = municipios
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<MunicipioResponse>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = items
        };
    }

}
