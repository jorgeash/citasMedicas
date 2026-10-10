using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Exceptions;
using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using MediatR;

namespace Business.Core.Features.CustomerTypes;

public class CreateCustomerTypeCommand : IRequest<HttpResponse<CustomerTypeDto>>
{
    public string Name { get; set; } = string.Empty;
}

public class CreateCustomerTypeCommandHandler : IRequestHandler<CreateCustomerTypeCommand, HttpResponse<CustomerTypeDto>>
{
    private readonly ICustomerTypeRepository _customerTypeRepository;

    public CreateCustomerTypeCommandHandler(ICustomerTypeRepository customerTypeRepository)
    {
        _customerTypeRepository = customerTypeRepository;
    }

    public async Task<HttpResponse<CustomerTypeDto>> Handle(CreateCustomerTypeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new DomainException(Errores.CLIENTE_INVALIDO, "El nombre del tipo de cliente es obligatorio.");

        var customerType = await _customerTypeRepository.AddAsync(new CustomerType { Name = request.Name.Trim() });

        return new HttpResponse<CustomerTypeDto>(new CustomerTypeDto
        {
            Id = customerType.Id,
            Name = customerType.Name,
            IsActive = customerType.IsActive
        });
    }
}