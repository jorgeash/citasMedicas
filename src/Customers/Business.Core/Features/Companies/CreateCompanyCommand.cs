using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Exceptions;
using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using MediatR;

namespace Business.Core.Features.Companies;

public class CreateCompanyCommand : IRequest<HttpResponse<CompanyDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public string MainEmail { get; set; } = string.Empty;
}

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, HttpResponse<CompanyDto>>
{
    private readonly ICompanyRepository _companyRepository;

    public CreateCompanyCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<HttpResponse<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new DomainException(Errores.CLIENTE_INVALIDO, "El nombre de la empresa es obligatorio.");

        var company = await _companyRepository.AddAsync(new Company
        {
            Name = request.Name.Trim(),
            Sigla = request.Sigla?.Trim() ?? string.Empty,
            MainEmail = request.MainEmail?.Trim() ?? string.Empty
        });

        return new HttpResponse<CompanyDto>(new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Sigla = company.Sigla,
            MainEmail = company.MainEmail,
            IsActive = company.IsActive
        });
    }
}