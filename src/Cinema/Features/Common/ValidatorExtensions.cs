using Cinema.Persistance;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Common;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> IdExist<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IServiceProvider serviceProvider)
        where TEntity : class, IEntity
    {
        return ruleBuilder.MustAsync(async (id, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
            return await db.Set<TEntity>().AsNoTracking().AnyAsync(e => e.Id == id, cancellationToken);
        }).WithMessage($"{nameof(TEntity)} does not exits");
    }
}
