using System.Reflection;
using Firefly.Signal.ServiceDefaults;
using Firefly.Signal.SharedKernel.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Firefly.Signal.Api.UnitTests.Startup;

[TestClass]
public sealed class ApiHostDependencyInjectionTests
{
    public static IEnumerable<object[]> ApiDescriptors()
    {
        yield return
        [
            "Firefly.Signal.Ai.Api",
            "Firefly.Signal.Ai.Api.Extensions.ApplicationServiceExtensions"
        ];

        yield return
        [
            "Firefly.Signal.Gateway.Api",
            "Firefly.Signal.Gateway.Api.Extensions.ApplicationServiceExtensions"
        ];

        yield return
        [
            "Firefly.Signal.Identity.Api",
            "Firefly.Signal.Identity.Api.Extensions.ApplicationServiceExtensions"
        ];

        yield return
        [
            "Firefly.Signal.JobSearch.Api",
            "Firefly.Signal.JobSearch.Api.Extensions.ApplicationServiceExtensions"
        ];
    }

    [TestMethod]
    [DynamicData(nameof(ApiDescriptors))]
    public void AddApplicationServices_RegistersServicesThatValidateOnBuild(
        string assemblyName,
        string extensionTypeName)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = assemblyName,
            EnvironmentName = "Testing"
        });

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Testing:DatabaseName"] = $"{assemblyName}-tests",
            ["Jwt:SigningKey"] = "firefly-signal-dev-signing-key-please-change",
            ["UserDocumentStorage:AllowedContentTypes:0"] = "application/pdf",
            ["UserDocumentStorage:AllowedFileExtensions:0"] = ".pdf"
        });

        builder.AddFireflyServiceDefaults();
        InvokeAddApplicationServices(builder, assemblyName, extensionTypeName);
        builder.Services.AddProblemDetails();
        builder.Services.AddFireflyExceptionHandling();
        builder.AddDefaultOpenApi();

        using var serviceProvider = builder.Services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });

        Assert.IsNotNull(serviceProvider);
    }

    private static void InvokeAddApplicationServices(
        WebApplicationBuilder builder,
        string assemblyName,
        string extensionTypeName)
    {
        var assembly = Assembly.Load(assemblyName);
        var extensionType = assembly.GetType(extensionTypeName, throwOnError: true)!;
        var method = extensionType.GetMethod(
            "AddApplicationServices",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.IsNotNull(method, $"Could not find AddApplicationServices on {extensionTypeName}.");

        method.Invoke(null, [builder]);
    }
}
