using System.Linq.Expressions;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Domain.Entities;

namespace DeveloperEvaluation.Core.Application.Services;

public class SaleFilterBuilder
{
    public Expression<Func<Sale, bool>>? BuildFilter(SalesQueryParametersDto parameters)
    {
        var filters = new List<Expression<Func<Sale, bool>>>();

        if (!string.IsNullOrWhiteSpace(parameters.SaleNumber))
        {
            if (parameters.SaleNumber.Contains('*'))
            {
                var pattern = parameters.SaleNumber.Replace("*", "");
                if (parameters.SaleNumber.StartsWith('*') && parameters.SaleNumber.EndsWith('*'))
                {
                    filters.Add(s => s.SaleNumber.Contains(pattern));
                }
                else if (parameters.SaleNumber.StartsWith('*'))
                {
                    filters.Add(s => s.SaleNumber.EndsWith(pattern));
                }
                else if (parameters.SaleNumber.EndsWith('*'))
                {
                    filters.Add(s => s.SaleNumber.StartsWith(pattern));
                }
            }
            else
            {
                filters.Add(s => s.SaleNumber == parameters.SaleNumber);
            }
        }

        if (parameters.CustomerId.HasValue)
        {
            filters.Add(s => s.CustomerId == parameters.CustomerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameters.CustomerName))
        {
            if (parameters.CustomerName.Contains('*'))
            {
                var pattern = parameters.CustomerName.Replace("*", "");
                if (parameters.CustomerName.StartsWith('*') && parameters.CustomerName.EndsWith('*'))
                {
                    filters.Add(s => s.CustomerName.Contains(pattern));
                }
                else if (parameters.CustomerName.StartsWith('*'))
                {
                    filters.Add(s => s.CustomerName.EndsWith(pattern));
                }
                else if (parameters.CustomerName.EndsWith('*'))
                {
                    filters.Add(s => s.CustomerName.StartsWith(pattern));
                }
            }
            else
            {
                filters.Add(s => s.CustomerName == parameters.CustomerName);
            }
        }

        if (!string.IsNullOrWhiteSpace(parameters.CustomerEmail))
        {
            if (parameters.CustomerEmail.Contains('*'))
            {
                var pattern = parameters.CustomerEmail.Replace("*", "");
                if (parameters.CustomerEmail.StartsWith('*') && parameters.CustomerEmail.EndsWith('*'))
                {
                    filters.Add(s => s.CustomerEmail.Contains(pattern));
                }
                else if (parameters.CustomerEmail.StartsWith('*'))
                {
                    filters.Add(s => s.CustomerEmail.EndsWith(pattern));
                }
                else if (parameters.CustomerEmail.EndsWith('*'))
                {
                    filters.Add(s => s.CustomerEmail.StartsWith(pattern));
                }
            }
            else
            {
                filters.Add(s => s.CustomerEmail == parameters.CustomerEmail);
            }
        }

        if (parameters.BranchId.HasValue)
        {
            filters.Add(s => s.BranchId == parameters.BranchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameters.BranchName))
        {
            if (parameters.BranchName.Contains('*'))
            {
                var pattern = parameters.BranchName.Replace("*", "");
                if (parameters.BranchName.StartsWith('*') && parameters.BranchName.EndsWith('*'))
                {
                    filters.Add(s => s.BranchName.Contains(pattern));
                }
                else if (parameters.BranchName.StartsWith('*'))
                {
                    filters.Add(s => s.BranchName.EndsWith(pattern));
                }
                else if (parameters.BranchName.EndsWith('*'))
                {
                    filters.Add(s => s.BranchName.StartsWith(pattern));
                }
            }
            else
            {
                filters.Add(s => s.BranchName == parameters.BranchName);
            }
        }

        if (parameters.MinSaleDate.HasValue)
        {
            filters.Add(s => s.SaleDate >= parameters.MinSaleDate.Value);
        }

        if (parameters.MaxSaleDate.HasValue)
        {
            filters.Add(s => s.SaleDate <= parameters.MaxSaleDate.Value);
        }

        if (parameters.MinTotalAmount.HasValue)
        {
            filters.Add(s => s.TotalAmount >= parameters.MinTotalAmount.Value);
        }

        if (parameters.MaxTotalAmount.HasValue)
        {
            filters.Add(s => s.TotalAmount <= parameters.MaxTotalAmount.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Status))
        {
            var isActive = parameters.Status.ToLowerInvariant() switch
            {
                "active" => true,
                "cancelled" => false,
                _ => (bool?)null
            };

            if (isActive.HasValue)
            {
                filters.Add(s => s.IsCancelled != isActive.Value);
            }
        }

        if (!filters.Any())
            return null;

        if (filters.Count == 1)
            return filters[0];

        Expression<Func<Sale, bool>> combinedFilter = filters[0];
        for (int i = 1; i < filters.Count; i++)
        {
            combinedFilter = CombineWithAnd(combinedFilter, filters[i]);
        }

        return combinedFilter;
    }

    private Expression<Func<T, bool>> CombineWithAnd<T>(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var firstBody = ReplaceParameter(first.Body, first.Parameters[0], parameter);
        var secondBody = ReplaceParameter(second.Body, second.Parameters[0], parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
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
