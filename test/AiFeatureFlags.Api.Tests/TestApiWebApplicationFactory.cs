using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AiFeatureFlags.Api.Tests;

public sealed class TestApiWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly string _databasePath;

    public TestApiWebApplicationFactory()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"aff-api-tests-{Guid.NewGuid():n}.db");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Sqlite", $"Data Source={_databasePath};Foreign Keys=True");
        builder.UseSetting("SeedDemoData", "false");
        builder.UseSetting("Jwt:Issuer", "AiFeatureFlagsTests");
        builder.UseSetting("Jwt:Audience", "AiFeatureFlagsTestsClients");
        builder.UseSetting("Jwt:SigningKey", "unit-test-signing-key-must-be-32-bytes-min!");
        builder.UseSetting("Jwt:AccessTokenMinutes", "60");
        builder.UseSetting("Cors:FrontendOrigin", "http://localhost");
    }

    public new void Dispose()
    {
        base.Dispose();
        TryDelete(_databasePath);
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Best-effort cleanup for temp integration DB files.
        }
    }
}
