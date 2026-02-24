using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using GUIApps.Shared.Models;
using Microsoft.Extensions.Configuration;

namespace AdminService.Services;

public class KeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public KeycloakService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var baseUrl = _configuration["Keycloak:BaseUrl"];
        var adminUser = _configuration["Keycloak:AdminUser"];
        var adminPassword = _configuration["Keycloak:AdminPassword"];

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", "admin-cli"),
            new KeyValuePair<string, string>("username", adminUser),
            new KeyValuePair<string, string>("password", adminPassword),
            new KeyValuePair<string, string>("grant_type", "password")
        });

        var response = await _httpClient.PostAsync($"{baseUrl}/realms/master/protocol/openid-connect/token", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonNode>();
        return json?["access_token"]?.ToString() ?? string.Empty;
    }

    public async Task<List<UserInfo>> GetUsersAsync()
    {
        var token = await GetAdminTokenAsync();
        var baseUrl = _configuration["Keycloak:BaseUrl"];
        var realm = _configuration["Keycloak:Realm"];

        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/admin/realms/{realm}/users");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var usersJson = await response.Content.ReadFromJsonAsync<JsonNode>();
        var result = new List<UserInfo>();

        if (usersJson is JsonArray userArray)
        {
            foreach (var u in userArray)
            {
                result.Add(new UserInfo
                {
                    UserId = u["id"]?.ToString() ?? "",
                    Name = $"{u["firstName"]} {u["lastName"]}".Trim(),
                    Email = u["email"]?.ToString() ?? "",
                    // Default empty lists
                    Roles = new(),
                    Reports = new()
                });
            }
        }
        return result;
    }
}
