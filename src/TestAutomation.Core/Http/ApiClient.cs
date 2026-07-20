using System.Net.Http.Json;
using System.Text.Json;

namespace TestAutomation.Core.Http;

/// <summary>
/// Lightweight HTTP helper that wraps <see cref="HttpClient"/> for REST API tests.
/// Exposes typed GET, POST, PUT, PATCH and DELETE methods that return the raw
/// <see cref="HttpResponseMessage"/> so tests can assert on both status codes
/// and deserialised response bodies.
///
/// A single <see cref="HttpClient"/> is accepted via the constructor to allow
/// tests to share or isolate client instances as needed.
/// </summary>
public sealed class ApiClient
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Initialises a new <see cref="ApiClient"/> with the supplied <see cref="HttpClient"/>.
    /// The <paramref name="httpClient"/> base address should already be set by the caller.
    /// </summary>
    public ApiClient(HttpClient httpClient)
    {
        _http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    // -------------------------------------------------------------------------
    // GET
    // -------------------------------------------------------------------------

    /// <summary>Returns the raw <see cref="HttpResponseMessage"/> for a GET request.</summary>
    public async Task<HttpResponseMessage> GetAsync(string endpoint) =>
        await _http.GetAsync(endpoint);

    /// <summary>
    /// Sends a GET request and deserialises the JSON response body to
    /// <typeparamref name="T"/>.
    /// </summary>
    public async Task<T?> GetAsync<T>(string endpoint) =>
        await _http.GetFromJsonAsync<T>(endpoint, JsonOptions);

    // -------------------------------------------------------------------------
    // POST
    // -------------------------------------------------------------------------

    /// <summary>Returns the raw response for a POST request with a JSON body.</summary>
    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T payload) =>
        await _http.PostAsJsonAsync(endpoint, payload, JsonOptions);

    // -------------------------------------------------------------------------
    // PUT
    // -------------------------------------------------------------------------

    /// <summary>Returns the raw response for a PUT request with a JSON body.</summary>
    public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T payload) =>
        await _http.PutAsJsonAsync(endpoint, payload, JsonOptions);

    // -------------------------------------------------------------------------
    // PATCH
    // -------------------------------------------------------------------------

    /// <summary>Returns the raw response for a PATCH request with a JSON body.</summary>
    public async Task<HttpResponseMessage> PatchAsync<T>(string endpoint, T payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
        {
            Content = JsonContent.Create(payload, options: JsonOptions)
        };
        return await _http.SendAsync(request);
    }

    // -------------------------------------------------------------------------
    // DELETE
    // -------------------------------------------------------------------------

    /// <summary>Returns the raw response for a DELETE request.</summary>
    public async Task<HttpResponseMessage> DeleteAsync(string endpoint) =>
        await _http.DeleteAsync(endpoint);

    // -------------------------------------------------------------------------
    // Deserialisation helper
    // -------------------------------------------------------------------------

    /// <summary>
    /// Deserialises the JSON body of <paramref name="response"/> to
    /// <typeparamref name="T"/>.
    /// </summary>
    public static async Task<T?> DeserialiseAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }
}
