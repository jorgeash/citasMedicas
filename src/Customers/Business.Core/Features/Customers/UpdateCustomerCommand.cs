using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Exceptions;
using Business.Core.Interfaces.Repositories;
using Business.Core.Interfaces.Services;
using Business.Domain.Entities;
using MediatR;

namespace Business.Core.Features.Customers;

public class UpdateCustomerCommand : IRequest<HttpResponse<string>>
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CustomerTypeId { get; set; }
    public int CompanyId { get; set; }
    public int? LocationId { get; set; }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, HttpResponse<string>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILocationService _locationService;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, ILocationService locationService)
    {
        _customerRepository = customerRepository;
        _locationService = locationService;
    }

    public async Task<HttpResponse<string>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);

        if (customer is null)
            throw new DomainException(Errores.CLIENTE_NO_ENCONTRADO, $"El cliente con Id {request.Id} no existe.");

        if (!string.IsNullOrWhiteSpace(request.Code))
            customer.Code = request.Code.Trim();

        if (!string.IsNullOrWhiteSpace(request.Name))
            customer.Name = request.Name.Trim();

        if (!string.IsNullOrWhiteSpace(request.Email))
            customer.Email = request.Email.Trim();

        if (request.CustomerTypeId > 0)
            customer.CustomerTypeId = request.CustomerTypeId;

        if (request.CompanyId > 0)
            customer.CompanyId = request.CompanyId;

        // Si viene LocationId se vuelve a validar contra el microservicio de Addresses.
        if (request.LocationId is > 0)
        {
            var location = await _locationService.ObtenerLocationAsync(request.LocationId.Value, cancellationToken);

            if (location is null)
                throw new DomainException(Errores.LOCATION_NO_ENCONTRADA,
                    $"No existe una direccion con LocationId {request.LocationId} en el microservicio de Addresses.");

            customer.LocationId = location.Id;
        }

        await _customerRepository.UpdateAsync(customer);

        return new HttpResponse<string>($"Cliente {customer.Id} actualizado correctamente.");
    }
}