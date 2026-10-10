using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Extensions;
using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Business.Core.Features.CustomerTypes;

public class GetCustomerTypesQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<CustomerTypeDto>>>>
{
}

public class GetCustomerTypesQueryHandler : IRequestHandler<GetCustomerTypesQuery, HttpResponse<PagedDto<List<CustomerTypeDto>>>>
{
    private readonly ICustomerTypeRepository _customerTypeRepository;

    public GetCustomerTypesQueryHandler(ICustomerTypeRepository customerTypeRepository)
    {
        _customerTypeRepository = customerTypeRepository;
    }

    public async Task<HttpResponse<PagedDto<List<CustomerTypeDto>>>> Handle(GetCustomerTypesQuery request, CancellationToken cancellationToken)
    {
        var result = await _customerTypeRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            !string.IsNullOrEmpty(request.Filter) ? Filter.FromStringExpression<CustomerType>(request.Filter) : null,
            orderByProperty: request.OrderBy,
            orderByDescending: request.OrderByDescending,
            cancellationToken: cancellationToken);

        return new HttpResponse<PagedDto<List<CustomerTypeDto>>>(result.ToPagedDto(t => new CustomerTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            IsActive = t.IsActive
        }));
    }
}