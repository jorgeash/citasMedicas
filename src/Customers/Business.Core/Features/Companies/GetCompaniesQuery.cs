using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Extensions;
using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Business.Core.Features.Companies;

public class GetCompaniesQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<CompanyDto>>>>
{
}

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, HttpResponse<PagedDto<List<CompanyDto>>>>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompaniesQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<HttpResponse<PagedDto<List<CompanyDto>>>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var result = await _companyRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            !string.IsNullOrEmpty(request.Filter) ? Filter.FromStringExpression<Company>(request.Filter) : null,
            orderByProperty: request.OrderBy,
            orderByDescending: request.OrderByDescending,
            cancellationToken: cancellationToken);

        return new HttpResponse<PagedDto<List<CompanyDto>>>(result.ToPagedDto(c => new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            Sigla = c.Sigla,
            MainEmail = c.MainEmail,
            IsActive = c.IsActive
        }));
    }
}