using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Firefly.Signal.JobSearch.FunctionalTests;

[TestClass]
public class JobSearchApiTests
{
    [TestMethod]
    public async Task Demo_endpoint_returns_success()
    {
        await using var factory = new WebApplicationFactory<Firefly.Signal.JobSearch.Api.Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Testing"));
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
    }
}
