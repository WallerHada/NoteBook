using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoteBook.HttpNote;
public class TypedClients
{
    // 相较于 private static readonly HttpClient client = new HttpClient();
    //        ...省略代码
    //        ...省略代码
    // 因为是复用的HttpClient，那么一些公共的设置就没办法灵活的调整了，如请求头的自定义。
    // 因为HttpClient请求每个url时，会缓存该url对应的主机ip，从而会导致DNS更新失效（TTL失效了）

    // 所以更新为注册 IHttpClientFactory 并将其用于配置和创建应用中的 HttpClient 实例
    // 顾名思义HttpClientFactory就是HttpClient的工厂，内部已经帮我们处理好了对HttpClient的管理，不需要我们人工进行对象释放，同时，支持自定义请求头，支持DNS更新等等等。

    // 使用，只需在要用到的地方进行DI即刻 

    // 序列化可选项
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        //IgnoreNullValues = true,

        // Text.Json默认区分大小写，有时候反序列化成功，但对象的属性值都丢失时，加上该配置即刻
        PropertyNameCaseInsensitive = true
        // 驼峰
        // PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // 防止中文序列化成unicode
        // Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
        // 将枚举序列化为名称字符串而不是数值
        // Converters = { new JsonStringEnumConverter()}
    };

    public HttpClient Client { get; }

    public TypedClients(HttpClient client)
    {
        client.BaseAddress = new Uri("https://localhost:5001");
        // client.DefaultRequestHeaders.Add("Accept", "");
        // client.DefaultRequestHeaders.Add("User-Agent", "");
        client.Timeout = TimeSpan.FromSeconds(5);
        Client = client;
    }

    public async Task<string> HttpPost<TRequest>(string uri, TRequest request)
    {
        var json = new StringContent(
            JsonSerializer.Serialize(request, _jsonSerializerOptions),
            Encoding.UTF8, "application/json");

        using var response = await Client.PostAsync(uri, json);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            return $"StatusCode: {response.StatusCode}";
        }
    }

    public async Task<string> HttpGet<TRequest>(string uri, TRequest request)
    {
        // 对象转字典
        var dic = request.GetType().GetProperties().ToDictionary(propInfo => propInfo.Name, propInfo => propInfo.GetValue(request, null));
        // 字典转字符串
        var strParam = string.Join("&", dic.Select(o => o.Key + "=" + o.Value));
        // 拼接连接
        uri = string.Concat(uri, '?', strParam);

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);

        var response = await Client.SendAsync(httpRequest);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            return $"StatusCode: {response.StatusCode}";
        }
    }

    public async Task<TResult> HttpPost<TRequest, TResult>(string uri, TRequest request)
    {
        var json = new StringContent(
            JsonSerializer.Serialize(request, _jsonSerializerOptions),
            Encoding.UTF8, "application/json");

        using var response = await Client.PostAsync(uri, json);

        response.EnsureSuccessStatusCode();

        using var responseStream = await response.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<TResult>(responseStream, _jsonSerializerOptions);
    }

    public async Task<TResult> HttpGet<TRequest, TResult>(string uri, TRequest request)
    {
        // 对象转字典
        var dic = request.GetType().GetProperties().ToDictionary(propInfo => propInfo.Name, propInfo => propInfo.GetValue(request, null));
        // 字典转字符串
        var strParam = string.Join("&", dic.Select(o => o.Key + "=" + o.Value));
        // 拼接连接
        uri = string.Concat(uri, '?', strParam);

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);

        var response = await Client.SendAsync(httpRequest);

        response.EnsureSuccessStatusCode();

        using var responseStream = await response.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<TResult>(responseStream, _jsonSerializerOptions);
    }
}
