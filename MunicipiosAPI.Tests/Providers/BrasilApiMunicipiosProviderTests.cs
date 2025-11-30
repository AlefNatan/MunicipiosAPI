using FluentAssertions;
using MunicipiosAPI.Domain.Models;
using MunicipiosAPI.Providers.Providers;
using RichardSzalay.MockHttp;
using System.Net;

namespace MunicipiosAPI.Tests.Providers;

public class BrasilApiMunicipiosProviderTests
{
    [Fact]
    public async Task Deve_Mapear_Resposta_Da_BrasilAPI()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp
            .When("https://brasilapi.com.br/api/ibge/municipios/v1/SP")
            .Respond("application/json", """
            [
              { "nome": "CidadeTeste", "codigo_ibge": "000111" }
            ]
            """);

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("https://brasilapi.com.br/api/ibge/municipios/v1/");

        var provider = new BrasilApiMunicipiosProvider(client);

        var result = await provider.GetMunicipiosAsync("SP");

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("CidadeTeste");
        result[0].IbgeCode.Should().Be("000111");
    }

    [Fact]
    public async Task Deve_Retornar_Lista_Vazia_Quando_Resposta_For_Vazia()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.When("*")
                .Respond(HttpStatusCode.OK, "application/json", "[]");

        var client = mockHttp.ToHttpClient();

        var provider = new BrasilApiMunicipiosProvider(client);

        var result = await provider.GetMunicipiosAsync("SP");

        result.Should().BeEmpty();
    }
}