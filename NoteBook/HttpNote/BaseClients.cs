using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoteBook.HttpNote;
public class BaseClients
{
    // 序列化可选项
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        // Text.Json默认区分大小写，有时候反序列化成功，但对象的属性值都丢失时，加上该配置即刻
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory _clientFactory;

    public BaseClients(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<string> OnGet(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);       
        // request.Headers.Add("Accept", "application/vnd.github.v3+json");
        // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            return null;
        }
    }

    public async Task<string> OnPost<TRequest>(string url, TRequest request)
    {
        var client = _clientFactory.CreateClient();

        var json = new StringContent(
        JsonSerializer.Serialize(request, _jsonSerializerOptions),
        Encoding.UTF8,
        "application/json");

        var response = await client.PostAsync(url, json);

        if (response.IsSuccessStatusCode)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            return null;
        }
    }
}
