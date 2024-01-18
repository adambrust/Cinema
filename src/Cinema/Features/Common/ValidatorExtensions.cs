using Cinema.Persistance;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Common;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> IdExist<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        CinemaDbContext db)
        where TEntity : class, IEntity
    {
        return ruleBuilder.MustAsync((id, cancellationToken) =>
                db.Set<TEntity>().AsNoTracking().AnyAsync(e => e.Id == id, cancellationToken)
            ).WithMessage($"{nameof(TEntity)} does not exits");
    }
}
