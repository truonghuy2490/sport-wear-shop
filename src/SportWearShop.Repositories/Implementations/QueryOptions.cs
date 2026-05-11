using System.Linq.Expressions;

namespace SportWearShop.Repositories.Implementations;

public class QueryOptions<T>
{
    public Expression<Func<T, bool>>? Filter { get; set; }

    public Expression<Func<T, object>>? SortBy { get; set; }

    public bool Ascending { get; set; } = true;

    public bool AsNoTracking { get; set; } = true;

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public List<Expression<Func<T, object>>> Includes { get; set; } = [];
}