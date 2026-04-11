using Firefly.Signal.Identity.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContextSeed(IPasswordHasher<UserAccount> passwordHasher) : IDbSeeder<IdentityDbContext>
{
    public async Task SeedAsync(IdentityDbContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            var admin = UserAccount.Create("admin", string.Empty, "admin@firefly.local", "Firefly Admin", Roles.Admin);
            admin.ChangePassword(passwordHasher.HashPassword(admin, "Admin123!"));

            var testAdmin = UserAccount.Create("testadmin", string.Empty, "testadmin@firefly.local", "Firefly Test Admin", Roles.TestAdmin);
            testAdmin.ChangePassword(passwordHasher.HashPassword(testAdmin, "TestAdmin123!"));

            context.Users.AddRange(admin, testAdmin);
            await context.SaveChangesAsync();
        }
    }
}
