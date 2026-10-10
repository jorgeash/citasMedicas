using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Exceptions;
using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using MediatR;

namespace Business.Core.Features.CustomerContacts;

public class CreateCustomerContactCommand : IRequest<HttpResponse<CustomerContactDto>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone1 { get; set; } = string.Empty;
    public string? Phone2 { get; set; }
    public int CustomerId { get; set; }
}

public class CreateCustomerContactCommandHandler : IRequestHandler<CreateCustomerContactCommand, HttpResponse<CustomerContactDto>>
{
    private readonly ICustomerContactRepository _customerContactRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerContactCommandHandler(
        ICustomerContactRepository customerContactRepository,
        ICustomerRepository customerRepository)
    {
        _customerContactRepository = customerContactRepository;
        _customerRepository = customerRepository;
    }

    public async Task<HttpResponse<CustomerContactDto>> Handle(CreateCustomerContactCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.Email))
            throw new DomainException(Errores.CONTACTO_INVALIDO, "El nombre y el correo del contacto son obligatorios.");

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer is null)
            throw new DomainException(Errores.CLIENTE_NO_ENCONTRADO,
                $"El cliente con Id {request.CustomerId} no existe.");

        var contact = await _customerContactRepository.AddAsync(new CustomerContact
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName?.Trim() ?? string.Empty,
            Email = request.Email.Trim(),
            Phone1 = request.Phone1?.Trim() ?? string.Empty,
            Phone2 = request.Phone2?.Trim(),
            CustomerId = request.CustomerId
        });

        return new HttpResponse<CustomerContactDto>(new CustomerContactDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email,
            Phone1 = contact.Phone1,
            Phone2 = contact.Phone2,
            CustomerId = contact.CustomerId,
            IsActive = contact.IsActive
        });
    }
}