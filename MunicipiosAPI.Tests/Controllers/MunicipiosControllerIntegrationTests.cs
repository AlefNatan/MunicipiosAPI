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

    [Fact]
    public async Task GetMunicipios_Deve_Usar_Provider_IBGE_E_Retornar_Resultado()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp
            .When("https://servicodados.ibge.gov.br/api/v1/localidades/estados/RS/municipios")
            .Respond("application/json", """
        [
          { "id": 4314902, "nome": "Porto Alegre" }
        ]
        """);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var remove = services.FirstOrDefault(s => s.ServiceType == typeof(IProviderMunicipios));
                if (remove != null)
                    services.Remove(remove);

                services.AddHttpClient<IProviderMunicipios, IbgeMunicipiosProvider>()
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
    public async Task GetMunicipios_Deve_Retornar_Estrutura_Paginada_Correta()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp
            .When("https://brasilapi.com.br/api/ibge/municipios/v1/SP")
            .Respond("application/json", """
        [
          { "nome": "São Paulo", "codigo_ibge": "3550308" },
          { "nome": "Guarulhos", "codigo_ibge": "3518800" }
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

        var response = await client.GetAsync("/municipios/SP?page=1&pageSize=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<PagedResult<MunicipioResponse>>(json);

        data!.Page.Should().Be(1);
        data.PageSize.Should().Be(1);
        data.TotalItems.Should().Be(2);
        data.TotalPages.Should().Be(2);
        data.Items.Should().HaveCount(1);
    }


}
