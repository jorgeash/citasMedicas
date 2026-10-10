using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Extensions;
using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;
using System.Linq.Expressions;

namespace Business.Core.Features.Customers;

public class GetCustomersQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<CustomerDto>>>>
{
}

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, HttpResponse<PagedDto<List<CustomerDto>>>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<HttpResponse<PagedDto<List<CustomerDto>>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Customer, object>>[] includes = [c => c.CustomerType!, c => c.Company!];

        var result = await _customerRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            !string.IsNullOrEmpty(request.Filter) ? Filter.FromStringExpression<Customer>(request.Filter) : null,
            orderByProperty: request.OrderBy,
            orderByDescending: request.OrderByDescending,
            cancellationToken: cancellationToken,
            includes: includes);

        return new HttpResponse<PagedDto<List<CustomerDto>>>(result.ToPagedDto(c => new CustomerDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            Email = c.Email,
            CustomerTypeId = c.CustomerTypeId,
            CustomerTypeName = c.CustomerType?.Name ?? string.Empty,
            CompanyId = c.CompanyId,
            CompanyName = c.Company?.Name ?? string.Empty,
            LocationId = c.LocationId,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        }));
    }
}