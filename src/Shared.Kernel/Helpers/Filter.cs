using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared.Kernel.Helpers;

//}
public static class Filter
{
    /// <summary>
    /// Búsqueda genérica de texto libre: OR de <c>Contains</c> sobre los campos indicados.
    /// A diferencia de <see cref="FromStringExpression{TModel}"/>, esto NO exige una
    /// expresión LINQ: el usuario escribe "juan" y se busca en todos los campos.
    /// Las rutas soportan navegación con punto (ej. "Paciente.Nombres") y cada nivel
    /// lleva guarda de null. EF Core traduce string.Contains a CHARINDEX (sin problemas
    /// de comodines; la insensibilidad a mayúsculas la da la colación de la BD).
    /// </summary>
    public static Expression<Func<T, bool>> FreeText<T>(string texto, params string[] rutas)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        if (string.IsNullOrWhiteSpace(texto))
            return Expression.Lambda<Func<T, bool>>(Expression.Constant(true), parameter);

        if (rutas is null || rutas.Length == 0)
            throw new ValidationException("search fields not configured");

        string termino = texto.Trim();
        Expression? body = null;
        MethodInfo contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        foreach (string ruta in rutas)
        {
            // x -> x.Paciente -> x.Paciente.Nombres
            List<Expression> accesos = new() { parameter };
            foreach (string parte in ruta.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                PropertyInfo? prop = accesos[^1].Type.GetProperty(
                    parte, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (prop is null)
                    throw new ValidationException($"search field invalid: {ruta}");

                accesos.Add(Expression.Property(accesos[^1], prop));
            }

            Expression destino = accesos[^1];
            if (destino.Type != typeof(string))
                throw new ValidationException($"search field must be a string: {ruta}");

            // Guardas de null en cada nivel (skipeando el parámetro, nunca null):
            // x.Paciente != null && x.Paciente.Nombres != null && ... && destino != null
            Expression? guardas = null;
            for (int i = 1; i < accesos.Count; i++)
            {
                if (accesos[i].Type.IsValueType)
                    continue;

                Expression chk = Expression.NotEqual(accesos[i], Expression.Constant(null, accesos[i].Type));
                guardas = guardas is null ? chk : Expression.AndAlso(guardas, chk);
            }

            Expression contiene = Expression.Call(destino, contains, Expression.Constant(termino));
            Expression terminoCampo = guardas is null ? contiene : Expression.AndAlso(guardas, contiene);

            body = body is null ? terminoCampo : Expression.OrElse(body, terminoCampo);
        }

        return Expression.Lambda<Func<T, bool>>(body!, parameter);
    }

    /// <summary>
    /// Combina dos predicados con AND (útil para sumar el parámetro `buscar` a los
    /// filtros explícitos ya existentes). Reescribe los parámetros para no usar
    /// Invoke, que EF Core no puede traducir a SQL.
    /// </summary>
    public static Expression<Func<T, bool>> AndAlso<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        ParameterExpression parameter = left.Parameters[0];
        Expression rightBody = new ParameterReplacer(right.Parameters[0], parameter).Visit(right.Body)!;
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, rightBody), parameter);
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

    public static Expression<Func<TModel, bool>> FromStringExpression<TModel>(string query, string parameter = "x")
    {
        try
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TModel), parameter);
            return (Expression<Func<TModel, bool>>)DynamicExpressionParser.ParseLambda(new ParameterExpression[1] { parameterExpression }, null, query);
        }
        catch
        {
            throw new ValidationException("filter expression invalid");
        }
    }

    public static Func<IQueryable<T>, IOrderedQueryable<T>>? ParseOrderBy<T>(string orderBy) where T : class
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return null;

        try
        {
            return query => query.OrderBy(orderBy);
        }
        catch
        {
            throw new ValidationException("orderBy expression invalid");
        }
    }
}
