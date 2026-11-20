using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.SignosVitales.Commands.RegistrarSignosVitales;

public class RegistrarSignosVitalesCommand : IRequest<SignosVitalesDto>
{
    public long AtencionID { get; set; }
    public DateTime FechaMedicion { get; set; }
    public decimal? Temperatura { get; set; }
    public int? FrecuenciaCardiaca { get; set; }
    public int? FrecuenciaRespiratoria { get; set; }
    public int? PresionSistolica { get; set; }
    public int? PresionDiastolica { get; set; }
    public decimal? SaturacionOxigeno { get; set; }
    public decimal? PesoKg { get; set; }
    public decimal? TallaCm { get; set; }
    public string? Observaciones { get; set; }
}

public class RegistrarSignosVitalesCommandHandler : IRequestHandler<RegistrarSignosVitalesCommand, SignosVitalesDto>
{
    private readonly ISignosVitalesRepository _repository;
    private readonly IAtencionRepository _atenciones;

    public RegistrarSignosVitalesCommandHandler(
        ISignosVitalesRepository repository,
        IAtencionRepository atenciones)
    {
        _repository = repository;
        _atenciones = atenciones;
    }

    public async Task<SignosVitalesDto> Handle(RegistrarSignosVitalesCommand request, CancellationToken cancellationToken)
    {
        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            cancellationToken: cancellationToken);

        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        decimal? imc = null;
        if (request.PesoKg.HasValue && request.TallaCm.HasValue && request.PesoKg.Value > 0 && request.TallaCm.Value > 0)
        {
            var tallaMetros = request.TallaCm.Value / 100m;
            imc = Math.Round(request.PesoKg.Value / (tallaMetros * tallaMetros), 2);
        }

        var signos = new global::Clinical.Domain.Entities.SignosVitales
        {
            AtencionID = request.AtencionID,
            FechaMedicion = request.FechaMedicion == default ? DateTime.UtcNow : request.FechaMedicion,
            Temperatura = request.Temperatura,
            FrecuenciaCardiaca = request.FrecuenciaCardiaca,
            FrecuenciaRespiratoria = request.FrecuenciaRespiratoria,
            PresionSistolica = request.PresionSistolica,
            PresionDiastolica = request.PresionDiastolica,
            SaturacionOxigeno = request.SaturacionOxigeno,
            PesoKg = request.PesoKg,
            TallaCm = request.TallaCm,
            IMC = imc,
            Observaciones = request.Observaciones,
            CreatedAt = DateTime.UtcNow
        };

        var creados = await _repository.AddAsync(signos);
        return ClinicalMapper.ToDto(creados);
    }
}
