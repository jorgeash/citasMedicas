using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.DTOs.External;
using Business.Core.Exceptions;
using Business.Core.Interfaces.Repositories;
using Business.Core.Interfaces.Services;
using Business.Domain.Entities;
using MediatR;

namespace Business.Core.Features.Customers;

/// <summary>
/// Alta de clientes. LocationId es obligatorio: es el Id de la fila del catalogo de
/// direcciones (Addresses_Position) que mantiene el microservicio de Addresses.
/// </summary>
public class CreateCustomerCommand : IRequest<HttpResponse<CustomerDto>>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CustomerTypeId { get; set; }
    public int CompanyId { get; set; }
    public int LocationId { get; set; }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, HttpResponse<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerTypeRepository _customerTypeRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILocationService _locationService;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        ICustomerTypeRepository customerTypeRepository,
        ICompanyRepository companyRepository,
        ILocationService locationService)
    {
        _customerRepository = customerRepository;
        _customerTypeRepository = customerTypeRepository;
        _companyRepository = companyRepository;
        _locationService = locationService;
    }

    public async Task<HttpResponse<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
            throw new DomainException(Errores.CLIENTE_INVALIDO, "El codigo y el nombre del cliente son obligatorios.");

        var code = request.Code.Trim();
        var existente = await _customerRepository.GetOneByAsync(c => c.Code == code, cancellationToken: cancellationToken);
        if (existente is not null)
            throw new DomainException(Errores.CODIGO_CLIENTE_DUPLICADO, $"El codigo de cliente '{code}' ya esta registrado.");

        var customerType = await _customerTypeRepository.GetByIdAsync(request.CustomerTypeId);
        if (customerType is null)
            throw new DomainException(Errores.TIPO_CLIENTE_NO_ENCONTRADO,
                $"El tipo de cliente con Id {request.CustomerTypeId} no existe.");

        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company is null)
            throw new DomainException(Errores.EMPRESA_NO_ENCONTRADA,
                $"La empresa con Id {request.CompanyId} no existe.");

        if (request.LocationId <= 0)
            throw new DomainException(Errores.LOCATION_ID_INVALIDO,
                "El LocationId es obligatorio: debe apuntar a una direccion del microservicio de Addresses.");

        // Integracion entre microservicios: la direccion se valida contra el catalogo de
        // Addresses usando el cliente IRest de la libreria NugetPackage_Rest.
        var location = await _locationService.ObtenerLocationAsync(request.LocationId, cancellationToken);

        if (location is null)
            throw new DomainException(Errores.LOCATION_NO_ENCONTRADA,
                $"No existe una direccion con LocationId {request.LocationId} en el microservicio de Addresses.");

        var customer = await _customerRepository.AddAsync(new Customer
        {
            Code = code,
            Name = request.Name.Trim(),
            Email = request.Email?.Trim() ?? string.Empty,
            CustomerTypeId = request.CustomerTypeId,
            CompanyId = request.CompanyId,
            LocationId = location.Id
        });

        return new HttpResponse<CustomerDto>(new CustomerDto
        {
            Id = customer.Id,
            Code = customer.Code,
            Name = customer.Name,
            Email = customer.Email,
            CustomerTypeId = customer.CustomerTypeId,
            CustomerTypeName = customerType.Name,
            CompanyId = customer.CompanyId,
            CompanyName = company.Name,
            LocationId = customer.LocationId,
            Location = Resumir(location),
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt
        });
    }

    private static LocationSummaryDto Resumir(LocationDto location) => new()
    {
        Id = location.Id,
        Address = location.Address,
        CityName = location.CityName,
        CountryName = location.CountryName,
        ZipCode = location.ZipCode
    };
}