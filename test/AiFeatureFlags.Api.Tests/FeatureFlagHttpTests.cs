using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AiFeatureFlags.Application.Auth;
using AiFeatureFlags.Application.FeatureFlags;

namespace AiFeatureFlags.Api.Tests;

public sealed class FeatureFlagHttpTests : IClassFixture<TestApiWebApplicationFactory>
{
    private readonly TestApiWebApplicationFactory _factory;

    public FeatureFlagHttpTests(TestApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Public_summaries_are_available_without_authentication()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/public/feature-flags/summaries");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Authorized_crud_flow_works_end_to_end()
    {
        var client = _factory.CreateClient();
        var email = $"http-{Guid.NewGuid():n}@example.com";

        var register = await client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest(email, "password123", "HTTP User"));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);

        var auth = await register.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth!.AccessToken));

        var userClient = _factory.CreateClient();
        userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var emptyList = await userClient.GetFromJsonAsync<List<FeatureFlagResponse>>("/api/feature-flags");
        Assert.NotNull(emptyList);
        Assert.Empty(emptyList!);

        var createResponse = await userClient.PostAsJsonAsync("/api/feature-flags",
            new CreateFeatureFlagRequest(
                $"flag.{Guid.NewGuid():n}",
                "created via integration test",
                true,
                "Development",
                "{}"));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<FeatureFlagResponse>();
        Assert.NotNull(created);

        var updateResponse = await userClient.PutAsJsonAsync($"/api/feature-flags/{created!.Id}",
            new UpdateFeatureFlagRequest(
                "updated description",
                false,
                "Staging",
                """{"notes":"adjusted rollout"}"""));

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var deleteResponse = await userClient.DeleteAsync($"/api/feature-flags/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Protected_endpoint_requires_authentication()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/feature-flags");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Validation_errors_return_problem_json()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterUserRequest("not-an-email", "short", ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("title", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"errorCode\":\"VALIDATION_FAILED\"", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Duplicate_register_returns_conflict_problem_json()
    {
        var client = _factory.CreateClient();
        var email = $"dup-{Guid.NewGuid():n}@example.com";
        var req = new RegisterUserRequest(email, "password123", "Dup User");

        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/auth/register", req)).StatusCode);

        var conflict = await client.PostAsJsonAsync("/api/auth/register", req);
        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);

        var body = await conflict.Content.ReadAsStringAsync();
        Assert.Contains("\"errorCode\":\"CONFLICT\"", body, StringComparison.Ordinal);
    }
}
