using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Extensions;
using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;
using System.Linq.Expressions;

namespace Business.Core.Features.CustomerContacts;

public class GetCustomerContactsQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<CustomerContactDto>>>>
{
}

public class GetCustomerContactsQueryHandler : IRequestHandler<GetCustomerContactsQuery, HttpResponse<PagedDto<List<CustomerContactDto>>>>
{
    private readonly ICustomerContactRepository _customerContactRepository;

    public GetCustomerContactsQueryHandler(ICustomerContactRepository customerContactRepository)
    {
        _customerContactRepository = customerContactRepository;
    }

    public async Task<HttpResponse<PagedDto<List<CustomerContactDto>>>> Handle(GetCustomerContactsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<CustomerContact, object>>[] includes = [c => c.Customer!];

        var result = await _customerContactRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            !string.IsNullOrEmpty(request.Filter) ? Filter.FromStringExpression<CustomerContact>(request.Filter) : null,
            orderByProperty: request.OrderBy,
            orderByDescending: request.OrderByDescending,
            cancellationToken: cancellationToken,
            includes: includes);

        return new HttpResponse<PagedDto<List<CustomerContactDto>>>(result.ToPagedDto(c => new CustomerContactDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Phone1 = c.Phone1,
            Phone2 = c.Phone2,
            CustomerId = c.CustomerId,
            IsActive = c.IsActive
        }));
    }
}