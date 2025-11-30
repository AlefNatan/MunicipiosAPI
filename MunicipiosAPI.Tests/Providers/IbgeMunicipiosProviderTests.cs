using FluentAssertions;
using MunicipiosAPI.Providers.Providers;
using RichardSzalay.MockHttp;
using System.Net;

namespace MunicipiosAPI.Tests.Providers;

public class IbgeMunicipiosProviderTests
{
    [Fact]
    public async Task Deve_Mapear_Resposta_Do_IBGE()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp
            .When("https://servicodados.ibge.gov.br/api/v1/localidades/estados/SP/municipios")
            .Respond("application/json", """
            [
              { "id": 12345, "nome": "CidadeIBGE" }
            ]
            """);

        var client = mockHttp.ToHttpClient();

        var provider = new IbgeMunicipiosProvider(client);

        var result = await provider.GetMunicipiosAsync("SP");

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("CidadeIBGE");
        result[0].IbgeCode.Should().Be("12345");
    }

    [Fact]
    public async Task Deve_Retornar_Lista_Vazia_Quando_Resposta_For_Vazia()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.When("*")
                .Respond(HttpStatusCode.OK, "application/json", "[]");

        var client = mockHttp.ToHttpClient();

        var provider = new IbgeMunicipiosProvider(client);

        var result = await provider.GetMunicipiosAsync("SP");

        result.Should().BeEmpty();
    }
}
