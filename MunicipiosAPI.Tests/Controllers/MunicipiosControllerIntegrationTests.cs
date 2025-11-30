using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MunicipiosAPI.Domain.Interfaces;
using MunicipiosAPI.Domain.Models;
using MunicipiosAPI.Providers.Providers;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System.Net;

namespace MunicipiosAPI.Tests.Controllers;

public class MunicipiosControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MunicipiosControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        Environment.SetEnvironmentVariable("PROVIDER_MUNICIPIOS", "BRASIL_API");
        _factory = factory;
    }

    [Fact]
    public async Task GetMunicipios_Deve_Retornar_Municipios_Usando_BrasilApi()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp
            .When("https://brasilapi.com.br/api/ibge/municipios/v1/RS")
            .Respond("application/json", """
            [
              { "nome": "Porto Alegre", "codigo_ibge": "4314902" }
            ]
            """);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var remove = services.FirstOrDefault(s => s.ServiceType == typeof(IProviderMunicipios));
                if (remove != null)
                    services.Remove(remove);

                services.AddHttpClient<IProviderMunicipios, BrasilApiMunicipiosProvider>()
                        .ConfigurePrimaryHttpMessageHandler(() => mockHttp);
            });
        }).CreateClient();

        var response = await client.GetAsync("/municipios/RS?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<PagedResult<MunicipioResponse>>(json);

        data!.Items.Should().HaveCount(1);
        data.Items[0].Name.Should().Be("Porto Alegre");
        data.Items[0].IbgeCode.Should().Be("4314902");
    }

    [Fact]
    public async Task GetMunicipios_Deve_Retornar_BadRequest_Se_UF_Invalida()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/municipios/XYZ999?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
