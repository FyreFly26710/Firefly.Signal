using Firefly.Signal.Identity.Application.Queries;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.FunctionalTests.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.Identity.FunctionalTests.Application;

[TestClass]
public sealed class UserDocumentQueriesTests
{
    [TestMethod]
    public async Task ListAsync_OrdersDefaultDocumentsBeforeMoreRecentNonDefaults()
    {
        await using var database = new IdentitySqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var user = IdentityTestData.CreateUser();
        var defaultCv = IdentityTestData.CreateDocument(user.Id, UserDocumentType.Cv, "Default CV", isDefault: true);
        var laterSupportingDoc = IdentityTestData.CreateDocument(user.Id, UserDocumentType.ProfileSupporting, "Supporting Evidence", isDefault: false);

        dbContext.Users.Add(user);
        dbContext.UserDocuments.Add(defaultCv);
        await dbContext.SaveChangesAsync();

        await Task.Delay(20);

        dbContext.UserDocuments.Add(laterSupportingDoc);
        await dbContext.SaveChangesAsync();

        var queries = new UserDocumentQueries(dbContext);

        var documents = await queries.ListAsync(user.Id, CancellationToken.None);

        Assert.HasCount(2, documents);
        Assert.AreEqual(defaultCv.Id, documents[0].Id);
        Assert.IsTrue(documents[0].IsDefault);
        Assert.AreEqual(laterSupportingDoc.Id, documents[1].Id);
    }
}
