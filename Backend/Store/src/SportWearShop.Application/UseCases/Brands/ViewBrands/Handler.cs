using ErrorOr;
using FluentValidation;
using MediatR;
using SportWearShop.Application.Abstractions.DataAccess;
using SportWearShop.Application.Utilities;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Application.UseCases.Brands.ViewBrands;

internal sealed class Handler(IRepository<Brand, int> repository, IValidator<Request> validator)
    : IRequestHandler<Request, ErrorOr<PagedList<Response>>>
{
    public async Task<ErrorOr<PagedList<Response>>> Handle(
        Request request, CancellationToken cancellationToken)
    {
        var normalizedRequest = request with
        {
            SearchTerm = request.SearchTerm?.Trim(),
            SortBy = string.IsNullOrWhiteSpace(request.SortBy) ? null : request.SortBy.Trim()
        };

        var validation = await validator.ValidateAsync(normalizedRequest, cancellationToken);
        if (!validation.IsValid)
        {
            return Errors.Validation(validation.Errors);
        }

        var pageNumber = normalizedRequest.PageNumber.GetValueOrDefault(1);
        var pageSize = normalizedRequest.PageSize.GetValueOrDefault(10);
        var specification = new BrandsSpecification(normalizedRequest, pageNumber, pageSize);

        var totalCount = await repository.CountAsync(specification, cancellationToken);
        var items = await repository.ListAsync(specification, cancellationToken);

        return new PagedList<Response>(items, totalCount, pageNumber, pageSize);
    }
}
