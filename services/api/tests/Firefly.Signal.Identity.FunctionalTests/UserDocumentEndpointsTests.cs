using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Firefly.Signal.Identity.Application;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Signal.Identity.FunctionalTests;

[TestClass]
public class UserDocumentEndpointsTests
{
    [TestMethod]
    public async Task Upload_creates_document_metadata_and_lists_it_for_current_user()
    {
        await using var factory = CreateFactory();
        SeedIdentityData(factory.Services);
        using var client = factory.CreateClient();
        var login = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        using var uploadContent = CreateUploadContent(
            documentType: "cv",
            fileName: "firefly-cv.pdf",
            contentType: "application/pdf",
            fileBytes: "sample-cv-pdf-content"u8.ToArray(),
            displayName: "Primary CV");

        var uploadResponse = await client.PostAsync("/api/users/documents", uploadContent);
        uploadResponse.EnsureSuccessStatusCode();

        var createdDocument = await uploadResponse.Content.ReadFromJsonAsync<UserDocumentResponse>();
        Assert.IsNotNull(createdDocument);
        Assert.AreEqual("cv", createdDocument.DocumentType);
        Assert.AreEqual("Primary CV", createdDocument.DisplayName);
        Assert.IsTrue(createdDocument.IsDefault);
        Assert.IsFalse(string.IsNullOrWhiteSpace(createdDocument.StorageKey));
        Assert.AreEqual("application/pdf", createdDocument.ContentType);

        var listResponse = await client.GetAsync("/api/users/documents");
        listResponse.EnsureSuccessStatusCode();

        var documents = await listResponse.Content.ReadFromJsonAsync<List<UserDocumentResponse>>();
        Assert.IsNotNull(documents);
        Assert.HasCount(1, documents);
        Assert.AreEqual(createdDocument.Id, documents[0].Id);
    }

    [TestMethod]
    public async Task Set_default_switches_default_cv_document()
    {
        await using var factory = CreateFactory();
        SeedIdentityData(factory.Services);
        using var client = factory.CreateClient();
        var login = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        var firstDocument = await UploadDocumentAsync(client, "cv", "first-cv.pdf", "application/pdf", "first"u8.ToArray(), "First CV");
        var secondDocument = await UploadDocumentAsync(client, "cv", "second-cv.pdf", "application/pdf", "second"u8.ToArray(), "Second CV");

        var setDefaultResponse = await client.PostAsync($"/api/users/documents/{secondDocument.Id}/default", content: null);
        setDefaultResponse.EnsureSuccessStatusCode();

        var updatedDocument = await setDefaultResponse.Content.ReadFromJsonAsync<UserDocumentResponse>();
        Assert.IsNotNull(updatedDocument);
        Assert.AreEqual(secondDocument.Id, updatedDocument.Id);
        Assert.IsTrue(updatedDocument.IsDefault);

        var documents = await client.GetFromJsonAsync<List<UserDocumentResponse>>("/api/users/documents");
        Assert.IsNotNull(documents);
        Assert.HasCount(2, documents);
        Assert.AreEqual(secondDocument.Id, documents.Single(x => x.IsDefault).Id);
        Assert.IsFalse(documents.Single(x => x.Id == firstDocument.Id).IsDefault);
    }

    [TestMethod]
    public async Task Delete_removes_document_from_active_list_and_promotes_replacement_default()
    {
        await using var factory = CreateFactory();
        SeedIdentityData(factory.Services);
        using var client = factory.CreateClient();
        var login = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        var firstDocument = await UploadDocumentAsync(client, "cv", "first-cv.pdf", "application/pdf", "first"u8.ToArray(), "First CV");
        var secondDocument = await UploadDocumentAsync(client, "cv", "second-cv.pdf", "application/pdf", "second"u8.ToArray(), "Second CV");

        var deleteResponse = await client.DeleteAsync($"/api/users/documents/{firstDocument.Id}");
        Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var documents = await client.GetFromJsonAsync<List<UserDocumentResponse>>("/api/users/documents");
        Assert.IsNotNull(documents);
        Assert.HasCount(1, documents);
        Assert.AreEqual(secondDocument.Id, documents[0].Id);
        Assert.IsTrue(documents[0].IsDefault);
    }

    [TestMethod]
    public async Task Upload_rejects_unsupported_content_type()
    {
        await using var factory = CreateFactory();
        SeedIdentityData(factory.Services);
        using var client = factory.CreateClient();
        var login = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        using var uploadContent = CreateUploadContent(
            documentType: "cv",
            fileName: "firefly-cv.exe",
            contentType: "application/octet-stream",
            fileBytes: [1, 2, 3, 4],
            displayName: "Unsafe CV");

        var uploadResponse = await client.PostAsync("/api/users/documents", uploadContent);

        Assert.AreEqual(HttpStatusCode.BadRequest, uploadResponse.StatusCode);
    }

    private static WebApplicationFactory<Firefly.Signal.Identity.Api.Program> CreateFactory()
    {
        var databaseName = $"identity-documents-tests-{Guid.NewGuid():N}";

        return new WebApplicationFactory<Firefly.Signal.Identity.Api.Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.UseSetting("Testing:DatabaseName", databaseName);
            });
    }

    private static async Task<LoginResponse> LoginAsAdminAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginUserRequest("admin", "Admin123!"));
        response.EnsureSuccessStatusCode();

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.IsNotNull(login);
        return login;
    }

    private static async Task<UserDocumentResponse> UploadDocumentAsync(
        HttpClient client,
        string documentType,
        string fileName,
        string contentType,
        byte[] fileBytes,
        string displayName)
    {
        using var uploadContent = CreateUploadContent(documentType, fileName, contentType, fileBytes, displayName);
        var response = await client.PostAsync("/api/users/documents", uploadContent);
        response.EnsureSuccessStatusCode();

        var document = await response.Content.ReadFromJsonAsync<UserDocumentResponse>();
        Assert.IsNotNull(document);
        return document;
    }

    private static MultipartFormDataContent CreateUploadContent(
        string documentType,
        string fileName,
        string contentType,
        byte[] fileBytes,
        string displayName)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(documentType), "DocumentType");
        content.Add(new StringContent(displayName), "DisplayName");

        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "File", fileName);
        return content;
    }

    private static void SeedIdentityData(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserAccount>>();

        if (dbContext.Users.Any())
        {
            return;
        }

        var admin = UserAccount.Create("admin", string.Empty, "admin@firefly.local", "Firefly Admin", Roles.Admin);
        admin.ChangePassword(passwordHasher.HashPassword(admin, "Admin123!"));

        dbContext.Users.Add(admin);
        dbContext.SaveChanges();
    }
}
