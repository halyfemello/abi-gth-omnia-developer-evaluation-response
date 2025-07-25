using DeveloperEvaluation.Core.Domain.Entities;
using System.Linq.Expressions;

namespace DeveloperEvaluation.Core.Application.Services;

public class ProductFilterBuilder
{
    public Expression<Func<Product, bool>> BuildFilter(
        string? title = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? status = null,
        int? minStock = null,
        int? maxStock = null)
    {
        var filters = new List<Expression<Func<Product, bool>>>();

        if (!string.IsNullOrWhiteSpace(title))
        {
            filters.Add(BuildTitleFilter(title));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            filters.Add(BuildCategoryFilter(category));
        }

        if (minPrice.HasValue)
        {
            filters.Add(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            filters.Add(p => p.Price <= maxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            filters.Add(BuildStatusFilter(status));
        }

        if (minStock.HasValue)
        {
            filters.Add(p => p.Stock >= minStock.Value);
        }

        if (maxStock.HasValue)
        {
            filters.Add(p => p.Stock <= maxStock.Value);
        }

        return CombineFilters(filters);
    }

    private Expression<Func<Product, bool>> BuildTitleFilter(string title)
    {
        if (title.StartsWith("*") && title.EndsWith("*"))
        {
            // *termo* - contém
            var term = title.Trim('*');
            return p => p.Title.ToLower().Contains(term.ToLower());
        }
        else if (title.StartsWith("*"))
        {
            // *termo - termina com
            var term = title.TrimStart('*');
            return p => p.Title.ToLower().EndsWith(term.ToLower());
        }
        else if (title.EndsWith("*"))
        {
            // termo* - começa com
            var term = title.TrimEnd('*');
            return p => p.Title.ToLower().StartsWith(term.ToLower());
        }
        else
        {
            // termo exato (case insensitive)
            return p => p.Title.ToLower().Contains(title.ToLower());
        }
    }

    private Expression<Func<Product, bool>> BuildCategoryFilter(string category)
    {
        if (category.StartsWith("*") && category.EndsWith("*"))
        {
            // *termo* - contém
            var term = category.Trim('*');
            return p => p.Category.ToLower().Contains(term.ToLower());
        }
        else if (category.StartsWith("*"))
        {
            // *termo - termina com
            var term = category.TrimStart('*');
            return p => p.Category.ToLower().EndsWith(term.ToLower());
        }
        else if (category.EndsWith("*"))
        {
            // termo* - começa com
            var term = category.TrimEnd('*');
            return p => p.Category.ToLower().StartsWith(term.ToLower());
        }
        else
        {
            // termo exato (case insensitive)
            return p => p.Category.ToLower().Contains(category.ToLower());
        }
    }

    private Expression<Func<Product, bool>> BuildStatusFilter(string status)
    {
        if (Enum.TryParse<ProductStatus>(status, true, out var productStatus))
        {
            return p => p.Status == productStatus;
        }

        // Se não conseguir parsear, retorna filtro que não filtra nada
        return p => true;
    }

    private Expression<Func<Product, bool>> CombineFilters(List<Expression<Func<Product, bool>>> filters)
    {
        if (!filters.Any())
        {
            return p => true; // Sem filtros, retorna todos
        }

        Expression<Func<Product, bool>> combinedFilter = filters.First();

        for (int i = 1; i < filters.Count; i++)
        {
            combinedFilter = CombineWithAnd(combinedFilter, filters[i]);
        }

        return combinedFilter;
    }

    private Expression<Func<Product, bool>> CombineWithAnd(
        Expression<Func<Product, bool>> left,
        Expression<Func<Product, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(Product), "p");
        var leftBody = ReplaceParameter(left.Body, left.Parameters[0], parameter);
        var rightBody = ReplaceParameter(right.Body, right.Parameters[0], parameter);
        var andExpression = Expression.AndAlso(leftBody, rightBody);
        return Expression.Lambda<Func<Product, bool>>(andExpression, parameter);
    }

    private Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}
