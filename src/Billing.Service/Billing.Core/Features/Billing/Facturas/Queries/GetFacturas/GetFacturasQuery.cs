using System.Linq.Expressions;
using Billing.Core.DTOs;
using Billing.Core.DTOs.Billing;
using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using Billing.Domain.Enums;
using MediatR;
using Shared.Kernel.Helpers;

namespace Billing.Core.Features.Billing.Facturas.Queries.GetFacturas;

public class GetFacturasQuery : RequestParameters, IRequest<PagedDto<List<FacturaResumenDto>>>
{
    public EstadoFactura? Estado { get; set; }
    public int? PacienteID { get; set; }
    public int? AseguradoraID { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
}

public class GetFacturasQueryHandler : IRequestHandler<GetFacturasQuery, PagedDto<List<FacturaResumenDto>>>
{
    private readonly IFacturaRepository _repository;

    public GetFacturasQueryHandler(IFacturaRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<FacturaResumenDto>>> Handle(GetFacturasQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Factura, bool>>? filter = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Factura>(request.Filter);

        if (request.Estado.HasValue)
            filter = And(filter, f => f.Estado == request.Estado.Value);

        if (request.PacienteID.HasValue)
            filter = And(filter, f => f.PacienteID == request.PacienteID.Value);

        if (request.AseguradoraID.HasValue)
            filter = And(filter, f => f.AseguradoraID == request.AseguradoraID.Value);

        if (request.Desde.HasValue)
        {
            var desde = request.Desde.Value.Date;
            filter = And(filter, f => f.FechaEmision >= desde);
        }

        if (request.Hasta.HasValue)
        {
            // Incluye todo el día indicado.
            var hasta = request.Hasta.Value.Date.AddDays(1);
            filter = And(filter, f => f.FechaEmision < hasta);
        }

        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? Filter.ParseOrderBy<Factura>(request.OrderBy)
            : (Func<IQueryable<Factura>, IOrderedQueryable<Factura>>)(q => q.OrderByDescending(f => f.FechaEmision));

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            orderBy,
            cancellationToken: cancellationToken);

        return new PagedDto<List<FacturaResumenDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(FacturaMapper.ToResumenDto).ToList());
    }

    private static Expression<Func<T, bool>> And<T>(
        Expression<Func<T, bool>>? left,
        Expression<Func<T, bool>> right)
    {
        if (left is null)
            return right;

        var rightBody = new ParameterReplacer(right.Parameters[0], left.Parameters[0]).Visit(right.Body)!;
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left.Body, rightBody),
            left.Parameters[0]);
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ParameterReplacer(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _from ? _to : base.VisitParameter(node);
    }
}
