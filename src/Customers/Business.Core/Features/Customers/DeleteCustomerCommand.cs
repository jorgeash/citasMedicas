using Business.Core.DTOs;
using Business.Core.Exceptions;
using Business.Core.Interfaces.Repositories;
using MediatR;

namespace Business.Core.Features.Customers;

public class DeleteCustomerCommand : IRequest<HttpResponse<string>>
{
    public int Id { get; set; }
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, HttpResponse<string>>
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<HttpResponse<string>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);

        if (customer is null)
            throw new DomainException(Errores.CLIENTE_NO_ENCONTRADO, $"El cliente con Id {request.Id} no existe.");

        // Baja logica: el historico de citas del cliente se conserva.
        customer.IsActive = false;
        await _customerRepository.UpdateAsync(customer);

        return new HttpResponse<string>($"Cliente {request.Id} dado de baja correctamente.");
    }
}