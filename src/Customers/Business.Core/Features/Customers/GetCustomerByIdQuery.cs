using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Exceptions;
using Business.Core.Interfaces.Repositories;
using Business.Core.Interfaces.Services;
using Business.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Business.Core.Features.Customers;

public class GetCustomerByIdQuery : IRequest<HttpResponse<CustomerDto>?>
{
    public int Id { get; set; }

    public GetCustomerByIdQuery(int id)
    {
        Id = id;
    }
}

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, HttpResponse<CustomerDto>?>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILocationService _locationService;
    private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

    public GetCustomerByIdQueryHandler(
        ICustomerRepository customerRepository,
        ILocationService locationService,
        ILogger<GetCustomerByIdQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _locationService = locationService;
        _logger = logger;
    }

    public async Task<HttpResponse<CustomerDto>?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Customer, object>>[] includes = [c => c.CustomerType!, c => c.Company!];

        var customer = await _customerRepository.GetOneByAsync(
            c => c.Id == request.Id,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes: includes);

        if (customer is null)
            return null;

        // El detalle de la direccion no es critico para la lectura del cliente: si el
        // microservicio de Addresses no responde, se devuelve el cliente sin el resumen.
        LocationSummaryDto? location = null;
        try
        {
            var dto = await _locationService.ObtenerLocationAsync(customer.LocationId, cancellationToken);
            if (dto is not null)
            {
                location = new LocationSummaryDto
                {
                    Id = dto.Id,
                    Address = dto.Address,
                    CityName = dto.CityName,
                    CountryName = dto.CountryName,
                    ZipCode = dto.ZipCode
                };
            }
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex,
                "No se pudo consultar la direccion {LocationId} del cliente {CustomerId} en el microservicio de Addresses.",
                customer.LocationId, customer.Id);
        }

        return new HttpResponse<CustomerDto>(new CustomerDto
        {
            Id = customer.Id,
            Code = customer.Code,
            Name = customer.Name,
            Email = customer.Email,
            CustomerTypeId = customer.CustomerTypeId,
            CustomerTypeName = customer.CustomerType?.Name ?? string.Empty,
            CompanyId = customer.CompanyId,
            CompanyName = customer.Company?.Name ?? string.Empty,
            LocationId = customer.LocationId,
            Location = location,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt
        });
    }
}