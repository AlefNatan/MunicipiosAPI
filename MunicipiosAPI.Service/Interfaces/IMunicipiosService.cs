using MunicipiosAPI.Domain.Models;

namespace MunicipiosAPI.Service.Interfaces;

public interface IMunicipiosService
{
    Task<PagedResult<MunicipioResponse>> GetMunicipiosAsync(string uf, int page, int pageSize);
}
