using System.Linq.Expressions;
using Ardalis.Specification;

namespace SportWearShop.Application.Utilities;

internal static class SpecificationExtensions
{
    internal static ISpecificationBuilder<TEntity> ApplyPaging<TEntity>(
        this ISpecificationBuilder<TEntity> builder, int pageNumber, int pageSize)
        where TEntity : class
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        return builder.Skip(checked((pageNumber - 1) * pageSize)).Take(pageSize);
    }

    internal static IOrderedSpecificationBuilder<TEntity> ApplySorting<TEntity>(
        this ISpecificationBuilder<TEntity> builder,
        Expression<Func<TEntity, object?>> keySelector,
        bool descending)
        where TEntity : class
        => descending ? builder.OrderByDescending(keySelector) : builder.OrderBy(keySelector);
}
