using MunicipiosAPI.Domain.Models;

namespace MunicipiosAPI.Service.Interfaces;

public interface IMunicipiosService
{
    Task<List<MunicipioResponse>> GetMunicipiosAsync(string uf);
}
