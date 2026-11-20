using ErrorOr;
using MediatR;
using SportWearShop.Application.Utilities;

namespace SportWearShop.Application.UseCases.Brands.ViewBrands;

public sealed record Request(
    int? PageNumber = null,
    int? PageSize = null,
    string? SearchTerm = null,
    bool? IsActive = null,
    string? SortBy = null,
    bool? SortDesc = null) : IRequest<ErrorOr<PagedList<Response>>>;
