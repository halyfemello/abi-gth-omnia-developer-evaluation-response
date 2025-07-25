using DeveloperEvaluation.Core.Domain.Entities;
using System.Linq.Expressions;

namespace DeveloperEvaluation.Core.Application.Services;

public class CartFilterBuilder
{

    public Expression<Func<Cart, bool>> BuildFilter(
        int? userId = null,
        DateTime? minDate = null,
        DateTime? maxDate = null)
    {
        Expression<Func<Cart, bool>> filter = cart => true;

        if (userId.HasValue)
        {
            var userFilter = BuildUserFilter(userId.Value);
            filter = CombineFilters(filter, userFilter);
        }

        if (minDate.HasValue)
        {
            var minDateFilter = BuildMinDateFilter(minDate.Value);
            filter = CombineFilters(filter, minDateFilter);
        }

        if (maxDate.HasValue)
        {
            var maxDateFilter = BuildMaxDateFilter(maxDate.Value);
            filter = CombineFilters(filter, maxDateFilter);
        }

        return filter;
    }

    private Expression<Func<Cart, bool>> BuildUserFilter(int userId)
    {
        return cart => cart.UserId == userId;
    }

    private Expression<Func<Cart, bool>> BuildMinDateFilter(DateTime minDate)
    {
        return cart => cart.Date >= minDate;
    }

    private Expression<Func<Cart, bool>> BuildMaxDateFilter(DateTime maxDate)
    {
        return cart => cart.Date <= maxDate;
    }

    private Expression<Func<Cart, bool>> CombineFilters(
        Expression<Func<Cart, bool>> filter1,
        Expression<Func<Cart, bool>> filter2)
    {
        var parameter = Expression.Parameter(typeof(Cart), "cart");
        var body1 = Expression.Invoke(filter1, parameter);
        var body2 = Expression.Invoke(filter2, parameter);
        var combined = Expression.AndAlso(body1, body2);
        return Expression.Lambda<Func<Cart, bool>>(combined, parameter);
    }

    public bool IsValidFilter(int? userId = null, DateTime? minDate = null, DateTime? maxDate = null)
    {
        if (userId.HasValue && userId.Value <= 0)
            return false;

        if (minDate.HasValue && maxDate.HasValue && minDate > maxDate)
            return false;

        return true;
    }
}
