using Lab.Core.Exceptions;
using Lab.Core.Features.Lab.Estudios.Commands.CreateEstudio;
using Lab.Domain.Entities;
using Lab.Tests.Fakes;

namespace Lab.Tests.Tests;

public class CreateEstudioCommandHandlerTests
{
    [Fact]
    public async Task Handle_CodigoDuplicado_LanzaDomainException()
    {
        var repository = new FakeEstudioRepository();
        repository.Seed(new Estudio
        {
            EstudioID = 1,
            Codigo = "HEMO",
            Nombre = "Hemograma completo",
            Tipo = "LABORATORIO",
            Activo = true
        });

        var handler = new CreateEstudioCommandHandler(repository);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CreateEstudioCommand
        {
            Codigo = "hemo",
            Nombre = "Otro hemograma",
            Tipo = "LABORATORIO"
        }, CancellationToken.None));

        Assert.Equal(Errores.ESTUDIO_CODIGO_DUPLICADO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_EstudioValido_GuardaConEstadoActivoYPrecio()
    {
        var repository = new FakeEstudioRepository();
        var handler = new CreateEstudioCommandHandler(repository);

        var result = await handler.Handle(new CreateEstudioCommand
        {
            Codigo = "GLU",
            Nombre = "Glucosa",
            Tipo = "laboratorio",
            RequiereAyuno = true,
            Precio = 12.50m
        }, CancellationToken.None);

        Assert.Equal("GLU", result.Codigo);
        Assert.Equal("LABORATORIO", result.Tipo);
        Assert.True(result.Activo);
        Assert.True(result.RequiereAyuno);
        Assert.Equal(12.50m, result.Precio);
        Assert.Single(await repository.GetAllAsync());
    }

    [Fact]
    public async Task Handle_TipoInvalido_LanzaDomainException()
    {
        var handler = new CreateEstudioCommandHandler(new FakeEstudioRepository());

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CreateEstudioCommand
        {
            Codigo = "XXX",
            Nombre = "Estudio raro",
            Tipo = "DESCONOCIDO"
        }, CancellationToken.None));

        Assert.Equal(Errores.DATOS_INVALIDOS, ex.Codigo);
    }
}
