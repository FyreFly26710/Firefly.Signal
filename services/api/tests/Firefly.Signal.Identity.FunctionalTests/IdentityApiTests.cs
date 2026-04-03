using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Firefly.Signal.Identity.FunctionalTests;

[TestClass]
public class IdentityApiTests
{
    [TestMethod]
    public async Task Root_endpoint_returns_success()
    {
        await using var factory = new WebApplicationFactory<Firefly.Signal.Identity.Api.Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Testing"));
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
    }
}
