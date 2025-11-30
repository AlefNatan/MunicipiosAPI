using MunicipiosAPI.Domain.Models;

namespace MunicipiosAPI.Domain.Interfaces;

public interface IProviderMunicipios
{
    Task<List<MunicipioResponse>> GetMunicipiosAsync(string uf);
}
