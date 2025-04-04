#pragma warning disable CS1591

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace ClientApp.Api
{
  /// <summary>
  /// Класс для работы с api.
  /// </summary>
  public class CommonApi
  {
    private readonly HttpClient client;

    private readonly JsonSerializerOptions options = new()
    {
      WriteIndented = true,
      Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
#pragma warning disable SYSLIB0020
      IgnoreNullValues = true
#pragma warning restore SYSLIB0020
    };

    public static T GetEmptyObject<T>()
    {
      if (typeof(T).IsValueType || typeof(T) == typeof(string)) {
        return default;
      }

      return (T)Activator.CreateInstance(typeof(T));
    }

    public async Task<TResult> GetFromJsonAsync<TResult>(string requestUri)
    {
      return await this.client.GetFromJsonAsync<TResult>(requestUri, this.options) ?? GetEmptyObject<TResult>();
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
      return await this.client.GetAsync(requestUri);
    }


    public async Task<string> GetAsString(string requestUri)
    {
      var response = await this.client.GetAsync(requestUri);
      response.EnsureSuccessStatusCode();

      return await response.Content.ReadAsStringAsync();
    }

    public async Task<TResult> PostAsJsonAsync<TValue, TResult>(string requestUri, TValue value)
    {
      var response = await this.client.PostAsJsonAsync(requestUri, value, this.options);
      response.EnsureSuccessStatusCode();
      string contentAsString = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<TResult>(contentAsString, this.options) ?? GetEmptyObject<TResult>();
    }

    public async Task<TResult> PostAsJsonAsync<TResult>(string requestUri)
    {
      var response = await this.client.PostAsJsonAsync(requestUri, this.options);
      response.EnsureSuccessStatusCode();
      string contentAsString = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<TResult>(contentAsString, this.options) ?? GetEmptyObject<TResult>();
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string requestUri, TValue value, HttpStatusCode? availableStatusCode = null)
    {
      var response = await this.client.PostAsJsonAsync(requestUri, value, this.options);
      if (availableStatusCode != null && response.StatusCode == availableStatusCode)
        return response;
      response.EnsureSuccessStatusCode();

      return response;
    }

    public CommonApi(HttpClient client)
    {
      this.client = client;
    }
  }
}
