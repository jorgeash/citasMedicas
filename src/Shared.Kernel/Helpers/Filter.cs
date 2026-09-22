using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared.Kernel.Helpers;

//}
public static class Filter
{
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
