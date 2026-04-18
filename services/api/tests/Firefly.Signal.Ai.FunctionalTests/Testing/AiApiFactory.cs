using Firefly.Signal.Ai.Domain;
using Firefly.Signal.Ai.Infrastructure.AiProviders;
using Firefly.Signal.Ai.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Firefly.Signal.Ai.FunctionalTests.Testing;

internal sealed class AiApiFactory : WebApplicationFactory<Firefly.Signal.Ai.Api.Program>
{
    private readonly string databaseName = $"ai-api-tests-{Guid.NewGuid():N}";
    private readonly IAiChatProvider _chatGptProvider;
    private readonly IAiChatProvider _deepSeekProvider;

    public AiApiFactory(
        IAiChatProvider? chatGptProvider = null,
        IAiChatProvider? deepSeekProvider = null)
    {
        _chatGptProvider = chatGptProvider ?? Substitute.For<IAiChatProvider>();
        _deepSeekProvider = deepSeekProvider ?? Substitute.For<IAiChatProvider>();
    }

    public async Task SeedAsync(params object[] entities)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AiDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
        dbContext.AddRange(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task<TResult> ExecuteDbContextAsync<TResult>(Func<AiDbContext, Task<TResult>> action)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AiDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
        return await action(dbContext);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Testing:DatabaseName"] = databaseName
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(new AiProviderResolver(
                new ServiceCollection()
                    .AddKeyedSingleton(AiProvider.ChatGpt, _chatGptProvider)
                    .AddKeyedSingleton(AiProvider.DeepSeek, _deepSeekProvider)
                    .BuildServiceProvider()));
        });
    }
}
