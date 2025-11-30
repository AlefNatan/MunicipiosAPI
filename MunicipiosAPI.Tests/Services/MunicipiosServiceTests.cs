using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using MunicipiosAPI.Domain.Interfaces;
using MunicipiosAPI.Domain.Models;
using MunicipiosAPI.Service.Services;

namespace MunicipiosAPI.Tests.Services;

public class MunicipiosServiceTests
{
    private readonly Mock<IProviderMunicipios> _provider;
    private readonly IMemoryCache _cache;
    private readonly MunicipiosService _service;

    public MunicipiosServiceTests()
    {
        _provider = new Mock<IProviderMunicipios>();
        _cache = new MemoryCache(new MemoryCacheOptions());

        _service = new MunicipiosService(_provider.Object, _cache);
    }

    [Fact]
    public async Task Deve_Retornar_Dados_Do_Provider()
    {
        var expected = new List<MunicipioResponse>
        {
            new() { Name = "City", IbgeCode = "123" }
        };

        _provider.Setup(p => p.GetMunicipiosAsync("SP"))
                 .ReturnsAsync(expected);

        var result = await _service.GetMunicipiosAsync("SP", 1, 10);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("City");
    }

    [Fact]
    public async Task Deve_Usar_Cache_Quando_Disponivel()
    {
        var cached = new List<MunicipioResponse>
        {
            new() { Name = "CachedCity", IbgeCode = "999" }
        };

        _cache.Set("municipios_SP", cached);

        var result = await _service.GetMunicipiosAsync("SP", 1, 10);

        result.Items[0].Name.Should().Be("CachedCity");
        _provider.Verify(p => p.GetMunicipiosAsync(It.IsAny<string>()), Times.Never);
    }
}
