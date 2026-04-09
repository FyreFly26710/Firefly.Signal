using System.Linq.Expressions;
using Firefly.Signal.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.SharedKernel.EntityFramework;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var isNotDeleted = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(isNotDeleted, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }

        return modelBuilder;
    }
}
