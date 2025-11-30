using Newtonsoft.Json;

namespace MunicipiosAPI.Providers.Http;

public abstract class HttpProviderBase
{
    protected readonly HttpClient _httpClient;

    protected HttpProviderBase(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<T> GetAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Erro ao chamar API externa. StatusCode: {response.StatusCode}. Conteúdo: {content}");
        }

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<T>(json);
        if (result == null)
            throw new Exception($"Falha ao desserializar o JSON retornado pela URL '{url}'.");

        return result;
    }
}
